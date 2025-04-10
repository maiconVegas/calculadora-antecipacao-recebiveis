namespace CalculadoraAntecipacaoRecebiveis.Core.Entities
{
    public class Produto
    {
        public string Nome { get; set; }
        public int Quantidade { get; set; }
        public double ValorUnitario { get; set; }
        public double ValorTotalProduto { get; set; }
        public string CedenteCNPJ { get; set; }
        public string DataVencimento { get; set; }
    }
}
