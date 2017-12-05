using AuditingService.Core;
using AuditingService.Interfaces.Logics;
using AuditingService.Interfaces.Repositories;
using Microsoft.Azure.Documents;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuditingService.Logics
{
    public class ItemsLogic : IItemsLogic
    {
        private readonly IItemsRepository _itemsRepository;

        public ItemsLogic(IItemsRepository repository)
        {
            _itemsRepository = repository;
        }

        /// <summary>
        /// Saves a new item as document by saving it in the documentDB.
        /// </summary>
        /// <param name="item">The item that needs to be saved in the documentDB.</param>
        /// <returns>The item as it is saved.</returns>
        public async Task<Document> Create(JObject item)
        {
            Guard.ArgumentNotNull(nameof(item), item);
            return await _itemsRepository.Save(item);
        }
    }
}
