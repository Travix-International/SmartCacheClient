using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SmartCacheClient.Dotnet
{
    public interface IClientCertificateProvider
    {
        IEnumerable<X509Certificate> GetCertificates();
    }
}