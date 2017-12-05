using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Interfaces.Logics
{
    public interface IItemsLogic
    {

        /// <summary>
        /// Saves a new item as document by saving it in the documentDB.
        /// </summary>
        /// <param name="item">The item that needs to be saved in the documentDB.</param>
        /// <returns>The item as it is saved.</returns>
        Task<Document> Create(JObject item);
    }
}
