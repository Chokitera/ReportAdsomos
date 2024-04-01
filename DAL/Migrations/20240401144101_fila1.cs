using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class fila1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agentes_Fila_FilaVirtualId",
                table: "Agentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Agentes_Observacoes_ObservacoesVirtualId",
                table: "Agentes");

            migrationBuilder.DropIndex(
                name: "IX_Agentes_FilaVirtualId",
                table: "Agentes");

            migrationBuilder.DropColumn(
                name: "FilaVirtualId",
                table: "Agentes");

            migrationBuilder.RenameColumn(
                name: "ObservacoesVirtualId",
                table: "Agentes",
                newName: "FilaId");

            migrationBuilder.RenameIndex(
                name: "IX_Agentes_ObservacoesVirtualId",
                table: "Agentes",
                newName: "IX_Agentes_FilaId");

            migrationBuilder.AddColumn<int>(
                name: "AgenteId",
                table: "Observacoes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Observacoes_AgenteId",
                table: "Observacoes",
                column: "AgenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agentes_Fila_FilaId",
                table: "Agentes",
                column: "FilaId",
                principalTable: "Fila",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Observacoes_Agentes_AgenteId",
                table: "Observacoes",
                column: "AgenteId",
                principalTable: "Agentes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agentes_Fila_FilaId",
                table: "Agentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Observacoes_Agentes_AgenteId",
                table: "Observacoes");

            migrationBuilder.DropIndex(
                name: "IX_Observacoes_AgenteId",
                table: "Observacoes");

            migrationBuilder.DropColumn(
                name: "AgenteId",
                table: "Observacoes");

            migrationBuilder.RenameColumn(
                name: "FilaId",
                table: "Agentes",
                newName: "ObservacoesVirtualId");

            migrationBuilder.RenameIndex(
                name: "IX_Agentes_FilaId",
                table: "Agentes",
                newName: "IX_Agentes_ObservacoesVirtualId");

            migrationBuilder.AddColumn<int>(
                name: "FilaVirtualId",
                table: "Agentes",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Agentes_FilaVirtualId",
                table: "Agentes",
                column: "FilaVirtualId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agentes_Fila_FilaVirtualId",
                table: "Agentes",
                column: "FilaVirtualId",
                principalTable: "Fila",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Agentes_Observacoes_ObservacoesVirtualId",
                table: "Agentes",
                column: "ObservacoesVirtualId",
                principalTable: "Observacoes",
                principalColumn: "Id");
        }
    }
}
