using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Data;
using System.Text.Json.Nodes;

namespace FunctionApp1
{


    public class Function1
    {
        private readonly string CV_Endpoint;
        private readonly string CV_Key;
        private readonly string Blob_ConnectionString;
        private readonly string Library_Name;

        private UVS_V1_Wrapper uvs;
        private UVS_V2_Wrapper uvs2;


        public Function1(IConfiguration config)
        {
            CV_Endpoint = config["computer_vision_endpoint"];
            CV_Key = config["computer_vision_key"];
            Blob_ConnectionString = config["libraryConnectionString"];
            Library_Name = config["library_name"];
            uvs = new UVS_V1_Wrapper(CV_Endpoint, CV_Key, Library_Name);
            uvs2 = new UVS_V2_Wrapper(CV_Endpoint, CV_Key, Library_Name);
        }

        private (string,string) parseBlobUrl(string fileBlobUrl)
        {
            Uri blobUri = new Uri(fileBlobUrl);
            string containerName = blobUri.Segments[1].TrimEnd('/');
            string fileName = blobUri.Segments[2];

            return(containerName, fileName);
        }


        private string GenerateDocumentId(string fileBlobUrl)
        {
            (string, string) parsedUrl = parseBlobUrl(fileBlobUrl);
            return parsedUrl.Item2;
        }

        private string GenerateSaSUrl(string fileBlobUrl)
        {
             (string,string) parsedUrl = parseBlobUrl(fileBlobUrl);
            // Create a BlobClient object from the blob URL and connection string
            BlobClient blobClient = new BlobClient(Blob_ConnectionString, parsedUrl.Item1, parsedUrl.Item2);

            // Create a BlobSasBuilder object to specify the SAS parameters
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                // Set the expiry time and permissions for the SAS
                ExpiresOn = DateTime.UtcNow.AddHours(1),
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b"
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri.ToString();
        }

        private async Task TriggerIndex(string fileBlobUrl)
        {
            return;
        }

        [FunctionName("BlobTrigger")]
        public async void Run([BlobTrigger("videolibrary/{name}", Connection = "libraryConnectionString")] BlobClient myQueueItem, ILogger log)
        {
            
            log.LogInformation($"C# Queue trigger function processed; {myQueueItem.Uri.ToString()}");

            string SasUrl = GenerateSaSUrl(myQueueItem.Uri.ToString());

            var docId = GenerateDocumentId(myQueueItem.Uri.ToString());

            var result = await uvs.addDocumentToIndex(SasUrl, docId);

            await uvs2.addDocumentToIndex(SasUrl, docId);
            
            log.LogInformation($"C# sas url is {SasUrl}");
        }
    }
}
