using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace CalculadoraAntecipacaoRecebiveis.Infrastructure.Integrations.BlobStorage
{
    public class DownloadService : BlobService
    {
        public DownloadService(IOptions<BlobStorageConfiguration> configuration) : base(configuration)
        {
        }

        public async Task<MemoryStream> DownloadAsync(string containerName, string fileName, CancellationToken cancellationToken = default)
        {
            var blobContainer = BlobServiceClient.GetBlobContainerClient(containerName);
            var blockBlob = blobContainer.GetBlobClient(fileName);

            var memStream = new MemoryStream();
            await blockBlob.DownloadToAsync(memStream, cancellationToken);
            memStream.Seek(0, SeekOrigin.Begin);

            return memStream;
        }
    }
}
