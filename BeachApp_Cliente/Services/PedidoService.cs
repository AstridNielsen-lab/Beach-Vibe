using Microsoft.EntityFrameworkCore;
using BeachApp_Cliente.Data;
using BeachApp_Cliente.Models;

namespace BeachApp_Cliente.Services
{
    public interface IPedidoService
    {
        Task<Pedido> CreatePedidoAsync(Pedido pedido);
        Task<List<Pedido>> GetPedidosClienteAsync(string nomeCliente);
        Task<Pedido?> GetPedidoByIdAsync(int id);
        Task<Pedido?> GetPedidoByNumeroAsync(string numeroPedido);
    }

    public class PedidoService : IPedidoService
    {
        private readonly BeachAppContext _context;

        public PedidoService(BeachAppContext context)
        {
            _context = context;
        }

        public async Task<Pedido> CreatePedidoAsync(Pedido pedido)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            
            try
            {
                // Gerar número do pedido
                pedido.NumeroPedido = await GerarNumeroPedidoAsync();
                
                // Calcular total
                pedido.Total = pedido.Itens.Sum(i => i.Subtotal);
                
                // Adicionar pedido
                _context.Pedidos.Add(pedido);
                await _context.SaveChangesAsync();
                
                // Atualizar estoque dos produtos
                foreach (var item in pedido.Itens)
                {
                    var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                    if (produto != null)
                    {
                        produto.QuantidadeEstoque -= item.Quantidade;
                        produto.DataAtualizacao = DateTime.Now;
                    }
                }
                
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

        public async Task<List<Pedido>> GetPedidosClienteAsync(string nomeCliente)
        {
            return await _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .Where(p => p.NomeCliente.ToLower().Contains(nomeCliente.ToLower()))
                .OrderByDescending(p => p.DataPedido)
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

        private async Task<string> GerarNumeroPedidoAsync()
        {
            var hoje = DateTime.Today;
            var pedidosHoje = await _context.Pedidos
                .Where(p => p.DataPedido.Date == hoje)
                .CountAsync();
            
            return $"P{hoje:yyyyMMdd}{(pedidosHoje + 1):D3}";
        }
    }
}
