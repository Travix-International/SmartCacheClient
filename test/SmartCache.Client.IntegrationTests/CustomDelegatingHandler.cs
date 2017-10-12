using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SmartCache.Client.IntegrationTests
{
    public class CustomDelegatingHandler : DelegatingHandler
    {
        private Action delegatingHandlerCallBack;

        public CustomDelegatingHandler(HttpMessageHandler handler, Action delegatingHandlerCallBack)
        {
            InnerHandler = handler;
            this.delegatingHandlerCallBack = delegatingHandlerCallBack;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = base.SendAsync(request, cancellationToken);
            delegatingHandlerCallBack();

            return response;
        }
    }
}
