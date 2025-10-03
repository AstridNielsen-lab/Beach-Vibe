using BeachApp_Cliente.Data;
using BeachApp_Cliente.Models;
using Supabase;

namespace BeachApp_Cliente.Services
{
    public class SupabaseProdutoService : IProdutoService
    {
        private readonly Client _supabaseClient;

        public SupabaseProdutoService(Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<List<Produto>> GetProdutosAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SupabaseProdutoService: Iniciando GetProdutosAsync");
                
                var response = await _supabaseClient
                    .From<SupabaseProduto>()
                    .Where(p => p.Ativo == true)
                    .Order("nome", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var produtos = response.Models.Select(p => p.ToProduto()).ToList();
                
                System.Diagnostics.Debug.WriteLine($"SupabaseProdutoService: {produtos.Count} produtos carregados do Supabase");
                
                return produtos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabaseProdutoService: Erro ao obter produtos - {ex.Message}");
                
                // Em caso de erro, retorna uma lista com produtos de exemplo
                return GetProdutosExemplo();
            }
        }

        public async Task<List<Produto>> GetProdutosDisponiveisAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SupabaseProdutoService: Iniciando GetProdutosDisponiveisAsync");
                
                // Inicializar o cliente Supabase se necessário
                if (!_supabaseClient.Auth.CurrentSession?.AccessToken?.Any() == true)
                {
                    await _supabaseClient.InitializeAsync();
                }
                
                var response = await _supabaseClient
                    .From<SupabaseProduto>()
                    .Where(p => p.Ativo == true && p.QuantidadeEstoque > 0)
                    .Order("nome", Supabase.Postgrest.Constants.Ordering.Ascending)
                    .Get();

                var produtos = response.Models.Select(p => p.ToProduto()).ToList();
                
                System.Diagnostics.Debug.WriteLine($"SupabaseProdutoService: {produtos.Count} produtos disponíveis carregados do Supabase");
                
                // Se não há produtos no Supabase, criar produtos de exemplo
                if (produtos.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("SupabaseProdutoService: Nenhum produto encontrado, retornando produtos de exemplo");
                    return GetProdutosExemplo();
                }
                
                return produtos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabaseProdutoService: Erro ao obter produtos disponíveis - {ex.Message}");
                
                // Em caso de erro, retorna uma lista com produtos de exemplo
                return GetProdutosExemplo();
            }
        }

        public async Task<Produto?> GetProdutoByIdAsync(int id)
        {
            try
            {
                var response = await _supabaseClient
                    .From<SupabaseProduto>()
                    .Where(p => p.Id == id && p.Ativo == true)
                    .Single();

                return response?.ToProduto();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabaseProdutoService: Erro ao obter produto por ID - {ex.Message}");
                return null;
            }
        }

        private async Task CriarProdutosExemplo()
        {
            try
            {
                var produtosExemplo = new List<SupabaseProduto>
                {
                    new SupabaseProduto
                    {
                        Nome = "Água de Coco",
                        Descricao = "Água de coco gelada e refrescante",
                        Preco = 8.00m,
                        QuantidadeEstoque = 50,
                        EstoqueMinimo = 10,
                        Categoria = "Bebidas",
                        Ativo = true,
                        DataCriacao = DateTime.Now
                    },
                    new SupabaseProduto
                    {
                        Nome = "Caipirinha",
                        Descricao = "Caipirinha tradicional com limão",
                        Preco = 15.00m,
                        QuantidadeEstoque = 30,
                        EstoqueMinimo = 5,
                        Categoria = "Bebidas",
                        Ativo = true,
                        DataCriacao = DateTime.Now
                    },
                    new SupabaseProduto
                    {
                        Nome = "Porção de Camarão",
                        Descricao = "Camarão empanado com molho especial",
                        Preco = 35.00m,
                        QuantidadeEstoque = 20,
                        EstoqueMinimo = 3,
                        Categoria = "Petiscos",
                        Ativo = true,
                        DataCriacao = DateTime.Now
                    },
                    new SupabaseProduto
                    {
                        Nome = "Açaí na Tigela",
                        Descricao = "Açaí com granola, banana e mel",
                        Preco = 18.00m,
                        QuantidadeEstoque = 25,
                        EstoqueMinimo = 5,
                        Categoria = "Sobremesas",
                        Ativo = true,
                        DataCriacao = DateTime.Now
                    },
                    new SupabaseProduto
                    {
                        Nome = "Sanduíche Natural",
                        Descricao = "Sanduíche natural com peito de peru",
                        Preco = 12.00m,
                        QuantidadeEstoque = 15,
                        EstoqueMinimo = 3,
                        Categoria = "Lanches",
                        Ativo = true,
                        DataCriacao = DateTime.Now
                    }
                };

                await _supabaseClient.From<SupabaseProduto>().Insert(produtosExemplo);
                System.Diagnostics.Debug.WriteLine("SupabaseProdutoService: Produtos de exemplo criados no Supabase");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabaseProdutoService: Erro ao criar produtos de exemplo - {ex.Message}");
            }
        }

        private List<Produto> GetProdutosExemplo()
        {
            return new List<Produto>
            {
                new Produto { Id = 1, Nome = "Água de Coco", Descricao = "Água de coco gelada e refrescante", Preco = 8.00m, QuantidadeEstoque = 50, EstoqueMinimo = 10, Categoria = "Bebidas", Ativo = true, DataCriacao = DateTime.Now },
                new Produto { Id = 2, Nome = "Caipirinha", Descricao = "Caipirinha tradicional com limão", Preco = 15.00m, QuantidadeEstoque = 30, EstoqueMinimo = 5, Categoria = "Bebidas", Ativo = true, DataCriacao = DateTime.Now },
                new Produto { Id = 3, Nome = "Porção de Camarão", Descricao = "Camarão empanado com molho especial", Preco = 35.00m, QuantidadeEstoque = 20, EstoqueMinimo = 3, Categoria = "Petiscos", Ativo = true, DataCriacao = DateTime.Now },
                new Produto { Id = 4, Nome = "Açaí na Tigela", Descricao = "Açaí com granola, banana e mel", Preco = 18.00m, QuantidadeEstoque = 25, EstoqueMinimo = 5, Categoria = "Sobremesas", Ativo = true, DataCriacao = DateTime.Now },
                new Produto { Id = 5, Nome = "Sanduíche Natural", Descricao = "Sanduíche natural com peito de peru", Preco = 12.00m, QuantidadeEstoque = 15, EstoqueMinimo = 3, Categoria = "Lanches", Ativo = true, DataCriacao = DateTime.Now }
            };
        }
    }
}
