using Microsoft.EntityFrameworkCore;
using BeachApp_Cliente.Data;
using BeachApp_Cliente.Models;

namespace BeachApp_Cliente.Services
{
    public interface IProdutoService
    {
        Task<List<Produto>> GetProdutosAsync();
        Task<List<Produto>> GetProdutosDisponiveisAsync();
        Task<Produto?> GetProdutoByIdAsync(int id);
    }

    public class ProdutoService : IProdutoService
    {
        private readonly BeachAppContext _context;
        private static bool _dbInitialized = false;
        private static readonly object _lock = new object();

        public ProdutoService(BeachAppContext context)
        {
            _context = context;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            if (!_dbInitialized)
            {
                lock (_lock)
                {
                    if (!_dbInitialized)
                    {
                        try
                        {
                            _context.Database.EnsureCreated();
                            
                            // Verificar se há dados e popular se necessário
                            if (!_context.Produtos.Any())
                            {
                                PopularDadosIniciais();
                            }
                            
                            _dbInitialized = true;
                            System.Diagnostics.Debug.WriteLine("Banco de dados inicializado com sucesso");
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Erro ao inicializar banco: {ex.Message}");
                        }
                    }
                }
            }
        }
        
        private void PopularDadosIniciais()
        {
            try
            {
                var produtos = new List<Produto>
                {
                    new Produto
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
                    new Produto
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
                    new Produto
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
                    new Produto
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
                    new Produto
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
                
                _context.Produtos.AddRange(produtos);
                _context.SaveChanges();
                
                System.Diagnostics.Debug.WriteLine($"Adicionados {produtos.Count} produtos de exemplo ao banco");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao popular dados iniciais: {ex.Message}");
            }
        }

        public async Task<List<Produto>> GetProdutosAsync()
        {
            return await _context.Produtos
                .Where(p => p.Ativo)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }

        public async Task<List<Produto>> GetProdutosDisponiveisAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ProdutoService: Iniciando GetProdutosDisponiveisAsync");
                
                // Verificar se o banco foi criado
                await _context.Database.EnsureCreatedAsync();
                System.Diagnostics.Debug.WriteLine("ProdutoService: Banco de dados verificado");
                
                // Tentar obter todos os produtos primeiro
                var todosProdutos = await _context.Produtos.ToListAsync();
                System.Diagnostics.Debug.WriteLine($"ProdutoService: Total de produtos no banco: {todosProdutos.Count}");
                
                // Filtrar produtos disponíveis
                var produtosDisponiveis = await _context.Produtos
                    .Where(p => p.Ativo && p.QuantidadeEstoque > 0)
                    .OrderBy(p => p.Nome)
                    .ToListAsync();
                    
                System.Diagnostics.Debug.WriteLine($"ProdutoService: Produtos disponíveis encontrados: {produtosDisponiveis.Count}");
                
                return produtosDisponiveis;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ProdutoService: Erro ao obter produtos: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"ProdutoService: Stack trace: {ex.StackTrace}");
                
                // Retornar lista vazia em caso de erro
                return new List<Produto>();
            }
        }

        public async Task<Produto?> GetProdutoByIdAsync(int id)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);
        }
    }
}
