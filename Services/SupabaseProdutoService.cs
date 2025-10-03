using AdmBeachApp.Models;
using Microsoft.Extensions.Logging;

namespace AdmBeachApp.Services
{
    public class SupabaseProdutoService : IProdutoService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly ILogger<SupabaseProdutoService> _logger;

        public SupabaseProdutoService(Supabase.Client supabaseClient, ILogger<SupabaseProdutoService> logger)
        {
            _supabaseClient = supabaseClient;
            _logger = logger;
        }

        public async Task<List<Produto>> GetProdutosAsync()
        {
            try
            {
                _logger.LogDebug("Buscando produtos ativos no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabaseProduto>()
                    .Where(p => p.Ativo == true)
                    .Get();

                return response.Models.Select(sp => sp.ToProduto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos no Supabase");
                return GetProdutosExemplo();
            }
        }

        public async Task<Produto?> GetProdutoByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug($"Buscando produto ID {id} no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabaseProduto>()
                    .Where(p => p.Id == id && p.Ativo == true)
                    .Single();

                return response?.ToProduto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar produto ID {id} no Supabase");
                return null;
            }
        }

        public async Task<Produto> CreateProdutoAsync(Produto produto)
        {
            try
            {
                _logger.LogDebug($"Criando produto {produto.Nome} no Supabase");
                
                var supabaseProduto = SupabaseProduto.FromProduto(produto);
                supabaseProduto.DataCriacao = DateTime.Now;
                
                var response = await _supabaseClient
                    .From<SupabaseProduto>()
                    .Insert(supabaseProduto);

                var criado = response.Models.First();
                return criado.ToProduto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar produto {produto.Nome} no Supabase");
                throw;
            }
        }

        public async Task<Produto> UpdateProdutoAsync(Produto produto)
        {
            try
            {
                _logger.LogDebug($"Atualizando produto ID {produto.Id} no Supabase");
                
                var supabaseProduto = SupabaseProduto.FromProduto(produto);
                supabaseProduto.DataAtualizacao = DateTime.Now;
                
                var response = await _supabaseClient
                    .From<SupabaseProduto>()
                    .Where(p => p.Id == produto.Id)
                    .Update(supabaseProduto);

                var atualizado = response.Models.First();
                return atualizado.ToProduto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar produto ID {produto.Id} no Supabase");
                throw;
            }
        }

        public async Task<bool> DeleteProdutoAsync(int id)
        {
            try
            {
                _logger.LogDebug($"Fazendo soft delete do produto ID {id} no Supabase");
                
                await _supabaseClient
                    .From<SupabaseProduto>()
                    .Where(p => p.Id == id)
                    .Update(new SupabaseProduto 
                    { 
                        Ativo = false, 
                        DataAtualizacao = DateTime.Now 
                    });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao deletar produto ID {id} no Supabase");
                return false;
            }
        }

        public async Task<List<Produto>> GetProdutosEstoqueBaixoAsync()
        {
            try
            {
                _logger.LogDebug("Buscando produtos com estoque baixo no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabaseProduto>()
                    .Where(p => p.Ativo == true)
                    .Get();
                
                // Filtrar localmente produtos com estoque baixo
                var produtosComEstoqueBaixo = response.Models
                    .Where(p => p.QuantidadeEstoque <= p.EstoqueMinimo)
                    .OrderBy(p => p.QuantidadeEstoque);

                return produtosComEstoqueBaixo.Select(sp => sp.ToProduto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar produtos com estoque baixo no Supabase");
                return new List<Produto>();
            }
        }

        public async Task<List<Produto>> SearchProdutosAsync(string termo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termo))
                    return await GetProdutosAsync();

                _logger.LogDebug($"Buscando produtos com termo '{termo}' no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabaseProduto>()
                    .Where(p => p.Ativo == true)
                    .Get();
                
                // Filtrar localmente por termo de busca
                var produtosFiltrados = response.Models
                    .Where(p => p.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                               (p.Descricao != null && p.Descricao.Contains(termo, StringComparison.OrdinalIgnoreCase)) ||
                               (p.Categoria != null && p.Categoria.Contains(termo, StringComparison.OrdinalIgnoreCase)))
                    .OrderBy(p => p.Nome);

                return produtosFiltrados.Select(sp => sp.ToProduto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar produtos com termo '{termo}' no Supabase");
                return new List<Produto>();
            }
        }

        public async Task<bool> AtualizarEstoqueAsync(int produtoId, int quantidade)
        {
            try
            {
                _logger.LogDebug($"Atualizando estoque do produto ID {produtoId}, reduzindo {quantidade} no Supabase");

                // Primeiro buscar o produto atual
                var produto = await GetProdutoByIdAsync(produtoId);
                if (produto == null || produto.QuantidadeEstoque < quantidade)
                    return false;

                // Atualizar o estoque
                var novoEstoque = produto.QuantidadeEstoque - quantidade;
                
                await _supabaseClient
                    .From<SupabaseProduto>()
                    .Where(p => p.Id == produtoId)
                    .Update(new SupabaseProduto 
                    { 
                        QuantidadeEstoque = novoEstoque,
                        DataAtualizacao = DateTime.Now 
                    });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar estoque do produto ID {produtoId} no Supabase");
                return false;
            }
        }

        private List<Produto> GetProdutosExemplo()
        {
            return new List<Produto>
            {
                new Produto
                {
                    Id = 1,
                    Nome = "Água Mineral 500ml",
                    Descricao = "Água mineral natural",
                    Preco = 3.50m,
                    Categoria = "Bebidas",
                    QuantidadeEstoque = 100,
                    EstoqueMinimo = 20,
                    Ativo = true,
                    DataCriacao = DateTime.Now.AddDays(-30)
                },
                new Produto
                {
                    Id = 2,
                    Nome = "Refrigerante Lata 350ml",
                    Descricao = "Refrigerante gelado",
                    Preco = 5.00m,
                    Categoria = "Bebidas",
                    QuantidadeEstoque = 50,
                    EstoqueMinimo = 15,
                    Ativo = true,
                    DataCriacao = DateTime.Now.AddDays(-25)
                },
                new Produto
                {
                    Id = 3,
                    Nome = "Sanduíche Natural",
                    Descricao = "Sanduíche integral com peito de peru",
                    Preco = 12.00m,
                    Categoria = "Lanches",
                    QuantidadeEstoque = 25,
                    EstoqueMinimo = 5,
                    Ativo = true,
                    DataCriacao = DateTime.Now.AddDays(-20)
                }
            };
        }
    }
}
