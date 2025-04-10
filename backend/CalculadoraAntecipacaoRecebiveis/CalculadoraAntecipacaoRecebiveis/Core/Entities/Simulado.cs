namespace CalculadoraAntecipacaoRecebiveis.Core.Entities;

public class Simulado
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
