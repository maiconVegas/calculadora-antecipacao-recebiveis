using CalculadoraAntecipacaoRecebiveis.Infrastructure.Integrations.BlobStorage;
using CalculadoraAntecipacaoRecebiveis.Core.Messaging;
using FluentValidation;

namespace CalculadoraAntecipacaoRecebiveis.Features.Arquivos;
public static class SubirArquivo
{
    public class Command : BaseRequest<Result<Response>>
    {
        public IFormFile Arquivo { get; set; }

        public override bool EhValido()
        {
            ValidationResult = new CommandValidator().Validate(this);
            return ValidationResult.IsValid;
        }
    }
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(c => c.Arquivo)
                .NotEmpty()
                .WithMessage("O arquivo não foi carregado");
        }
    }
    public class Response
    {
        public string Endereco { get; set; }
    }
    public class Handler(UploadService uploadService) : BaseHandler<Command, Result<Response>>
    {
        private readonly string _container = "testcontainer";
        private readonly UploadService _uploadService = uploadService;

        public override async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (!request.EhValido())
            {
                AdicionarErros(request.ValidationResult);
                return Error<Response>();
            }

            await using var stream = request.Arquivo.OpenReadStream();
            var palavra = Guid.NewGuid().ToString("N");
            var endereco = await _uploadService.UploadAsync(_container, palavra + ".csv", stream, cancellationToken);
            var response = new Response { Endereco = endereco };

            return Success(response);
        }
    }
}
