using CalculadoraAntecipacaoRecebiveis.Infrastructure.Integrations.BlobStorage;
using CalculadoraAntecipacaoRecebiveis.Core.Messaging;
using CalculadoraAntecipacaoRecebiveis.Core.Paginator;
using MediatR;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CalculadoraAntecipacaoRecebiveis.Features.Arquivos
{
    public static class ListarArquivos
    {
        public class Command : QueryParams, IRequest<Result<QueryResponse<Response>>>
        {
            public string Pesquisa { get; set; }
        }
        public class Response
        {
            public string NomeArquivo { get; set; }
        }
        public class Handler(BlobService blobService) : BaseHandler<Command, Result<QueryResponse<Response>>>
        {
            private readonly string _container = "testcontainer";
            private readonly BlobService _blobService = blobService;

            public override async Task<Result<QueryResponse<Response>>> Handle(Command request, CancellationToken cancellationToken)
            {
                var blobs = await _blobService.ListBlobsAsync(_container, cancellationToken);

                if (!string.IsNullOrEmpty(request.Pesquisa))
                {
                    blobs = blobs.Where(c => c.Contains(request.Pesquisa)).ToList();
                }

                var response = blobs.Select(nome => new Response { NomeArquivo = nome }).ToList();
                var query = response.AsQueryable().ApplyPagination(request, out var count);

                return Success(new QueryResponse<Response>
                {
                    Items = query.ToList(),
                    Count = count,
                    Page = request.Page.Value,
                    PageSize = request.PageSize
                });
            }
        }
    }
}
