using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_backend_autenticacao.infrastructure.Migrations
{
    public partial class AddPasswordHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataAtualizacaoSenha",
                table: "Usuarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HistoricoSenhasJson",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataAtualizacaoSenha",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "HistoricoSenhasJson",
                table: "Usuarios");
        }
    }
}
