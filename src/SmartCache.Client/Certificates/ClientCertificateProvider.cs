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
            logger.LogInformation("Initializing ClientCertificateProvider.");

            var certPath = configuration["SmartCacheClient:ClientCertificateFilePath"];

            try
            {
                if (String.IsNullOrEmpty(certPath))
                {
                    logger.LogInformation("Certificate file path was not configured.");
                    return;
                }

                logger.LogInformation($"Trying to load certificate from {certPath}");

                if (!File.Exists(certPath))
                {
                    logger.LogInformation("The certificate file was not found.");
                    return;
                }

                cachedCertificate = new X509Certificate2(File.ReadAllBytes(certPath), configuration["SmartCacheClient:ClientCertificatePassword"]);

                logger.LogInformation("Certificate processed successfully");
                logger.LogInformation($"Subject: {cachedCertificate.Subject}");
                logger.LogInformation($"FriendlyName: {cachedCertificate.FriendlyName}");
                logger.LogInformation($"IssuerName: {cachedCertificate.IssuerName}");
                logger.LogInformation($"HasPrivateKey: {cachedCertificate.HasPrivateKey}");
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