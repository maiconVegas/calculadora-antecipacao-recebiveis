using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;

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
