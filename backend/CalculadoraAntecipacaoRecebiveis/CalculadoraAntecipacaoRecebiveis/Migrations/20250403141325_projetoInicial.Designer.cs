﻿// <auto-generated />
using CalculadoraAntecipacaoRecebiveis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CalculadoraAntecipacaoRecebiveis.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250403141325_projetoInicial")]
    partial class projetoInicial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CalculadoraAntecipacaoRecebiveis.Core.Entities.Simulado", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CNPJCedente")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DataVencimento")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DateTimeSimulado")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("EnderecoCsvBlob")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TaxaDiaria")
                        .HasColumnType("float");

                    b.Property<double>("ValorTotal")
                        .HasColumnType("float");

                    b.Property<double>("ValorTotalComTaxa")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Simulados");
                });
#pragma warning restore 612, 618
        }
    }
}
