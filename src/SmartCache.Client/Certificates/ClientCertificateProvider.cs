using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SmartCache.Client.Certificates
{
    public class ClientCertificateProvider : IClientCertificateProvider
    {
        private readonly X509Certificate2 cachedCertificate = null;

        public ClientCertificateProvider(IConfiguration configuration, ILogger<ClientCertificateProvider> logger)
        {
            logger.LogInformation("Initializing ClentCertificateProvider.");

            var certPath = configuration["SmartCacheClient:ClientCertificateFilePath"];

            try
            {
                if (!String.IsNullOrEmpty(certPath) && File.Exists(certPath))
                {
                    cachedCertificate = new X509Certificate2(File.ReadAllBytes(certPath), configuration["SmartCacheClient:ClientCertificateFilePath"]);

                    logger.LogInformation("Certificate processed successfully");
                    logger.LogInformation($"Subject: {cachedCertificate.Subject}");
                    logger.LogInformation($"FriendlyName: {cachedCertificate.FriendlyName}");
                    logger.LogInformation($"IssuerName: {cachedCertificate.IssuerName}");
                    logger.LogInformation($"HasPrivateKey: {cachedCertificate.HasPrivateKey}");
                }
                else
                {
                    logger.LogInformation("The certificate file was not found.");
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
    }
}