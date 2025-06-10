using Microsoft.EntityFrameworkCore;
using AdmBeachApp.Data;
using AdmBeachApp.Models;

namespace AdmBeachApp.Services
{
    public interface IPedidoService
    {
        Task<List<Pedido>> GetPedidosAsync();
        Task<List<Pedido>> GetPedidosAtivoAsync();
        Task<Pedido?> GetPedidoByIdAsync(int id);
        Task<Pedido?> GetPedidoByNumeroAsync(string numeroPedido);
        Task<Pedido> CreatePedidoAsync(Pedido pedido);
        Task<Pedido> UpdateStatusPedidoAsync(int pedidoId, StatusPedido novoStatus);
        Task<List<Pedido>> GetPedidosPorStatusAsync(StatusPedido status);
        Task<List<Pedido>> GetPedidosHojeAsync();
        Task<decimal> GetTotalPedidosHojeAsync();
        Task<string> GerarNumeroPedidoAsync();
        Task<bool> CancelarPedidoAsync(int pedidoId, string motivo = "");
    }
    
    public class PedidoService : IPedidoService
    {
        private readonly BeachAppContext _context;
        private readonly IProdutoService _produtoService;
        
        public PedidoService(BeachAppContext context, IProdutoService produtoService)
        {
            _context = context;
            _produtoService = produtoService;
        }
        
        public async Task<List<Pedido>> GetPedidosAsync()
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();
        }
        
        public async Task<List<Pedido>> GetPedidosAtivoAsync()
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .Where(p => p.Status != StatusPedido.Entregue && p.Status != StatusPedido.Cancelado)
                .OrderBy(p => p.DataPedido)
                .ToListAsync();
        }
        
        public async Task<Pedido?> GetPedidoByIdAsync(int id)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        
        public async Task<Pedido?> GetPedidoByNumeroAsync(string numeroPedido)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(p => p.NumeroPedido == numeroPedido);
        }
        
        public async Task<Pedido> CreatePedidoAsync(Pedido pedido)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Gerar número do pedido se não fornecido
                if (string.IsNullOrEmpty(pedido.NumeroPedido))
                {
                    pedido.NumeroPedido = await GerarNumeroPedidoAsync();
                }
                
                // Verificação de estoque (opcional - dependendo se quer reservar estoque)
                foreach (var item in pedido.Itens)
                {
                    var produto = await _produtoService.GetProdutoByIdAsync(item.ProdutoId);
                    if (produto == null)
                    {
                        throw new InvalidOperationException($"Produto {item.ProdutoId} não encontrado");
                    }
                    
                    if (produto.QuantidadeEstoque < item.Quantidade)
                    {
                        throw new InvalidOperationException($"Estoque insuficiente para {produto.Nome}. Disponível: {produto.QuantidadeEstoque}");
                    }
                }
                
                // Calcula o total do pedido
                pedido.Total = pedido.Itens.Sum(i => i.Subtotal);
                pedido.DataPedido = DateTime.Now;
                
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return pedido;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        
        public async Task<Pedido> UpdateStatusPedidoAsync(int pedidoId, StatusPedido novoStatus)
        {
            var pedido = await _context.Pedidos.FindAsync(pedidoId);
            if (pedido == null)
            {
                throw new InvalidOperationException("Pedido não encontrado");
            }
            
            pedido.Status = novoStatus;
            
            if (novoStatus == StatusPedido.Entregue)
            {
                pedido.DataEntrega = DateTime.Now;
                
                // Ao entregar, podemos opcionalmente baixar do estoque
                // (se não foi baixado na criação do pedido)
                foreach (var item in await _context.ItensPedido
                    .Where(i => i.PedidoId == pedidoId)
                    .ToListAsync())
                {
                    await _produtoService.AtualizarEstoqueAsync(item.ProdutoId, item.Quantidade);
                }
            }
            
            await _context.SaveChangesAsync();
            return pedido;
        }
        
        public async Task<List<Pedido>> GetPedidosPorStatusAsync(StatusPedido status)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .Where(p => p.Status == status)
                .OrderBy(p => p.DataPedido)
                .ToListAsync();
        }
        
        public async Task<List<Pedido>> GetPedidosHojeAsync()
        {
            var hoje = DateTime.Today;
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .Where(p => p.DataPedido.Date == hoje)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();
        }
        
        public async Task<decimal> GetTotalPedidosHojeAsync()
        {
            var hoje = DateTime.Today;
            return await _context.Pedidos
                .Where(p => p.DataPedido.Date == hoje && p.Status == StatusPedido.Entregue)
                .SumAsync(p => p.Total);
        }
        
        public async Task<string> GerarNumeroPedidoAsync()
        {
            var hoje = DateTime.Today;
            var contadorHoje = await _context.Pedidos
                .Where(p => p.DataPedido.Date == hoje)
                .CountAsync();
                
            return $"{hoje:yyyyMMdd}-{(contadorHoje + 1):D3}";
        }
        
        public async Task<bool> CancelarPedidoAsync(int pedidoId, string motivo = "")
        {
            var pedido = await _context.Pedidos.FindAsync(pedidoId);
            if (pedido == null || pedido.Status == StatusPedido.Entregue)
            {
                return false;
            }
            
            pedido.Status = StatusPedido.Cancelado;
            if (!string.IsNullOrEmpty(motivo))
            {
                pedido.Observacoes = string.IsNullOrEmpty(pedido.Observacoes) 
                    ? $"Cancelado: {motivo}" 
                    : $"{pedido.Observacoes} | Cancelado: {motivo}";
            }
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

