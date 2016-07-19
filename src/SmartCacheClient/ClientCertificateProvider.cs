using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SmartCacheClient.Dotnet
{
    public class ClientCertificateProvider : IClientCertificateProvider
    {
        private readonly X509Certificate cachedCertificate = null;

        public ClientCertificateProvider(IConfiguration configuration, ILogger<ClientCertificateProvider> logger)
        {
            var certPath = configuration["ClientCertificateFilePath"];
            try
            {
                if (!String.IsNullOrEmpty(certPath) && File.Exists(certPath))
                {
                    //cachedCertificate = new X509Certificate(File.ReadAllBytes(certPath), "dotnettest");
                    cachedCertificate = new X509Certificate2(File.ReadAllBytes(certPath));
                    var cachedCertificate2 = new X509Certificate2(File.ReadAllBytes(certPath));
                    //cachedCertificate = new X509Certificate(GetBytesFromPEM(File.ReadAllText(certPath), "CERTIFICATE"), "dotnettest");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"An error happened during reading the configured client certificate. Exception: {ex}");
            }
        }

        public IEnumerable<X509Certificate> GetCertificates()
        {
            if (cachedCertificate != null)
            {
                return new List<X509Certificate> { cachedCertificate };
            }

            return new List<X509Certificate>();
        }

        byte[] GetBytesFromPEM(string pemString, string section)
        {
            var header = String.Format("-----BEGIN {0}-----", section);
            var footer = String.Format("-----END {0}-----", section);

            var start = pemString.IndexOf(header, StringComparison.Ordinal);
            if (start < 0)
                return null;

            start += header.Length;
            var end = pemString.IndexOf(footer, start, StringComparison.Ordinal) - start;

            if (end < 0)
                return null;

            return Convert.FromBase64String(pemString.Substring(start, end));
        }
    }
}