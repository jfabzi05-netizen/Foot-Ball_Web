using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ProyectoWebFutbol.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "FutbolWeb");

            migrationBuilder.CreateTable(
                name: "Equipo",
                schema: "FutbolWeb",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Torneo",
                schema: "FutbolWeb",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Torneo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RondaTorneo",
                schema: "FutbolWeb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    TorneoId = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RondaTorneo", x => new { x.Id, x.TorneoId });
                    table.ForeignKey(
                        name: "FK_RondaTorneo_Torneo_TorneoId",
                        column: x => x.TorneoId,
                        principalSchema: "FutbolWeb",
                        principalTable: "Torneo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partido",
                schema: "FutbolWeb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TorneoId = table.Column<string>(type: "text", nullable: false),
                    RondaTorneoId = table.Column<int>(type: "integer", nullable: false),
                    Jornada = table.Column<string>(type: "text", nullable: false),
                    LocalId = table.Column<string>(type: "text", nullable: false),
                    ScoreLocal = table.Column<int>(type: "integer", nullable: true),
                    PenalesLocal = table.Column<int>(type: "integer", nullable: true),
                    VisitaId = table.Column<string>(type: "text", nullable: false),
                    ScoreVisita = table.Column<int>(type: "integer", nullable: true),
                    PenalesVisita = table.Column<int>(type: "integer", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partido_Equipo_LocalId",
                        column: x => x.LocalId,
                        principalSchema: "FutbolWeb",
                        principalTable: "Equipo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partido_Equipo_VisitaId",
                        column: x => x.VisitaId,
                        principalSchema: "FutbolWeb",
                        principalTable: "Equipo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partido_RondaTorneo_RondaTorneoId_TorneoId",
                        columns: x => new { x.RondaTorneoId, x.TorneoId },
                        principalSchema: "FutbolWeb",
                        principalTable: "RondaTorneo",
                        principalColumns: new[] { "Id", "TorneoId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partido_Torneo_TorneoId",
                        column: x => x.TorneoId,
                        principalSchema: "FutbolWeb",
                        principalTable: "Torneo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Partido_LocalId",
                schema: "FutbolWeb",
                table: "Partido",
                column: "LocalId");

            migrationBuilder.CreateIndex(
                name: "IX_Partido_RondaTorneoId_TorneoId",
                schema: "FutbolWeb",
                table: "Partido",
                columns: new[] { "RondaTorneoId", "TorneoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Partido_TorneoId",
                schema: "FutbolWeb",
                table: "Partido",
                column: "TorneoId");

            migrationBuilder.CreateIndex(
                name: "IX_Partido_VisitaId",
                schema: "FutbolWeb",
                table: "Partido",
                column: "VisitaId");

            migrationBuilder.CreateIndex(
                name: "IX_RondaTorneo_TorneoId",
                schema: "FutbolWeb",
                table: "RondaTorneo",
                column: "TorneoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Partido",
                schema: "FutbolWeb");

            migrationBuilder.DropTable(
                name: "Equipo",
                schema: "FutbolWeb");

            migrationBuilder.DropTable(
                name: "RondaTorneo",
                schema: "FutbolWeb");

            migrationBuilder.DropTable(
                name: "Torneo",
                schema: "FutbolWeb");
        }
    }
}
