using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SmartCache.Client;
using SmartCache.Client.Certificates;
using SmartCache.Client.Http;

namespace SmartCache.Client.UnitTests
{
    public class SmartCacheClientTest
    {
        private readonly Mock<IClientCertificateProvider> clientCertificateProvider;

        private readonly Mock<IHttpClientFactory> httpClientFactory;

        private readonly Mock<IHttpClient> httpClient;

        public SmartCacheClientTest()
        {
            clientCertificateProvider = new Mock<IClientCertificateProvider>();
            httpClientFactory = new Mock<IHttpClientFactory>();
            httpClient = new Mock<IHttpClient>();

            clientCertificateProvider.Setup(c => c.GetCertificates()).Returns(new List<X509Certificate>());
            httpClientFactory.Setup(h => h.Create(It.IsAny<HttpClientHandler>())).Returns(httpClient.Object);
        }

        [Fact]
        public async Task GetAsync_JsonHttpResponse_DeserializedJsonObjectReturned()
        {
            var uri = new Uri("http://example.com/smartcache/post/3");
            httpClient.Setup(h => h.GetAsync(uri)).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"userId\": 2, \"id\": 3, \"title\": \"TestTitle\", \"body\": \"TestBody\" }", Encoding.UTF8, "application/json")
            });

            var sut = new SmartCacheClient(httpClientFactory.Object, clientCertificateProvider.Object);

            var result = await sut.GetAsync<Post>(uri);

            Assert.NotNull(result);

            Assert.Equal(2, result.UserId);
            Assert.Equal(3, result.Id);
            Assert.Equal("TestTitle", result.Title);
            Assert.Equal("TestBody", result.Body);
        }

        [Fact]
        public async Task GetAsync_404HttpResponse_NullReturned()
        {
            var uri = new Uri("http://example.com/smartcache/post/2");
            httpClient.Setup(h => h.GetAsync(uri)).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

            var sut = new SmartCacheClient(httpClientFactory.Object, clientCertificateProvider.Object);

            var result = await sut.GetAsync<Post>(uri);

            Assert.Null(result);
        }
    }
}