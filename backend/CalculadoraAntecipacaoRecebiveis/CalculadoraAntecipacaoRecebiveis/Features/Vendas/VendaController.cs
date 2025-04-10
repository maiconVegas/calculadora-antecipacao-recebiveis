using System.Net;
using CalculadoraAntecipacaoRecebiveis.Core.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CalculadoraAntecipacaoRecebiveis.Features.Vendas;

[Route("vendas")]
[ApiController]
public class VendaController : MainController
{
    private readonly IMediator _mediator;

    public VendaController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("upload")]
    [ProducesResponseType(typeof(ObterResumoVenda.Response), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UploadArquivoAsync(ObterResumoVenda.Command command)
    {
        var result = await _mediator.Send(command);
        return CustomResponse(result);
    }
}
