using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hinario.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarRepertorio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "repertorios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    data = table.Column<DateOnly>(type: "date", nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repertorios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "repertorio_itens",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    repertorio_id = table.Column<int>(type: "integer", nullable: false),
                    hino_id = table.Column<int>(type: "integer", nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_repertorio_itens", x => x.id);
                    table.ForeignKey(
                        name: "FK_repertorio_itens_hinos_hino_id",
                        column: x => x.hino_id,
                        principalTable: "hinos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_repertorio_itens_repertorios_repertorio_id",
                        column: x => x.repertorio_id,
                        principalTable: "repertorios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_repertorio_itens_hino_id",
                table: "repertorio_itens",
                column: "hino_id");

            migrationBuilder.CreateIndex(
                name: "IX_repertorio_itens_repertorio_id_ordem",
                table: "repertorio_itens",
                columns: new[] { "repertorio_id", "ordem" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "repertorio_itens");

            migrationBuilder.DropTable(
                name: "repertorios");

        }
    }
}
