using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleHoras.Migrations
{
    /// <inheritdoc />
    public partial class CriarConfiguracoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuracoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JornadaDiariaHoras = table.Column<double>(type: "REAL", nullable: false),
                    ToleranciaMinutos = table.Column<int>(type: "INTEGER", nullable: false),
                    BancoHorasMeses = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuracoes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuracoes");
        }
    }
}
