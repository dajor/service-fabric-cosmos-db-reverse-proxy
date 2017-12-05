using AuditingService.Lexicon;
using CommonBase.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AuditingService.Core
{
    public class DataAccess : IDataAccess
    {
        private readonly IDocumentClient _client;
        private readonly DbOperationTraceWriter _trace;

        public DataAccess(IDocumentClient client)
        {
            _client = client;
            _trace = new DbOperationTraceWriter(DocumentDBNames.DatabaseName);
        }

        /// <summary>
        /// Returns a document from the documentDB by the given id.
        /// This method also returns soft deleted items if requested.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="id">The id of the object to return.</param>
        /// <param name="partition">The partition of the object to return.</param>
        /// <param name="doTraceWriting">When an exception is raised, it will be written to the tracing. Setting this parameter
        /// false will prevent the tracewriting.</param>
        /// <returns>The object or <c>null</c> if not exists.</returns>
        public async Task<Document> GetById(string collectionName, string id, string partition, bool doTraceWriting = true)
        {
            try
            {
                Uri uri = CreateDocumentLink(collectionName, id);
                ResourceResponse<Document> response = await _client.ReadDocumentAsync(uri);
                //ResourceResponse<Document> response = await _client.ReadDocumentAsync(uri, GetRequestOptions(partition));
                return response.Resource;
            }
            catch (DocumentClientException documentClientException)
            {
                if (doTraceWriting)
                {
                    _trace.TraceWarning( ErrorCodes.DBDocumentNotFound, collectionName, id, partition);
                }

                if (documentClientException.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        /// <summary>
        /// Updates the document with the given object instance by saving it in the documentDB.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="objectInstance">The object instance that needs to be saved in the documentDB.</param>
        /// <param name="partition">The partition of the object to save.</param>
        /// <returns>The object instance as it is saved.</returns>
        public async Task<Document> Save(string collectionName, object objectInstance, string partition)
        {
            try
            {
                Uri uri = CreateCollectionLink(collectionName);
                //ResourceResponse<Document> response = await _client.UpsertDocumentAsync(uri, objectInstance, GetRequestOptions(partition));
                ResourceResponse<Document> response = await _client.UpsertDocumentAsync(uri, objectInstance);
                return response.Resource;
            }
            catch (DocumentClientException documentClientException)
            {
                switch (documentClientException.StatusCode)
                {
                    case HttpStatusCode.BadRequest: _trace.TraceError(ErrorCodes.DBInvalidDocument, collectionName, objectInstance); break;
                    case HttpStatusCode.Forbidden: _trace.TraceError(ErrorCodes.DBCollectionFull, collectionName); break;
                    case HttpStatusCode.RequestEntityTooLarge: _trace.TraceError(ErrorCodes.DBDocumentSizeExceedsMaximumSize, collectionName, objectInstance); break;
                    default: _trace.TraceError(ErrorCodes.DBUncommonHttpStatusFound, collectionName, documentClientException.StatusCode, objectInstance); break;
                }
                throw;
            }
        }

        /// <summary>
        /// Deletes the object instance softly. Will be implemented as soon as a design is made.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="id">Id of the object instance that must be flagged as removed.</param>
        /// <param name="partition">The partition of the object to delete.</param>
        public Task Delete(string collectionName, string id, string partition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the object instance softly. Will be implemented as soon as a design is made.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="document">The document that needs to be removed.</param>
        /// <param name="partition">The partition of the object to be removed.</param>
        public Task Delete(string collectionName, Document document, string partition)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the object instance by removing the document from documentdb.
        /// WARNING: The document will be hard deleted from the db, not by setting an enddate field.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="id">The id of the object instance that needs to be deleted forever.</param>
        /// <param name="partition">The partition of the object to remove.</param>
        public async Task Remove(string collectionName, string id, string partition)
        {
            try
            {
                Uri uri = CreateDocumentLink(collectionName, id);
                await _client.DeleteDocumentAsync(uri, GetRequestOptions(partition));
            }
            catch (DocumentClientException documentClientException)
            {
                if (documentClientException.StatusCode == HttpStatusCode.NotFound)
                {
                    _trace.TraceInformation(ErrorCodes.DBDocumentNotFound, collectionName, id, partition);
                }
                throw;
            }
        }

        /// <summary>
        /// Executes a query and returns the result of the execution.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="query">The query that needs to be executed.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <returns>The result of the query.</returns>
        public async Task<IEnumerable<T>> ExecuteSqlQuery<T>(string collectionName, string query, SqlParameterCollection parameters)
        {
            Guard.ArgumentNotNull(nameof(parameters), parameters);
            var querySpec = new SqlQuerySpec(query, parameters);
            var documentBatches = new List<IEnumerable<T>>();
            Uri uri = CreateCollectionLink(collectionName);

            try
            {
                IQueryable<T> queryable = _client.CreateDocumentQuery<T>(uri, querySpec, new FeedOptions { EnableCrossPartitionQuery = true });
                IDocumentQuery<T> documentQuery = queryable.AsDocumentQuery();

                do
                {
                    FeedResponse<T> documentBatch = await documentQuery.ExecuteNextAsync<T>();
                    documentBatches.Add(documentBatch);
                }
                while (documentQuery.HasMoreResults);

                return documentBatches.SelectMany(documentBatch => documentBatch as T[] ?? documentBatch.ToArray());
            }
            catch (Exception exception)
            {
                _trace.TraceError(ErrorCodes.DBGeneralSqlQueryError, collectionName, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Executes a paged query and returns a page of the execution.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="query">The query that needs to be executed.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <param name="partitionKey">The partition key value.</param>
        /// <param name="continuationToken">The token for the continuation of the query, or <c>null</c>
        /// if the first page is requested.</param>
        /// <param name="pageSize">The maximum number of items per page.</param>
        /// <returns>The result of the query.</returns>
        public async Task<PagedQueryResult<T>> ExecutePagedSqlQuery<T>(
            string collectionName,
            string query,
            SqlParameterCollection parameters,
            string partitionKey,
            string continuationToken,
            int pageSize)
        {
            Guard.ArgumentNotNull(nameof(parameters), parameters);
            Guard.ArgumentNotNull(nameof(partitionKey), partitionKey);

            var querySpec = new SqlQuerySpec(query, parameters);
            Uri uri = CreateCollectionLink(collectionName);
            FeedOptions feedOptions = new FeedOptions
            {
                MaxItemCount = pageSize,
                EnableCrossPartitionQuery = false,
                PartitionKey = new PartitionKey(partitionKey),
                RequestContinuation = continuationToken
            };

            try
            {
                IDocumentQuery<T> documentQuery = _client.CreateDocumentQuery<T>(uri, querySpec, feedOptions).AsDocumentQuery();

                FeedResponse<T> page = await documentQuery.ExecuteNextAsync<T>();

                PagedQueryResult<T> result = new PagedQueryResult<T>
                {
                    Result = page,
                    HasMoreResults = documentQuery.HasMoreResults,
                    ContinuationToken = page.ResponseContinuation
                };

                return result;
            }
            catch (Exception exception)
            {
                _trace.TraceError(ErrorCodes.DBGeneralSqlQueryError, collectionName, exception.Message);
                throw;
            }
        }


        /// <summary>
        /// Creates the link to an document for the given id.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="documentId">Id of the document to which the link must be created.</param>
        /// <returns>An URI instance to the document with the given id.</returns>
        private Uri CreateDocumentLink(string collectionName, string documentId)
        {
            return UriFactory.CreateDocumentUri(
                DocumentDBNames.DatabaseName,
                collectionName,
                documentId);
        }

        /// <summary>
        /// Creates the link to the collection.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <returns>An URI instance to item collection.</returns>
        private Uri CreateCollectionLink(string collectionName)
        {
            return UriFactory.CreateDocumentCollectionUri(
                DocumentDBNames.DatabaseName,
                collectionName);
        }

        private RequestOptions GetRequestOptions(string partition)
        {
            return new RequestOptions
            {
                PartitionKey = new PartitionKey(partition)
            };
        }
    }
}
