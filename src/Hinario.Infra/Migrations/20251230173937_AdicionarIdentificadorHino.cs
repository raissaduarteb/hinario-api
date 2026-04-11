using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hinario.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarIdentificadorHino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hinos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    identificador = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    letra = table.Column<string>(type: "text", nullable: false),
                    titulo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hinos", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hinos_identificador",
                table: "hinos",
                column: "identificador",
                unique: true,
                filter: "\"identificador\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hinos");
        }
    }
}
