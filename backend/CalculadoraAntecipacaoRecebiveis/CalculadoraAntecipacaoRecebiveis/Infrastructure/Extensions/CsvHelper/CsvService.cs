using CalculadoraAntecipacaoRecebiveis.Core.Entities;
using CsvHelper;

namespace CalculadoraAntecipacaoRecebiveis.Infrastructure.Extensions.CsvHelper;

public class CsvService
{
    public async Task<List<Produto>> GetProdutosAsync(Stream arquivo, CancellationToken cancellationToken)
    {
        var produtos = new List<Produto>();
        using (var reader = new StreamReader(arquivo))
        using (var csv = new CsvReader(reader, CsvConfig.Default))
        {
            await foreach (var registro in csv.GetRecordsAsync<dynamic>(cancellationToken))
            {
                var produto = new Produto
                {
                    Nome = registro.produto,
                    Quantidade = Convert.ToInt32(registro.quantidade),
                    ValorUnitario = Convert.ToDouble(registro.valor),
                    ValorTotalProduto = Convert.ToDouble(registro.valor) * Convert.ToInt32(registro.quantidade),
                    CedenteCNPJ = registro.cedentecnpj,
                    DataVencimento = registro.datavencimento,
                };
                produtos.Add(produto);
            }
        }
        return produtos;
    }
}
