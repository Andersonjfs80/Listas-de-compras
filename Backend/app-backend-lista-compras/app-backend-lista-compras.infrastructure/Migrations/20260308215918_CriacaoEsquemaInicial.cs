using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_backend_lista_compras.infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoEsquemaInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListasCompras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasCompras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ofertas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NomeProduto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    PrecoAtual = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PrecoAnterior = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Imagem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CategoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoriaNome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ofertas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItensLista",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProdutoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NomeProduto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NomeCurto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Quantidade = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    UnidadeMedida = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QuantidadeConversao = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    UnidadeMedidaConversao = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PrecoCompra = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PrecoVenda = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Marcado = table.Column<bool>(type: "bit", nullable: false),
                    CategoriaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CategoriaNome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Imagem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensLista", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensLista_ListasCompras_ListaId",
                        column: x => x.ListaId,
                        principalTable: "ListasCompras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensLista_ListaId",
                table: "ItensLista",
                column: "ListaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensLista");

            migrationBuilder.DropTable(
                name: "Ofertas");

            migrationBuilder.DropTable(
                name: "ListasCompras");
        }
    }
}
