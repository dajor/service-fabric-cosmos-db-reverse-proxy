using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuditingService.Interfaces.Logics;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Documents;
using AuditingService.Utilities;
using AuditingService.Models;
using CommonBase.Controllers;

namespace AuditingService.Controllers
{
    [Route("api/audit")]
    public class AuditController : EdmlApiController
    {
        private readonly IItemsLogic _itemsLogic;

        public AuditController(IItemsLogic itemLogic)
        {
            _itemsLogic = itemLogic;
        }

        // POST api/audit
        [HttpPost]
        public Task<IActionResult> Post([FromBody]AuditModel auditModel)
        {
            return GuardedExecute(async () =>
            {
                var item = JObject.FromObject(auditModel);
                Document savedItem = await _itemsLogic.Create(item);
                return CreatedAtRoute("GetAuditFile", new { id = savedItem.Id }, savedItem);

            });
        }

    }
}
