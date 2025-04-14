using System.Net;
using System.Text;
using CalculadoraAntecipacaoRecebiveis.Infrastructure.Integrations.BlobStorage;
using CalculadoraAntecipacaoRecebiveis.Core.Messaging;
using MediatR;

namespace CalculadoraAntecipacaoRecebiveis.Features.Arquivos;
public static class ObterConteudoArquivo
{
    public class Command : IRequest<Result<Response>>
    {
        public string BlobContainer { get; set; }
        public string BlobName { get; set; }
    }

    public class Response
    {
        public string Conteudo { get; set; }
    }

    public class Handler(DownloadService downloadService) : BaseHandler<Command, Result<Response>>
    {
        private readonly DownloadService _downloadService = downloadService;

        public override async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            string containerName = request.BlobContainer;
            string blobName = request.BlobName;

            if (!await _downloadService.ExistsAsync(containerName, blobName))
            {
                AdicionarErro("Arquivo e/ou Container não encontrada");
                return Error<Response>(HttpStatusCode.NotFound);
            }

            using (var stream = await _downloadService.DownloadAsync(containerName, blobName, cancellationToken))
            {
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string conteudo = await reader.ReadToEndAsync();
                    var response = new Response { Conteudo = conteudo };

                    return Success(response);
                }
            }
        }
    }
}
