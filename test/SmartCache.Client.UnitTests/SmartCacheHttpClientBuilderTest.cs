using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Moq;
using SmartCache.Client.Certificates;
using SmartCache.Client.Http;
using Xunit;

namespace SmartCache.Client.UnitTests
{
    public class SmartCacheHttpClientBuilderTest
    {
        private readonly Mock<IClientCertificateProvider> clientCertificateProvider;

        private readonly Mock<IHttpClientFactory> httpClientFactory;

        public SmartCacheHttpClientBuilderTest()
        {
            clientCertificateProvider = new Mock<IClientCertificateProvider>();
            httpClientFactory = new Mock<IHttpClientFactory>();
        }

        [Fact]
        public void GetHttpClient_NoCertificatesProvided_NoCertificateAddedToTheHandler()
        {
            clientCertificateProvider
                .Setup(c => c.GetCertificates())
                .Returns(new List<X509Certificate>());

            var sut = new SmartCacheHttpClientBuilder(clientCertificateProvider.Object, httpClientFactory.Object);

            var result = sut.HttpClient;

            httpClientFactory.Verify(
                factory => factory.Create(It.Is<HttpClientHandler>(handler => handler.ClientCertificates.Count == 0)));
        }

        [Fact]
        public void GetHttpClient_TwoCertificatesProvided_TwoCertificateAddedToTheHandler()
        {
            clientCertificateProvider
                .Setup(c => c.GetCertificates())
                .Returns(new List<X509Certificate>
                {
                    new X509Certificate(),
                    new X509Certificate()
                });

            var sut = new SmartCacheHttpClientBuilder(clientCertificateProvider.Object, httpClientFactory.Object);

            var result = sut.HttpClient;

            httpClientFactory.Verify(
                factory => factory.Create(It.Is<HttpClientHandler>(handler => handler.ClientCertificates.Count == 2)));
        }
    }
}