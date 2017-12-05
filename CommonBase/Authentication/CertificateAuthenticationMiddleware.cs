using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CommonBase.Authentication
{
    public class CertificateAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CertificateAuthenticationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IOptions<AppSettings> appSettings, IHostingEnvironment hostingEnvironment)
        {
            _appSettings = appSettings.Value;
            _next = next;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                string thumbprint = _appSettings.ClientCertificateThumbprint;
                X509Certificate2 authority = null;
                var certCollection = LoadCertificate(thumbprint);
                if (certCollection == null || certCollection.Count == 0)
                {
                    httpContext.Response.StatusCode = 403;
                    await WriteErrorMessage(_hostingEnvironment, httpContext, "Client certificate cannot be found in local certificate store.");
                    return;
                }
                else
                {
                    authority = certCollection[0];
                }

                StringValues headerValues;
                var certificate = string.Empty;

                if (httpContext.Request.Headers.TryGetValue("X-Client-Certificate", out headerValues))
                {
                    certificate = headerValues.FirstOrDefault();
                }
                else
                {
                    httpContext.Response.StatusCode = 403;
                    await WriteErrorMessage(_hostingEnvironment, httpContext, "Certificate not found in header.");
                    return;
                }
                byte[] data = Encoding.ASCII.GetBytes(certificate);
                X509Certificate2 certificateToValidate = new X509Certificate2(data);

                X509Chain chain = new X509Chain();
                chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;
                chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
                chain.ChainPolicy.VerificationTime = DateTime.Now;
                chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 0, 0);

                // This part is very important. You're adding your known root here.
                // It doesn't have to be in the computer store at all. Neither certificates do.
                chain.ChainPolicy.ExtraStore.Add(authority);

                bool isChainValid = chain.Build(certificateToValidate);

                if (!isChainValid)
                {
                    string[] errors = chain.ChainStatus
                        .Select(x => String.Format("{0} ({1})", x.StatusInformation.Trim(), x.Status))
                        .ToArray();
                    string certificateErrorsString = "Unknown errors.";

                    if (errors != null && errors.Length > 0)
                    {
                        certificateErrorsString = String.Join(", ", errors);
                    }
                    httpContext.Response.StatusCode = 403;
                    await WriteErrorMessage(_hostingEnvironment, httpContext, "Trust chain did not complete to the known authority anchor. Errors: " + certificateErrorsString);
                    return;
                }

                // This piece makes sure it actually matches your known root
                if (!chain.ChainElements
                    .Cast<X509ChainElement>()
                    .Any(x => x.Certificate.Thumbprint == authority.Thumbprint))
                {
                    httpContext.Response.StatusCode = 403;
                    await WriteErrorMessage(_hostingEnvironment, httpContext, "Trust chain did not complete to the known authority anchor. Thumbprints did not match.");
                    return;
                }

                await _next.Invoke(httpContext);

            }
            catch (Exception ex)
            {
                await httpContext.Response.WriteAsync(ex.Message);
            }
        }


        private static X509Certificate2Collection LoadCertificate(string thumbprint)
        {
            if (string.IsNullOrEmpty(thumbprint))
            {
                return null;
            }

            X509Store store = null;
            try
            {
                store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

                // select the certificate from store
                return store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            }
            finally
            {
                if (store != null)
                {
                    store.Close();
                }
            }
        }
        private static async Task WriteErrorMessage(IHostingEnvironment env, HttpContext httpContext, string msg)
        {
            if (env.IsDevelopment())
            {
                await httpContext.Response.WriteAsync(msg);
            }
        }
    }
}
