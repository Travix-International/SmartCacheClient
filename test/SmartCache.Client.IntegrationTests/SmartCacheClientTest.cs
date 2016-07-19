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
            var sut = new SmartCacheClient(new SmartCacheHttpClientFactory(), new EmptyClientCertProvider());

            var result = await sut.GetAsync<Post>(new Uri("https://jsonplaceholder.typicode.com/posts/3"));

            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
        }

        [Fact]
        public async Task GetAsync_CallNotFoundUrl_NullReturned()
        {
            var sut = new SmartCacheClient(new SmartCacheHttpClientFactory(), new EmptyClientCertProvider());

            var result = await sut.GetAsync<Post>(new Uri("https://jsonplaceholder.typicode.com/posts/doesntexist"));

            Assert.Equal(null, result);
        }
    }
}