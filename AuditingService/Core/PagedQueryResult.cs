using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Core
{
    public sealed class PagedQueryResult<T>
    {
        public IEnumerable<T> Result { get; set; }

        public bool HasMoreResults { get; set; }

        public string ContinuationToken { get; set; }
    }
}
