using System;
using System.Threading.Tasks;
using Xunit;
using SmartCache.Client;
using SmartCache.Client.Http;

namespace SmartCache.Client.IntegrationTests
{
    public class SmartCacheClientTest
    {
        [Fact]
        public async Task GetAsync_CallJsonApiSuccessfully_DeserializedObjectReturned()
        {
            var sut = new SmartCacheClient(new SmartCacheHttpClientBuilder(new EmptyClientCertProvider(), new SmartCacheHttpClientFactory()));

            var result = await sut.GetAsync<Post>(new Uri("https://jsonplaceholder.typicode.com/posts/3"));

            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
        }

        [Fact]
        public async Task ConfigApiHalJsonTest()
        {
            var sut = new SmartCacheClient(new SmartCacheHttpClientBuilder(new EmptyClientCertProvider(), new SmartCacheHttpClientFactory()));

            var result = await sut.GetAsync<string>(new Uri("http://configurationapi.travix.com/Setting/DefaultLanguageCode/CheapticketsBE"));

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAsync_CallNotFoundUrl_NullReturned()
        {
            var sut = new SmartCacheClient(new SmartCacheHttpClientBuilder(new EmptyClientCertProvider(), new SmartCacheHttpClientFactory()));

            var result = await sut.GetAsync<Post>(new Uri("https://jsonplaceholder.typicode.com/posts/doesntexist"));

            Assert.Equal(null, result);
        }
    }
}