using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SmartCache.Client.Certificates
{
    public interface IClientCertificateProvider
    {
        IEnumerable<X509Certificate> GetCertificates();
    }
}