using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CalculadoraAntecipacaoRecebiveis.Migrations
{
    /// <inheritdoc />
    public partial class projetoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Simulados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValorTotal = table.Column<double>(type: "float", nullable: false),
                    ValorTotalComTaxa = table.Column<double>(type: "float", nullable: false),
                    CNPJCedente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataVencimento = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxaDiaria = table.Column<double>(type: "float", nullable: false),
                    EnderecoCsvBlob = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateTimeSimulado = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulados", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Simulados");
        }
    }
}
