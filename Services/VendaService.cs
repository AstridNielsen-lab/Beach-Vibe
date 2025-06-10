using Microsoft.EntityFrameworkCore;
using AdmBeachApp.Data;
using AdmBeachApp.Models;

namespace AdmBeachApp.Services
{
    public interface IVendaService
    {
        Task<List<Venda>> GetVendasAsync();
        Task<Venda?> GetVendaByIdAsync(int id);
        Task<Venda> CreateVendaAsync(Venda venda);
        Task<List<Venda>> GetVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<decimal> GetTotalVendasHojeAsync();
        Task<List<Venda>> GetVendasHojeAsync();
        Task<Dictionary<string, decimal>> GetRelatorioVendasPorCategoriaAsync(DateTime dataInicio, DateTime dataFim);
    }
    
    public class VendaService : IVendaService
    {
        private readonly BeachAppContext _context;
        private readonly IProdutoService _produtoService;
        
        public VendaService(BeachAppContext context, IProdutoService produtoService)
        {
            _context = context;
            _produtoService = produtoService;
        }
        
        public async Task<List<Venda>> GetVendasAsync()
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }
        
        public async Task<Venda?> GetVendaByIdAsync(int id)
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
        
        public async Task<Venda> CreateVendaAsync(Venda venda)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Verifica e atualiza o estoque dos produtos
                foreach (var item in venda.Itens)
                {
                    var sucesso = await _produtoService.AtualizarEstoqueAsync(item.ProdutoId, item.Quantidade);
                    if (!sucesso)
                    {
                        throw new InvalidOperationException($"Estoque insuficiente para o produto {item.ProdutoId}");
                    }
                }
                
                // Calcula o total da venda
                venda.Total = venda.Itens.Sum(i => i.Subtotal);
                venda.DataVenda = DateTime.Now;
                
                _context.Vendas.Add(venda);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return venda;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task<List<Venda>> GetVendasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Vendas
                .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
                .Where(v => v.DataVenda.Date >= dataInicio.Date && v.DataVenda.Date <= dataFim.Date)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }
        
        public async Task<decimal> GetTotalVendasHojeAsync()
        {
            var hoje = DateTime.Today;
            return await _context.Vendas
                .Where(v => v.DataVenda.Date == hoje)
                .SumAsync(v => v.Total);
        }
        
        public async Task<List<Venda>> GetVendasHojeAsync()
        {
            var hoje = DateTime.Today;
            return await _context.Vendas
                .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
                .Where(v => v.DataVenda.Date == hoje)
                .OrderByDescending(v => v.DataVenda)
                .ToListAsync();
        }
        
        public async Task<Dictionary<string, decimal>> GetRelatorioVendasPorCategoriaAsync(DateTime dataInicio, DateTime dataFim)
        {
            var vendas = await _context.ItensVenda
                .Include(i => i.Produto)
                .Include(i => i.Venda)
                .Where(i => i.Venda!.DataVenda.Date >= dataInicio.Date && 
                           i.Venda.DataVenda.Date <= dataFim.Date)
                .GroupBy(i => i.Produto!.Categoria ?? "Sem Categoria")
                .Select(g => new
                {
                    Categoria = g.Key,
                    Total = g.Sum(i => i.Subtotal)
                })
                .ToDictionaryAsync(x => x.Categoria, x => x.Total);
                
            return vendas;
        }
    }
}

