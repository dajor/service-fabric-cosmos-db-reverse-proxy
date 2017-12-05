using AuditingService.Lexicon;
using CommonBase;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Core
{
    internal class DocumentClientProvider
    {
        private readonly IDocumentClient _client;

        private readonly AppSettings _appSettings;

        internal IDocumentClient Client => _client;

        internal DocumentClientProvider(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _client = CreateClient();
        }

        /// <summary>
        /// Reads the app settings and uses it to create the documentClient.
        /// </summary>
        private IDocumentClient CreateClient()
        {
            var endpointUrl = new Uri(_appSettings.DocumentDBEndpointUrl);
            var authKey = _appSettings.DocumentDBAuthKey;

            return new DocumentClient(endpointUrl, authKey);
        }
    }
}
