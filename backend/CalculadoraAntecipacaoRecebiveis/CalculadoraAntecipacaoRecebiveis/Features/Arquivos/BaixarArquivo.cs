using System.Net;
using System.Text;
using CalculadoraAntecipacaoRecebiveis.Infrastructure.Integrations.BlobStorage;
using CalculadoraAntecipacaoRecebiveis.Core.Messaging;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CalculadoraAntecipacaoRecebiveis.Features.Arquivos;
public static class BaixarArquivo
{
    public class Command : IRequest<Result<Response>>
    {
        public string NomeArquivo { get; set; }
    }
    public class Response
    {
        public FileContentResult Arquivo { get; set; }
    }
    public class Handler(DownloadService downloadService) : BaseHandler<Command, Result<Response>>
    {
        private readonly string _container = "testcontainer";
        private readonly DownloadService _downloadService = downloadService;

        public override async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (!await _downloadService.ExistsAsync(_container, request.NomeArquivo, cancellationToken))
            {
                AdicionarErro("Arquivo não encontrada");
                return Error<Response>(HttpStatusCode.NotFound);
            }

            using (var stream = await _downloadService.DownloadAsync(_container, request.NomeArquivo, cancellationToken))
            {
                var content = stream.ToArray();
                var response = new Response
                {
                    Arquivo = new FileContentResult(content, "text/csv")
                    {
                        FileDownloadName = request.NomeArquivo
                    }
                };

                return Success(response);
            }
        }
    }
}
