using System.Globalization;
using CsvHelper.Configuration;

namespace CalculadoraAntecipacaoRecebiveis.Infrastructure.Extensions.CsvHelper
{
    public static class CsvConfig
    {
        public static readonly CsvConfiguration Default = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            DetectDelimiter = true
        };
    }
}
