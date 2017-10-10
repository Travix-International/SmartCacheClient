using System;
using System.Threading.Tasks;
using Xunit;
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
        public async Task GetAsync_CallNotFoundUrl_NullReturned()
        {
            var sut = new SmartCacheClient(new SmartCacheHttpClientBuilder(new EmptyClientCertProvider(), new SmartCacheHttpClientFactory()));

            var result = await sut.GetAsync<Post>(new Uri("https://jsonplaceholder.typicode.com/posts/doesntexist"));

            Assert.Equal(null, result);
        }

        [Fact]
        public async Task GetAsync_CallApiWithCustomHttpClientFactoryAndDelegatingHandler_ShouldUseTheCustomDeletagatingHandler()
        {
            var delegatingHandlerUsed = false;

            Action delegatingHandlerCallBack = () => { delegatingHandlerUsed = true; };

            var sut = new SmartCacheClient(new SmartCacheHttpClientBuilder(new EmptyClientCertProvider(), new CustomSmartCacheHttpClientFactory(delegatingHandlerCallBack)));

            var result = await sut.GetAsync<Post>(new Uri("https://jsonplaceholder.typicode.com/posts/3"));

            Assert.NotNull(result);
            Assert.Equal(3, result.Id);
            Assert.True(delegatingHandlerUsed);
        }
    }
}