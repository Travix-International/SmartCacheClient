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
        private readonly Mock<IHttpClientBuilder> httpClientBuilder;

        private readonly Mock<IHttpClient> httpClient;

        public SmartCacheClientTest()
        {
            httpClientBuilder = new Mock<IHttpClientBuilder>();
            httpClient = new Mock<IHttpClient>();

            httpClientBuilder.SetupGet(h => h.HttpClient).Returns(httpClient.Object);
        }
        
        [Fact]
        public async Task GetAsync_JsonHttpResponse_DeserializedJsonObjectReturned()
        {
            // Arrange
            var uri = new Uri("http://example.com/smartcache/post/3");
            httpClient.Setup(h => h.GetAsync(uri)).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"userId\": 2, \"id\": 3, \"title\": \"TestTitle\", \"body\": \"TestBody\" }", Encoding.UTF8, "application/json")
            });

            var sut = new SmartCacheClient(httpClientBuilder.Object);

            // Act
            var result = await sut.GetAsync<Post>(uri);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(2, result.UserId);
            Assert.Equal(3, result.Id);
            Assert.Equal("TestTitle", result.Title);
            Assert.Equal("TestBody", result.Body);
        }

        [Fact]
        public async Task GetAsync_JsonHttpResponseWithBoolContent_DeserializedBoolReturned()
        {
            // Arrange
            var uri = new Uri("http://example.com/smartcache/post/3");
            httpClient.Setup(h => h.GetAsync(uri)).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("true", Encoding.UTF8, "application/json")
            });

            var sut = new SmartCacheClient(httpClientBuilder.Object);

            // Act
            var result = await sut.GetAsync<bool>(uri);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetAsync_ResponseIsHalPlusJson_ContentSuccessfullyDeserialized()
        {
            // Arrange
            var uri = new Uri("http://example.com/smartcache/post/3");
            httpClient.Setup(h => h.GetAsync(uri)).ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{ \"userId\": 2, \"id\": 3, \"title\": \"TestTitle\", \"body\": \"TestBody\" }", Encoding.UTF8, "application/hal+json")
            });

            var sut = new SmartCacheClient(httpClientBuilder.Object);

            // Act
            var result = await sut.GetAsync<Post>(uri);

            // Assert
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

            var sut = new SmartCacheClient(httpClientBuilder.Object);

            var result = await sut.GetAsync<Post>(uri);

            Assert.Null(result);
        }
    }
}