using AuditingService.Core;
using AuditingService.Interfaces.Repositories;
using AuditingService.Lexicon;
using AuditingService.Utilities;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Repositories
{
    public class ItemsRepository : IItemsRepository
    {
        private const string CollectionName = DocumentDBNames.ItemCollectionName;

        private readonly IDataAccess _dataAccess;

        private const string Partition = "classic";

        public ItemsRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
        

        /// <summary>
        /// Returns a document from the documentDB by the given id.
        /// This method also returns soft deleted items if requested.
        /// </summary>
        /// <param name="id">The id of the object to return.</param>
        /// <param name="doTraceWriting">When an exception is raised, it will be written to the tracing. Setting this parameter
        /// false will prevent the tracewriting.</param>
        /// <returns>The object or <c>null</c> if not exists.</returns>
        public Task<Document> GetById(Guid id, bool doTraceWriting = true)
        {
            string idAsString = id.ToString();
            return GetById(idAsString, Partition, doTraceWriting);
        }

        /// <summary>
        /// Returns a document from the documentDB by the given id.
        /// This method also returns soft deleted items if requested.
        /// </summary>
        /// <param name="id">The id of the object to return.</param>
        /// <param name="partition">The partition of the object to return.</param>
        /// <param name="doTraceWriting">When an exception is raised, it will be written to the tracing. Setting this parameter
        /// false will prevent the tracewriting.</param>
        /// <returns>The object or <c>null</c> if not exists.</returns>
        public Task<Document> GetById(string id, string partition, bool doTraceWriting = true)
        {
            return _dataAccess.GetById(CollectionName, id, partition, doTraceWriting);
        }

        /// <summary>
        /// Returns documents from the documentDB by the given ids.
        /// This method also returns soft deleted items if requested.
        /// </summary>
        /// <param name="ids">The ids of the objects to return.</param>
        /// <param name="doTraceWriting">When an exception is raised, it will be written to the tracing. Setting this parameter
        /// false will prevent the tracewriting.</param>
        /// <returns>The objects or <c>null</c> if not exists.</returns>
        public async Task<IList<Document>> GetByIds(List<Guid> ids, bool doTraceWriting = true)
        {
            string[] idsAsString = ids.Select(id => id.ToString()).ToArray();

            SqlParameterCollection parameters = new SqlParameterCollection();
            string inQueryPartIds = RepositoryUtilities.CreateInStatementForQuery("item.id", parameters, idsAsString);

            string queryString = $@"SELECT
                                      *
                                    FROM item
                                    WHERE {inQueryPartIds} AND item.instance = '{Partition}'";

            IEnumerable<Document> documents = await _dataAccess.ExecuteSqlQuery<Document>(CollectionName, queryString, parameters);
            return documents.ToList();
        }

        /// <summary>
        /// Updates the document with the given item by saving it in the documentDB.
        /// </summary>
        /// <param name="item">The item that needs to be saved in the documentDB.</param>
        /// <returns>The item as it is saved.</returns>
        public Task<Document> Save(JObject item)
        {
            item[CommonFields.Instance] = Partition;
            return _dataAccess.Save(CollectionName, item, Partition);
        }

        /// <summary>
        /// Removes a item from the database.
        /// WARNING: Removes means physicaly removed, no enddates of flags will be set.
        /// </summary>
        /// <param name="document">The item that needs to be removed.</param>
        public async Task Remove(Document document)
        {
            document.SetPropertyValue(CommonFields.Instance, Partition);
            await _dataAccess.Remove(CollectionName, document.Id, Partition);
        }

    }
}
