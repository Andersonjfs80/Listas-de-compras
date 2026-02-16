using app_backend_produto.domain.Models;
using app_backend_produto.domain.Types;
using System;
using System.Linq;

namespace app_backend_produto.infrastructure.Configuration;

public static class DbInitializer
{
    public static void Seed(AppDbContext context)
    {
        var usuarioIdDefault = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var dataAtual = DateTime.Now;



        // --- 1. Unidades de Medida ---
        var un = context.UnidadesMedida.FirstOrDefault(u => u.Sigla == "UN");
        if (un == null)
        {
            un = new UnidadeMedidaModel { Id = Guid.NewGuid(), Sigla = "UN", Descricao = "Unidade", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
            context.UnidadesMedida.Add(un);
        }

        var kg = context.UnidadesMedida.FirstOrDefault(u => u.Sigla == "KG");
        if (kg == null)
        {
            kg = new UnidadeMedidaModel { Id = Guid.NewGuid(), Sigla = "KG", Descricao = "Quilograma", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
            context.UnidadesMedida.Add(kg);
        }

        // --- 2. Tipos de Preço ---
        var normal = context.TiposPreco.FirstOrDefault(t => t.Nome == "Normal");
        if (normal == null)
        {
            normal = new TipoPrecoModel { Id = Guid.NewGuid(), Nome = "Normal", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
            context.TiposPreco.Add(normal);
        }

        var oferta = context.TiposPreco.FirstOrDefault(t => t.Nome == "Oferta");
        if (oferta == null)
        {
            oferta = new TipoPrecoModel { Id = Guid.NewGuid(), Nome = "Oferta", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
            context.TiposPreco.Add(oferta);
        }

        // --- 3. Categorias ---
        var catAlimentos = context.Categorias.FirstOrDefault(c => c.Nome == "Alimentos");
        if (catAlimentos == null)
        {
            catAlimentos = new CategoriaModel { Id = Guid.NewGuid(), Nome = "Alimentos", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
            context.Categorias.Add(catAlimentos);
        }

        var catLimpeza = context.Categorias.FirstOrDefault(c => c.Nome == "Limpeza");
        if (catLimpeza == null)
        {
            catLimpeza = new CategoriaModel { Id = Guid.NewGuid(), Nome = "Limpeza", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
            context.Categorias.Add(catLimpeza);
        }

        // --- 4. Tipos de Estabelecimento (NOVO) ---
        var supermercado = context.TiposEstabelecimento.FirstOrDefault(t => t.Nome == "Supermercado");
        if (supermercado == null)
        {
            supermercado = new TipoEstabelecimentoModel { Id = Guid.NewGuid(), Nome = "Supermercado", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
            context.TiposEstabelecimento.Add(supermercado);
        }

        var drogaria = context.TiposEstabelecimento.FirstOrDefault(t => t.Nome == "Drogaria");
        if (drogaria == null)
        {
            drogaria = new TipoEstabelecimentoModel { Id = Guid.NewGuid(), Nome = "Drogaria", Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault };
            context.TiposEstabelecimento.Add(drogaria);
        }

        context.SaveChanges(); // Salva para garantir IDs para Fornecedores

        // --- 5. Fornecedores (NOVO) ---
        var sonda = context.Fornecedores.FirstOrDefault(f => f.NomeFantasia == "Sonda Supermercados");
        if (sonda == null)
        {
            sonda = new FornecedorModel 
            { 
                Id = Guid.NewGuid(), 
                Nome = "Sonda Supermercados Exportação e Importação SA", 
                NomeFantasia = "Sonda Supermercados", 
                TipoEstabelecimentoId = supermercado.Id,
                Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault 
            };
            context.Fornecedores.Add(sonda);
        }

        var bergamini = context.Fornecedores.FirstOrDefault(f => f.NomeFantasia == "Bergamini Supermercado");
        if (bergamini == null)
        {
            bergamini = new FornecedorModel 
            { 
                Id = Guid.NewGuid(), 
                Nome = "Bergamini Supermercados Ltda", 
                NomeFantasia = "Bergamini Supermercado", 
                TipoEstabelecimentoId = supermercado.Id,
                Ativo = true, DataCadastro = dataAtual, UsuarioId = usuarioIdDefault 
            };
            context.Fornecedores.Add(bergamini);
        }

        context.SaveChanges();

        // --- 6. Produtos ---
        if (context.Produtos.Any()) return;

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
                // CategoriaId removido - Agora é N-N
                UsuarioId = usuarioIdDefault
            };

            // Adicionar Categoria Principal
            var catId = i < 5 ? catAlimentos.Id : catLimpeza.Id;
            produto.ProdutoCategorias.Add(new ProdutoCategoriaModel
            {
                // Id composto ou Guid se tiver PK, mas ProductId será setado pelo EF automagicamente ao adicionar na lista do produto
                ProdutoId = produto.Id, 
                CategoriaId = catId,
                Tipo = app_backend_produto.domain.Enums.TipoCategoria.Principal,
                Ativo = true,
                DataCadastro = dataAtual,
                UsuarioId = usuarioIdDefault
            });

            // Imagem
            produto.ProdutoImagens.Add(new ProdutoImagemModel
            {
                Id = Guid.NewGuid(),
                Conteudo = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==",
                Tipo = TipoImagem.Principal,
                Ativo = true,
                DataCadastro = dataAtual,
                UsuarioId = usuarioIdDefault,
                Favorito = i % 2 == 0
            });

            // Preços
            produto.ProdutoPrecos.Add(new ProdutoPrecoModel
            {
                Id = Guid.NewGuid(),
                Valor = 5.90m + i,
                Principal = true,
                Ativo = true,
                DataCadastro = dataAtual,
                TipoPrecoId = i % 3 == 0 ? oferta.Id : normal.Id,
                UsuarioId = usuarioIdDefault
            });

            // Código de Barras (Vincula Fornecedor aqui)
            produto.ProdutoCodigos.Add(new ProdutoCodigoModel
            {
                Id = Guid.NewGuid(),
                CodigoProduto = $"PRD-00{i+1}",
                CodigoBarras = $"78912345678{i}",
                Ativo = true,
                DataCadastro = dataAtual,
                UnidadeMedidaId = i < 5 ? un.Id : kg.Id,
                UsuarioId = usuarioIdDefault,
                FornecedorId = (i % 2 == 0 && sonda != null) ? sonda.Id : (bergamini != null ? bergamini.Id : sonda!.Id)
            });

            context.Produtos.Add(produto);
        }

        context.SaveChanges();
    }
}
