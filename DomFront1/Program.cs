using System;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace DomainFrontingExample
{
    class Program
    {
        static void Main()
        {
            string targetWebsite = "example.com";
            string frontingDomain = "https://www.cloudflare.com"; // Beispiel für ein CDN

            // Deaktiviert HTTP/2, um SNI zu umgehen (kann zu Kompatibilitätsproblemen führen)
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            // HttpClientHandler konfigurieren, um alle SSL-Zertifikatsfehler zu ignorieren
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Host = targetWebsite;
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/103.0.0.0 Safari/537.36");

                try
                {
                    var response = client.GetAsync(frontingDomain).Result;
                    Console.WriteLine(response.StatusCode);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Fehler beim Abrufen von {targetWebsite}: {ex.Message}");
                    Console.WriteLine($"Innerer Auslöser: {ex.InnerException?.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unbekannter Fehler beim Abrufen von {targetWebsite}: {ex.Message}");
                }
            }
        }
    }
}
