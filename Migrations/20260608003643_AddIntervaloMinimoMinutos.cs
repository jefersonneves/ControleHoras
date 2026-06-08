using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleHoras.Migrations
{
    /// <inheritdoc />
    public partial class AddIntervaloMinimoMinutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IntervaloMinimoMinutos",
                table: "Configuracoes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntervaloMinimoMinutos",
                table: "Configuracoes");
        }
    }
}
