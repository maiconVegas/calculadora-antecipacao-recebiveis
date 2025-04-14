using System.Net;
using CalculadoraAntecipacaoRecebiveis.Core.Controllers;
using CalculadoraAntecipacaoRecebiveis.Core.Paginator;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CalculadoraAntecipacaoRecebiveis.Features.Arquivos;

[Route("arquivo")]
[ApiController]
public class ArquivoController : MainController
{
    private readonly IMediator _mediator;

    public ArquivoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // Aqui é mais envolvido para implementar nome do container junto com o arquivo, a prática mesmo é 
    // manipular somente os nomes dos arquivos pois container pertence ao projeto
    [HttpGet("{containerName}/{blobName}")]
    [ProducesResponseType(typeof(ObterConteudoArquivo.Response), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetContentArquivoAsync(string containerName, string blobName)
    {
        var response = await _mediator.Send(new ObterConteudoArquivo.Command
        {
            BlobContainer = containerName,
            BlobName = blobName
        });
        // Essa parte de ler o conteudo é apenas para testar se realmente esta lendo.

        return CustomResponse(response);
    }

    [HttpDelete("{blobName}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteArquivoAsync(string blobName)
    {
        var response = await _mediator.Send(new DeletarArquivo.Command
        {
            blobName = blobName
        });

        return CustomResponse(response);
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(SubirArquivo.Response), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UploadArquivoAsync(SubirArquivo.Command command)
    {
        var result = await _mediator.Send(command);
        return CustomResponse(result);
    }

    [HttpGet("download/{blobName}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetArquivoAsync(string blobName)
    {
        var response = await _mediator.Send(new BaixarArquivo.Command
        {
            NomeArquivo = blobName
        });
        if (response.Success)
        {
            var fileResult = response.Value.Arquivo;
            return File(fileResult.FileContents, fileResult.ContentType, fileResult.FileDownloadName);
        }
        return CustomResponse(response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(QueryResponse<ListarArquivos.Response>), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetListArquivoAsync(string pesquisa, string orderBy, OrderDirection direction, int? page, int? pageSize)
    {
        var result = await _mediator.Send(new ListarArquivos.Command
        {
            Pesquisa = pesquisa,
            Direction = direction,
            OrderBy = orderBy,
            Page = page,
            PageSize = pageSize
        });

        return CustomResponse(result);
    }
}