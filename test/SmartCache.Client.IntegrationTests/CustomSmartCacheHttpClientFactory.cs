using SmartCache.Client.Http;
using System;
using System.Net.Http;

namespace SmartCache.Client.IntegrationTests
{
    public class CustomSmartCacheHttpClientFactory : IHttpClientFactory
    {
        private Action delegatingHandlerCallBack;

        public CustomSmartCacheHttpClientFactory(Action delegatingHandlerCallBack)
        {
            this.delegatingHandlerCallBack = delegatingHandlerCallBack;
        }

        public IHttpClient Create(HttpMessageHandler handler)
        {
            return new SmartCacheHttpClient(new CustomDelegatingHandler(handler, delegatingHandlerCallBack));
        }
    }
}
