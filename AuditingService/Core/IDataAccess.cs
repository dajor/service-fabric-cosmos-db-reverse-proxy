using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Core
{
    public interface IDataAccess
    {
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
        Task<Document> GetById(string collectionName, string id, string partition, bool doTraceWriting = true);

        /// <summary>
        /// Updates the document with the given object instance by saving it in the documentDB.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="objectInstance">The object instance that needs to be saved in the documentDB.</param>
        /// <param name="partition">The partition of the object to save.</param>
        /// <returns>The object instance as it is saved.</returns>
        Task<Document> Save(string collectionName, object objectInstance, string partition);

        /// <summary>
        /// Deletes the object instance by setting an enddate field.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="id">Id of the object instance that must be flagged as removed.</param>
        /// <param name="partition">The partition of the object to delete.</param>
        Task Delete(string collectionName, string id, string partition);

        /// <summary>
        /// Deletes the object instance by setting an enddate field.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="document">The document that needs to be removed.</param>
        /// <param name="partition">The partition of the object to delete.</param>
        Task Delete(string collectionName, Document document, string partition);

        /// <summary>
        /// Removes the object instance by removing the document from documentdb.
        /// WARNING: The document will be hard deleted from the db, not by setting an enddate field.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="id">The id of the object instance that needs to be deleted forever.</param>
        /// <param name="partition">The partition of the object to remove.</param>
        Task Remove(string collectionName, string id, string partition);

        /// <summary>
        /// Executes a query and returns the result of the execution.
        /// </summary>
        /// <param name="collectionName">Name of the collection on which the operation must be performed.</param>
        /// <param name="query">The query that needs to be executed.</param>
        /// <param name="parameters">The parameters for the query.</param>
        /// <returns>The result of the query.</returns>
        Task<IEnumerable<T>> ExecuteSqlQuery<T>(string collectionName, string query, SqlParameterCollection parameters);

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
        Task<PagedQueryResult<T>> ExecutePagedSqlQuery<T>(
            string collectionName,
            string query,
            SqlParameterCollection parameters,
            string partitionKey,
            string continuationToken,
            int pageSize);
    }
}
