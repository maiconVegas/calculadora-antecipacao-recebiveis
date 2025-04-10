using System.Net;
using CalculadoraAntecipacaoRecebiveis.Infrastructure.Integrations.BlobStorage;
using CalculadoraAntecipacaoRecebiveis.Core.Messaging;
using MediatR;

namespace CalculadoraAntecipacaoRecebiveis.Features.Arquivos;
public static class DeletarArquivo
{
    public class Command : IRequest<Result>
    {
        public string blobName { get; set; }
    }
    // Por mais que penso que o result possa não estar para lidar com retorno do item.
    // é melhor deixar ele pelo motivo de ele existir para lidar com os possíveis erros que podem surgir
    // além de manter um retorno consistente entre diferentes handlers.
    public class Handler(UploadService uploadService) : BaseHandler<Command, Result>
    {
        private readonly string _container = "testcontainer";
        private readonly UploadService _uploadService = uploadService;

        public override async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            if (!await _uploadService.ExistsAsync(_container, request.blobName, cancellationToken))
            {
                AdicionarErro("Arquivo não encontrada");
                return Error(HttpStatusCode.NotFound);
            }

            await _uploadService.DeleteAsync(_container, request.blobName);
            return Success();
        }
    }
}
