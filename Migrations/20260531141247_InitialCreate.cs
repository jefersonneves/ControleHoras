using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleHoras.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrosPonto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EntradaManha = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    SaidaManha = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    EntradaTarde = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    SaidaTarde = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    Observacao = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosPonto", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosPonto");
        }
    }
}
