using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Interfaces.Repositories
{
    public interface IItemsRepository
    {
       

        /// <summary>
        /// Returns a document from the documentDB by the given id.
        /// This method also returns soft deleted items if requested.
        /// </summary>
        /// <param name="id">The id of the object to return.</param>
        /// <param name="partition">The partition of the object to return.</param>
        /// <param name="doTraceWriting">When an exception is raised, it will be written to the tracing. Setting this parameter
        /// false will prevent the tracewriting.</param>
        /// <returns>The object or <c>null</c> if not exists.</returns>
        Task<Document> GetById(string id, string partition, bool doTraceWriting = true);

        /// <summary>
        /// Returns a document from the documentDB by the given id.
        /// This method also returns soft deleted items if requested.
        /// </summary>
        /// <param name="id">The id of the object to return.</param>
        /// <param name="doTraceWriting">When an exception is raised, it will be written to the tracing. Setting this parameter
        /// false will prevent the tracewriting.</param>
        /// <returns>The object or <c>null</c> if not exists.</returns>
        Task<Document> GetById(Guid id, bool doTraceWriting = true);

        /// <summary>
        /// Returns documents from the documentDB by the given ids.
        /// This method also returns soft deleted items if requested.
        /// </summary>
        /// <param name="ids">The ids of the objects to return.</param>
        /// <param name="doTraceWriting">When an exception is raised, it will be written to the tracing. Setting this parameter
        /// false will prevent the tracewriting.</param>
        /// <returns>The objects or <c>null</c> if not exists.</returns>
        Task<IList<Document>> GetByIds(List<Guid> ids, bool doTraceWriting = true);

        /// <summary>
        /// Updates the document with the given item by saving it in the documentDB.
        /// </summary>
        /// <param name="item">The item that needs to be saved in the documentDB.</param>
        /// <returns>The item as it is saved.</returns>
        Task<Document> Save(JObject item);

        /// <summary>
        /// Removes a item from the database.
        /// WARNING: Removes means physicaly removed, no enddates of flags will be set.
        /// </summary>
        /// <param name="document">The item that needs to be removed.</param>
        Task Remove(Document document);
    }
}
