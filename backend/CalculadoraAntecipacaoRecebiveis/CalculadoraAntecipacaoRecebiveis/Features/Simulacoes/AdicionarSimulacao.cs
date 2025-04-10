using System.Text.RegularExpressions;
using CalculadoraAntecipacaoRecebiveis.Core.Entities;
using CalculadoraAntecipacaoRecebiveis.Infrastructure.Data;
using CalculadoraAntecipacaoRecebiveis.Infrastructure.Integrations.BlobStorage;
using AutoMapper;
using CalculadoraAntecipacaoRecebiveis.Core.Messaging;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CalculadoraAntecipacaoRecebiveis.Features.Simulacoes;

public static class AdicionarSimulacao
{
    public class Command : BaseRequest<Result<Response>>
    {
        public IFormFile Arquivo { get; set; }
        public double ValorTotal { get; set; }
        public string CNPJCedente { get; set; }
        public string DataVencimento { get; set; }
        public int DiasRestantes { get; set; }
        public double TaxaDiaria { get; set; }
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
            RuleFor(p => p.ValorTotal)
                .GreaterThanOrEqualTo(0).WithMessage("possui valor total negativo.");

            RuleFor(p => p.CNPJCedente)
                .NotEmpty().WithMessage(" possui um cnpj cedente vázio.")
                .Must(EhValidoCNPJ).WithMessage("possui um CNPJ inválido.");

            RuleFor(p => p.DataVencimento)
                .NotEmpty().WithMessage(" possui data vencimento vazio")
                .Must(EhValidoData).WithMessage(" possui data vencimento inválido");

            When(p => EhValidoData(p.DataVencimento), () =>
            {
                RuleFor(p => p.DataVencimento)
                    .Must(EhDentroDaValidade).WithMessage("Possui data já vencida")
                    .Must(EhMaiorDoQuePrazoMinimo).WithMessage(" possui data vencimento menor que 30 dias");
            });

            RuleFor(x => x.DiasRestantes)
                .NotEmpty().WithMessage(" possui dias restantes vazio")
                .GreaterThanOrEqualTo(0).WithMessage("Os Dias Restantes não podem ser negativos.");

            RuleFor(x => x.TaxaDiaria)
                .NotEmpty().WithMessage(" possui taxa diaria vazio")
                .GreaterThanOrEqualTo(0).WithMessage("A Taxa Diária não pode ser negativa.");
        }

        private bool EhMaiorDoQuePrazoMinimo(string data)
        {
            var minimo = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            return DateOnly.FromDateTime(DateTime.Parse(data)) >= minimo;
        }

        private bool EhDentroDaValidade(string data)
            => DateOnly.FromDateTime(DateTime.Now) < DateOnly.FromDateTime(DateTime.Parse(data));

        private bool EhValidoData(string data) => DateOnly.TryParse(data, out DateOnly dateOnly);

        private bool EhValidoCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj)) return false;

            cnpj = Regex.Replace(cnpj, "[^0-9]", "");
            if (cnpj.Length != 14) return false;

            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            tempCnpj += digito1;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            return cnpj.EndsWith(digito1.ToString() + digito2.ToString());
        }
    }

    public class Response
    {
        public int Id { get; set; }
        public double ValorTotal { get; set; }
        public double ValorTotalComTaxa { get; set; }
        public string CNPJCedente { get; set; }
        public string DataVencimento { get; set; }
        public double TaxaDiaria { get; set; }
        public string EnderecoCsvBlob { get; set; }
        public string DateTimeSimulado { get; set; }
    }

    public class Handler(UploadService uploadService, ApplicationDbContext dbContext, IMapper mapper)
        : BaseHandler<Command, Result<Response>>
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly string _container = "simuladocontainer";
        private readonly UploadService _uploadService = uploadService;
        private readonly IMapper _mapper = mapper;

        public override async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {

            if (!request.EhValido())
            {
                AdicionarErros(request.ValidationResult);
                return Error<Response>();
            }

            string endereco = string.Empty;
            if (request.Arquivo is not null)
            {
                await using var stream = request.Arquivo.OpenReadStream();
                var palavra = Guid.NewGuid().ToString("N");
                endereco = await _uploadService.UploadAsync(_container, palavra + ".csv", stream, cancellationToken);
            }

            var valorTotal = request.ValorTotal;
            var valorTotalComTaxa = Math.Round(valorTotal - valorTotal * (request.TaxaDiaria / 100 * request.DiasRestantes), 2);
            var cnpjCedente = Regex.Replace(request.CNPJCedente, "[^0-9]", "");
            var dataVencimento = DateOnly.Parse(request.DataVencimento).ToString("dd/MM/yyyy");
            var taxaDiaria = request.TaxaDiaria;
            var enderecoCsvBlob = endereco;
            var dateTimeSimulado = DateTime.Now.ToString("dd/MM/yyyy - HH:mm");

            var simulado = new Simulado
            {
                ValorTotal = valorTotal,
                ValorTotalComTaxa = valorTotalComTaxa,
                CNPJCedente = cnpjCedente,
                DataVencimento = dataVencimento,
                TaxaDiaria = taxaDiaria,
                EnderecoCsvBlob = enderecoCsvBlob,
                DateTimeSimulado = dateTimeSimulado
            };

            //await _dbContext.Simulados.AddAsync(simulado, cancellationToken);
            await _dbContext.Database.ExecuteSqlRawAsync(
                "INSERT INTO Simulados (ValorTotal, ValorTotalComTaxa, CNPJCedente, DataVencimento, TaxaDiaria, EnderecoCsvBlob, DateTimeSimulado) " +
                "VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6)",
                [valorTotal, valorTotalComTaxa, cnpjCedente, dataVencimento, taxaDiaria, enderecoCsvBlob, dateTimeSimulado],
                cancellationToken);

            await _dbContext.SaveChangesAsync(cancellationToken);

            var response = _mapper.Map<Response>(simulado);

            return Success(response);
        }
    }

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Simulado, Response>();
        }
    }
}
