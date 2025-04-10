using CalculadoraAntecipacaoRecebiveis.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalculadoraAntecipacaoRecebiveis.Infrastructure.Data.Configuration;

public class SimuladoConfiguration : IEntityTypeConfiguration<Simulado>
{
    public void Configure(EntityTypeBuilder<Simulado> builder)
    {
        builder.HasKey(c => c.Id);
    }
}
