using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditingService.Utilities
{
    public static class ControllerUtilities
    {
        /// <summary>
        /// Creates a resource URI with an id based the requestUri and the specified path and pathSuffix.
        /// </summary>
        /// <param name="requestUri">The requestUri that needs to be extended with the id.</param>
        /// <param name="path">The path to use in the URI.</param>
        /// <param name="id">The id that needs to be added to the requestUri and path.</param>
        /// <param name="pathSuffix">The (optional) suffix to append.</param>
        /// <returns>The requestUri with the path and id.</returns>
        public static Uri CreateLocationHeaderUri(Uri requestUri, string path, string id, string pathSuffix = null)
        {
            UriBuilder builder = new UriBuilder(requestUri)
            {
                Path = path + id
            };

            if (!string.IsNullOrWhiteSpace(pathSuffix))
            {
                if (!builder.Path.EndsWith("/") && !pathSuffix.StartsWith("/"))
                {
                    builder.Path = builder.Path + "/";
                }
                builder.Path = builder.Path + pathSuffix.Trim();
            }

            return builder.Uri;
        }
    }
}
