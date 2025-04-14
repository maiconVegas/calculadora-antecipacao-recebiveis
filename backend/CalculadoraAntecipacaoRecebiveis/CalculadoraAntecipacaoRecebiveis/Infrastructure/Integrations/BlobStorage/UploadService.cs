using Microsoft.Extensions.Options;

namespace CalculadoraAntecipacaoRecebiveis.Infrastructure.Integrations.BlobStorage
{
    public class UploadService : BlobService
    {
        public UploadService(IOptions<BlobStorageConfiguration> configuration) : base(configuration)
        {
        }

        public async Task DeleteAsync(string containerName, string fileName, CancellationToken cancellationToken = default)
        {
            var blobContainer = await CreateIfNotExistsAsync(containerName, cancellationToken);
            var blockBlob = blobContainer.GetBlobClient(fileName);

            await blockBlob.DeleteAsync(cancellationToken: cancellationToken);
            // Como é await com task async, não precisa de tipo de retorno
            // Por mais que seja Task, ele representa operação assicrona sem valor de retorno
            // Esse await é mais para todas as operações sejam concluidas antes do termino da execução
        }

        public async Task<string> UploadAsync(string containerName, string fileName, Stream fileStream, CancellationToken cancellationToken = default)
        {
            var blobContainer = await CreateIfNotExistsAsync(containerName, cancellationToken);
            var blockBlob = blobContainer.GetBlobClient(fileName);

            fileStream.Seek(0, SeekOrigin.Begin);

            await blockBlob.UploadAsync(fileStream, true, cancellationToken);

            return blockBlob.Uri.LocalPath;
            // retorna o caminho ou url do arquivo que foi armazenado, mas nesse projeto é necessário? hummm...
        }
    }
}
