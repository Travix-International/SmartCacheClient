using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using SmartCache.Client.Certificates;

namespace SmartCache.Client.IntegrationTests
{
    public class EmptyClientCertProvider : IClientCertificateProvider
    {
        public IEnumerable<X509Certificate> GetCertificates()
        {
            return new List<X509Certificate>();
        }
    }
}