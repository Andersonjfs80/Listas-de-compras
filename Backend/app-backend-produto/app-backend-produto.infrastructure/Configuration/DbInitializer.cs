using app_backend_produto.domain.Models;
using app_backend_produto.domain.Types;
using System;
using System.Linq;

namespace app_backend_produto.infrastructure.Configuration;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        // Verifica se já existem produtos para evitar duplicidade
        if (context.Produtos.Any()) return;

        var usuarioIdDefault = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var dataAtual = DateTime.Now;

        // 1. Unidades de Medida
        var un = new UnidadeMedidaModel { Id = Guid.NewGuid(), Sigla = "UN", Descricao = "Unidade", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
        var kg = new UnidadeMedidaModel { Id = Guid.NewGuid(), Sigla = "KG", Descricao = "Quilograma", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
        context.UnidadesMedida.AddRange(un, kg);

        // 2. Tipos de Preço
        var normal = new TipoPrecoModel { Id = Guid.NewGuid(), Nome = "Normal", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
        var oferta = new TipoPrecoModel { Id = Guid.NewGuid(), Nome = "Oferta", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
        context.TiposPreco.AddRange(normal, oferta);

        // 3. Categorias
        var catAlimentos = new CategoriaModel { Id = Guid.NewGuid(), Nome = "Alimentos", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
        var catLimpeza = new CategoriaModel { Id = Guid.NewGuid(), Nome = "Limpeza", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
        context.Categorias.AddRange(catAlimentos, catLimpeza);

        context.SaveChanges();

        // 4. Produtos (Gerando 10)
        string[] nomesProdutos = { 
            "Arroz 5kg", "Feijão Preto 1kg", "Macarrão Espaguete", "Óleo de Soja", "Açúcar Refinado",
            "Detergente Líquido", "Sabão em Pó 1kg", "Amaciante de Roupas", "Desinfetante Pinho", "Esponja de Aço"
        };

        for (int i = 0; i < 10; i++)
        {
            var produto = new ProdutoModel
            {
                Id = Guid.NewGuid(),
                Nome = nomesProdutos[i],
                NomeCurto = nomesProdutos[i].Split(' ')[0],
                Ativo = true,
                DataCadastro = dataAtual,
                CategoriaId = i < 5 ? catAlimentos.Id : catLimpeza.Id,
                UsuarioId = usuarioIdDefault
            };

            // Imagem Base64 fictícia
            produto.Imagens.Add(new ProdutoImagemModel
            {
                Id = Guid.NewGuid(),
                Conteudo = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==",
                Tipo = TipoImagem.Principal,
                Ativo = true,
                DataCadastro = dataAtual,
                UsuarioId = usuarioIdDefault,
                Favorito = i % 2 == 0
            });

            // Preço
            produto.Precos.Add(new PrecoModel
            {
                Id = Guid.NewGuid(),
                Valor = 5.90m + i,
                Principal = true,
                Ativo = true,
                DataCadastro = dataAtual,
                TipoPrecoId = i % 3 == 0 ? oferta.Id : normal.Id,
                UsuarioId = usuarioIdDefault
            });

            // Código de Barras
            produto.Codigos.Add(new CodigoProdutoModel
            {
                Id = Guid.NewGuid(),
                CodigoProduto = $"PRD-00{i+1}",
                CodigoBarras = $"78912345678{i}",
                Ativo = true,
                DataCadastro = dataAtual,
                UnidadeMedidaId = i < 5 ? un.Id : kg.Id,
                UsuarioId = usuarioIdDefault
            });

            context.Produtos.Add(produto);
        }

        context.SaveChanges();
    }
}
