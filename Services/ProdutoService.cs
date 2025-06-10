using Microsoft.EntityFrameworkCore;
using AdmBeachApp.Data;
using AdmBeachApp.Models;

namespace AdmBeachApp.Services
{
    public interface IProdutoService
    {
        Task<List<Produto>> GetProdutosAsync();
        Task<Produto?> GetProdutoByIdAsync(int id);
        Task<Produto> CreateProdutoAsync(Produto produto);
        Task<Produto> UpdateProdutoAsync(Produto produto);
        Task<bool> DeleteProdutoAsync(int id);
        Task<List<Produto>> GetProdutosEstoqueBaixoAsync();
        Task<List<Produto>> SearchProdutosAsync(string termo);
        Task<bool> AtualizarEstoqueAsync(int produtoId, int quantidade);
    }
    
    public class ProdutoService : IProdutoService
    {
        private readonly BeachAppContext _context;
        
        public ProdutoService(BeachAppContext context)
        {
            _context = context;
        }
        
        public async Task<List<Produto>> GetProdutosAsync()
        {
            return await _context.Produtos
                .Where(p => p.Ativo)
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }
        
        public async Task<Produto?> GetProdutoByIdAsync(int id)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == id && p.Ativo);
        }
        
        public async Task<Produto> CreateProdutoAsync(Produto produto)
        {
            produto.DataCriacao = DateTime.Now;
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return produto;
        }
        
        public async Task<Produto> UpdateProdutoAsync(Produto produto)
        {
            produto.DataAtualizacao = DateTime.Now;
            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return produto;
        }
        
        public async Task<bool> DeleteProdutoAsync(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null) return false;
            
            // Soft delete - apenas marca como inativo
            produto.Ativo = false;
            produto.DataAtualizacao = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
        
        public async Task<List<Produto>> GetProdutosEstoqueBaixoAsync()
        {
            return await _context.Produtos
                .Where(p => p.Ativo && p.QuantidadeEstoque <= p.EstoqueMinimo)
                .OrderBy(p => p.QuantidadeEstoque)
                .ToListAsync();
        }
        
        public async Task<List<Produto>> SearchProdutosAsync(string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return await GetProdutosAsync();
                
            return await _context.Produtos
                .Where(p => p.Ativo && 
                           (p.Nome.Contains(termo) || 
                            p.Descricao!.Contains(termo) ||
                            p.Categoria!.Contains(termo)))
                .OrderBy(p => p.Nome)
                .ToListAsync();
        }
        
        public async Task<bool> AtualizarEstoqueAsync(int produtoId, int quantidade)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            if (produto == null || produto.QuantidadeEstoque < quantidade)
                return false;
                
            produto.QuantidadeEstoque -= quantidade;
            produto.DataAtualizacao = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

