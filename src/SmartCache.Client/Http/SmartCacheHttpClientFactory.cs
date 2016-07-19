using System.Net.Http;
using System.Linq;
using SmartCache.Client.Certificates;

namespace SmartCache.Client.Http
{
    public class SmartCacheHttpClientFactory : IHttpClientFactory
    {
        private readonly IClientCertificateProvider clientCertificateProvider;

        public SmartCacheHttpClientFactory(IClientCertificateProvider clientCertificateProvider)
        {
            this.clientCertificateProvider = clientCertificateProvider;
        }

        public IHttpClient Create()
        {
            var handler = new HttpClientHandler();

            var certificates = clientCertificateProvider.GetCertificates().ToArray();

            handler.ClientCertificates.AddRange(certificates);

            return new SmartCacheHttpClient(handler);
        }
    }
}