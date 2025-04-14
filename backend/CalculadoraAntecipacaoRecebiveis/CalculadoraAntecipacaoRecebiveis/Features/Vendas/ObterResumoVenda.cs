using CalculadoraAntecipacaoRecebiveis.Core.Entities;
using FluentValidation;
using System.Text.RegularExpressions;
using System.Text;
using CalculadoraAntecipacaoRecebiveis.Infrastructure.Extensions.CsvHelper;
using FluentValidation.Results;
using CalculadoraAntecipacaoRecebiveis.Core.Messaging;

namespace CalculadoraAntecipacaoRecebiveis.Features.Vendas;

public static class ObterResumoVenda
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

    public class ProdutoValidator : AbstractValidator<Produto>
    {
        public ProdutoValidator()
        {

            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("O nome do produto não pode estar vazio.")
                .MinimumLength(3).WithMessage("O nome do produto deve ter pelo menos 3 caracteres.")
                .MaximumLength(100).WithMessage("O nome do produto deve ter no máximo 100 caracteres.");

            RuleFor(p => p.Quantidade)
                .GreaterThan(0).WithMessage(p => $"Produto {p.Nome} possui quantidade menor ou igual a zero.");

            RuleFor(p => p.ValorUnitario)
                .GreaterThan(0).WithMessage(p => $"Produto {p.Nome} possui valor unitário menor ou igual a zero.");

            RuleFor(p => p.ValorTotalProduto)
                .GreaterThanOrEqualTo(0).WithMessage(p => $"Produto {p.Nome} possui valor total negativo.")
                .Equal(p => p.Quantidade * p.ValorUnitario)
                .WithMessage(p => $"Produto {p.Nome} possui valor total incorreto.");

            RuleFor(p => p.CedenteCNPJ)
                .NotEmpty().WithMessage(p => $"Produto {p.Nome} possui um cnpj cedente vázio.")
                .Must(EhValidoCNPJ).WithMessage(p => $"Produto {p.Nome} possui um CNPJ inválido.");

            RuleFor(p => p.DataVencimento)
                .NotEmpty().WithMessage(p => $"Produto {p.Nome} possui data vencimento vazio")
                .Must(EhValidoData).WithMessage(p => $"Produto {p.Nome} possui data vencimento inválido");

            When(p => EhValidoData(p.DataVencimento), () =>
            {
                RuleFor(p => p.DataVencimento)
                    .Must(EhDentroDaValidade).WithMessage(p => $"Produto {p.Nome} possui data já vencida")
                    .Must(EhMaiorDoQuePrazoMinimo).WithMessage(p => $"Produto {p.Nome} possui data vencimento menor que 30 dias");
            });

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

    public class ListaProdutoValidator : AbstractValidator<List<Produto>>
    {
        public ListaProdutoValidator()
        {
            RuleFor(p => p)
                .Must(EhDataUnica).WithMessage("Produtos possuem datas de vencimentos diferentes.");

            RuleFor(p => p)
                .Must(EhCnpjUnica).WithMessage("Produtos possuemm CNPJ diferentes.");
        }

        private bool EhCnpjUnica(List<Produto> list)
        {
            var cnpjlist = list.Select(p => Regex.Replace(p.CedenteCNPJ, "[^0-9]", ""));
            return !(cnpjlist.Distinct().Count() > 1);
        }

        private bool EhDataUnica(List<Produto> list)
        {
            return !(list.Select(p => DateOnly.FromDateTime(DateTime.Parse(p.DataVencimento))).Distinct().Count() > 1);
        }
    }

    public class Response
    {
        public double ValorTotal { get; set; }
        public string CNPJCedente { get; set; }
        public string DataVencimento { get; set; }
        public int DiasRestantes { get; set; }
        public double TaxaDiaria { get; set; }
        public List<Produto> Produtos { get; set; }
    }

    public class Handler(CsvService csvService) : BaseHandler<Command, Result<Response>>
    {
        private readonly CsvService _csvService = csvService;

        public override async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (!request.EhValido())
            {
                AdicionarErros(request.ValidationResult);
                return Error<Response>();
            }

            var novoStream = await ModificarPrimeiraLinhaCsv(request.Arquivo, cancellationToken);
            if (novoStream is null)
            {
                AdicionarErro("Arquivo Csv não contém conteúdo e/ou nomes das colunas válidas");
                return Error<Response>();
            }

            var produtos = await _csvService.GetProdutosAsync(novoStream, cancellationToken);
            var erros = await ValidarProdutos(produtos, cancellationToken);

            if (erros.Any())
            {
                foreach (var erro in erros)
                {
                    AdicionarErros(erro);
                }
                return Error<Response>();
            }

            var erros2 = await new ListaProdutoValidator().ValidateAsync(produtos, cancellationToken);
            if (!erros2.IsValid)
            {
                AdicionarErros(erros2);
                return Error<Response>();
            }

            var valorTotal = produtos.Sum(p => p.ValorTotalProduto);
            var cnpjCedente = Regex.Replace(produtos.FirstOrDefault().CedenteCNPJ, "[^0-9]", "");
            var dataVencimento = DateOnly.Parse(produtos
                .FirstOrDefault().DataVencimento).ToString("dd/MM/yyyy");
            var diasRestantes = DateOnly.Parse(produtos.FirstOrDefault().DataVencimento).DayNumber -
                DateOnly.FromDateTime(DateTime.Now).DayNumber;
            var taxaDiaria = CalcularTaxaDiaria(diasRestantes);

            var response = new Response
            {
                Produtos = produtos,
                ValorTotal = valorTotal,
                CNPJCedente = cnpjCedente,
                DataVencimento = dataVencimento,
                DiasRestantes = diasRestantes,
                TaxaDiaria = taxaDiaria,
            };

            return Success(response);
        }

        private double CalcularTaxaDiaria(int diasRestantes) => diasRestantes switch
        {
            >= 30 and <= 60 => 0.033,
            > 60 and <= 90 => 0.028,
            > 90 and <= 120 => 0.025,
            > 120 and <= 180 => 0.023,
            > 180 and <= 365 => 0.022,
            > 365 => 0.020
        };

        private async Task<List<ValidationResult>> ValidarProdutos(List<Produto> produtos, CancellationToken cancellationToken)
        {
            var validator = new ProdutoValidator();

            var task = produtos.Select(p => validator.ValidateAsync(p, cancellationToken));
            var resultados = await Task.WhenAll(task);

            return resultados.Where(r => !r.IsValid).ToList();
        }

        private async Task<Stream> ModificarPrimeiraLinhaCsv(IFormFile arquivo, CancellationToken cancellationToken)
        {
            using var reader = new StreamReader(arquivo.OpenReadStream());

            string primeiraLinha = await reader.ReadLineAsync(cancellationToken);
            StringBuilder novoConteudo = new StringBuilder();

            if (!string.IsNullOrEmpty(primeiraLinha))
            {
                primeiraLinha = Regex.Replace(primeiraLinha, @"[()\s]", "").ToLower();
                primeiraLinha = Regex.Replace(primeiraLinha, @"\b(nomeproduto|produto|item|peça)\b", "produto", RegexOptions.IgnoreCase);
                primeiraLinha = Regex.Replace(primeiraLinha, @"\b(valor|preço|preco)\b", "valor", RegexOptions.IgnoreCase);
                primeiraLinha = Regex.Replace(primeiraLinha, @"\b(cnpjcedente|cedente)\b", "cedentecnpj", RegexOptions.IgnoreCase);
                primeiraLinha = Regex.Replace(primeiraLinha, @"\b(porção|qtd|n°)\b", "quantidade", RegexOptions.IgnoreCase);
                primeiraLinha = Regex.Replace(primeiraLinha, @"\b(datadevencimento|vencimentodata)\b", "datavencimento", RegexOptions.IgnoreCase);
            }
            else
            {
                return null;
            }

            if (!(primeiraLinha.Contains("produto") && primeiraLinha.Contains("valor") && primeiraLinha.Contains("cedentecnpj") && primeiraLinha.Contains("quantidade") && primeiraLinha.Contains("datavencimento")))
            {
                return null;
            }

            novoConteudo.AppendLine(primeiraLinha);

            while (!reader.EndOfStream)
            {
                string linha = await reader.ReadLineAsync(cancellationToken);
                novoConteudo.AppendLine(linha);
            }
            reader.Close();

            var outputStream = new MemoryStream(Encoding.UTF8.GetBytes(novoConteudo.ToString()));
            return outputStream;
        }

    }
}
