using System.Net;
using CalculadoraAntecipacaoRecebiveis.Core.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CalculadoraAntecipacaoRecebiveis.Features.Simulacoes;

[Route("Simulacao")]
[ApiController]
public class SimulacaoController : MainController
{
    private readonly IMediator _mediator;

    public SimulacaoController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(AdicionarSimulacao.Response), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UploadSimulacaoAsync(AdicionarSimulacao.Command command)
    {
        var result = await _mediator.Send(command);
        return CustomResponse(result);
    }
}
