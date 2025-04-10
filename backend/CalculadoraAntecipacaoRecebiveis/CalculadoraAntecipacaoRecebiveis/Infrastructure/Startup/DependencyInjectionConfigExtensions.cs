using CalculadoraAntecipacaoRecebiveis.Infrastructure.Data;
using CalculadoraAntecipacaoRecebiveis.Infrastructure.Extensions.CsvHelper;
using CalculadoraAntecipacaoRecebiveis.Infrastructure.Integrations.BlobStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CalculadoraAntecipacaoRecebiveis.Infrastructure.Startup
{
    public static class DependencyInjectionConfigExtensions
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddAutoMapper(AssemblyReference.Assembly);

            services.AddMediatR(c => c.RegisterServicesFromAssemblies(AssemblyReference.Assembly));

            services.Configure<BlobStorageConfiguration>(configuration.GetSection("AzureStorage"));

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("MinhaConexao")));

            services.AddScoped<BlobService>();
            services.AddScoped<DownloadService>();
            services.AddScoped<UploadService>();

            services.AddScoped<CsvService>();

        }
    }
}
