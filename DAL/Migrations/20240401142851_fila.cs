using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class fila : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fila",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    Setor = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Observacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraInicial = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    HoraFinal = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id1", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    FilaVirtualId = table.Column<int>(type: "integer", nullable: true),
                    ObservacoesVirtualId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("id2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agentes_Fila_FilaVirtualId",
                        column: x => x.FilaVirtualId,
                        principalTable: "Fila",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Agentes_Observacoes_ObservacoesVirtualId",
                        column: x => x.ObservacoesVirtualId,
                        principalTable: "Observacoes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agentes_FilaVirtualId",
                table: "Agentes",
                column: "FilaVirtualId");

            migrationBuilder.CreateIndex(
                name: "IX_Agentes_ObservacoesVirtualId",
                table: "Agentes",
                column: "ObservacoesVirtualId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Agentes");

            migrationBuilder.DropTable(
                name: "Fila");

            migrationBuilder.DropTable(
                name: "Observacoes");
        }
    }
}
