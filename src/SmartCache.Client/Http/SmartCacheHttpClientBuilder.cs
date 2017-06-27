using System.Net.Http;
using System.Linq;
using SmartCache.Client.Certificates;
using System;

namespace SmartCache.Client.Http
{
    public class SmartCacheHttpClientBuilder : IHttpClientBuilder
    {
        private readonly IClientCertificateProvider clientCertificateProvider;

        private readonly IHttpClientFactory httpClientFactory;

        public IHttpClient HttpClient { get; }

        public SmartCacheHttpClientBuilder(IClientCertificateProvider clientCertificateProvider, IHttpClientFactory httpClientFactory)
        {
            this.clientCertificateProvider = clientCertificateProvider;
            this.httpClientFactory = httpClientFactory;

            HttpClient = Build();
        }

        private IHttpClient Build()
        {
            var handler = new HttpClientHandler();

            var certificates = clientCertificateProvider.GetCertificates().ToArray();

            handler.ClientCertificates.AddRange(certificates);

            return httpClientFactory.Create(handler);
        }
    }
}