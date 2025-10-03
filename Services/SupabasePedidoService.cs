using AdmBeachApp.Models;
using Microsoft.Extensions.Logging;

namespace AdmBeachApp.Services
{
    public class SupabasePedidoService : IPedidoService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly IProdutoService _produtoService;
        private readonly ILogger<SupabasePedidoService> _logger;

        public SupabasePedidoService(
            Supabase.Client supabaseClient, 
            IProdutoService produtoService,
            ILogger<SupabasePedidoService> logger)
        {
            _supabaseClient = supabaseClient;
            _produtoService = produtoService;
            _logger = logger;
        }

        public async Task<List<Pedido>> GetPedidosAsync()
        {
            try
            {
                _logger.LogDebug("Buscando todos os pedidos no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Get();

                var pedidos = new List<Pedido>();
                foreach (var supabasePedido in response.Models)
                {
                    var pedido = supabasePedido.ToPedido();
                    pedido.Itens = await GetItensPedidoAsync(pedido.Id);
                    pedidos.Add(pedido);
                }

                return pedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos no Supabase");
                return GetPedidosExemplo();
            }
        }

        public async Task<List<Pedido>> GetPedidosAtivoAsync()
        {
            try
            {
                _logger.LogDebug("Buscando pedidos ativos no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.Status != "Entregue" && p.Status != "Cancelado")
                    .Get();

                var pedidos = new List<Pedido>();
                foreach (var supabasePedido in response.Models)
                {
                    var pedido = supabasePedido.ToPedido();
                    pedido.Itens = await GetItensPedidoAsync(pedido.Id);
                    pedidos.Add(pedido);
                }

                return pedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos ativos no Supabase");
                return new List<Pedido>();
            }
        }

        public async Task<Pedido?> GetPedidoByIdAsync(int id)
        {
            try
            {
                _logger.LogDebug($"Buscando pedido ID {id} no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.Id == id)
                    .Single();

                if (response == null) return null;

                var pedido = response.ToPedido();
                pedido.Itens = await GetItensPedidoAsync(pedido.Id);
                return pedido;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar pedido ID {id} no Supabase");
                return null;
            }
        }

        public async Task<Pedido?> GetPedidoByNumeroAsync(string numeroPedido)
        {
            try
            {
                _logger.LogDebug($"Buscando pedido número {numeroPedido} no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.NumeroPedido == numeroPedido)
                    .Single();

                if (response == null) return null;

                var pedido = response.ToPedido();
                pedido.Itens = await GetItensPedidoAsync(pedido.Id);
                return pedido;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar pedido número {numeroPedido} no Supabase");
                return null;
            }
        }

        public async Task<Pedido> CreatePedidoAsync(Pedido pedido)
        {
            try
            {
                _logger.LogDebug($"Criando pedido {pedido.NumeroPedido} no Supabase");

                // Gerar número do pedido se não fornecido
                if (string.IsNullOrEmpty(pedido.NumeroPedido))
                {
                    pedido.NumeroPedido = await GerarNumeroPedidoAsync();
                }

                // Verificação de estoque
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

                // Calcular total
                pedido.Total = pedido.Itens.Sum(i => i.Subtotal);
                pedido.DataPedido = DateTime.Now;

                // Criar pedido no Supabase
                var supabasePedido = SupabasePedido.FromPedido(pedido);
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Insert(supabasePedido);

                var pedidoCriado = response.Models.First();

                // Criar itens do pedido
                foreach (var item in pedido.Itens)
                {
                    item.PedidoId = pedidoCriado.Id;
                    var supabaseItem = SupabaseItemPedido.FromItemPedido(item);
                    
                    await _supabaseClient
                        .From<SupabaseItemPedido>()
                        .Insert(supabaseItem);
                }

                // Retornar pedido com itens
                var pedidoFinal = pedidoCriado.ToPedido();
                pedidoFinal.Itens = await GetItensPedidoAsync(pedidoFinal.Id);
                
                return pedidoFinal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar pedido {pedido.NumeroPedido} no Supabase");
                throw;
            }
        }

        public async Task<Pedido> UpdateStatusPedidoAsync(int pedidoId, StatusPedido novoStatus)
        {
            try
            {
                _logger.LogDebug($"Atualizando status do pedido ID {pedidoId} para {novoStatus} no Supabase");

                var pedido = await GetPedidoByIdAsync(pedidoId);
                if (pedido == null)
                {
                    throw new InvalidOperationException("Pedido não encontrado");
                }

                var dataEntrega = novoStatus == StatusPedido.Entregue ? DateTime.Now : (DateTime?)null;

                await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.Id == pedidoId)
                    .Update(new SupabasePedido 
                    { 
                        Status = novoStatus.ToString(),
                        DataAtualizacao = DateTime.Now
                    });

                // Se entregue, baixar estoque
                if (novoStatus == StatusPedido.Entregue)
                {
                    foreach (var item in pedido.Itens)
                    {
                        await _produtoService.AtualizarEstoqueAsync(item.ProdutoId, item.Quantidade);
                    }
                }

                pedido.Status = novoStatus;
                pedido.DataEntrega = dataEntrega;
                return pedido;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar status do pedido ID {pedidoId} no Supabase");
                throw;
            }
        }

        public async Task<List<Pedido>> GetPedidosPorStatusAsync(StatusPedido status)
        {
            try
            {
                _logger.LogDebug($"Buscando pedidos com status {status} no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.Status == status.ToString())
                    .Get();

                var pedidos = new List<Pedido>();
                foreach (var supabasePedido in response.Models)
                {
                    var pedido = supabasePedido.ToPedido();
                    pedido.Itens = await GetItensPedidoAsync(pedido.Id);
                    pedidos.Add(pedido);
                }

                return pedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar pedidos com status {status} no Supabase");
                return new List<Pedido>();
            }
        }

        public async Task<List<Pedido>> GetPedidosHojeAsync()
        {
            try
            {
                var hoje = DateTime.Today;
                var amanha = hoje.AddDays(1);
                
                _logger.LogDebug($"Buscando pedidos de hoje ({hoje:dd/MM/yyyy}) no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.DataPedido >= hoje && p.DataPedido < amanha)
                    .Get();

                var pedidos = new List<Pedido>();
                foreach (var supabasePedido in response.Models)
                {
                    var pedido = supabasePedido.ToPedido();
                    pedido.Itens = await GetItensPedidoAsync(pedido.Id);
                    pedidos.Add(pedido);
                }

                return pedidos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar pedidos de hoje no Supabase");
                return new List<Pedido>();
            }
        }

        public async Task<decimal> GetTotalPedidosHojeAsync()
        {
            try
            {
                var hoje = DateTime.Today;
                var amanha = hoje.AddDays(1);
                
                _logger.LogDebug($"Calculando total dos pedidos entregues hoje no Supabase");
                
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.DataPedido >= hoje && 
                               p.DataPedido < amanha && 
                               p.Status == "Entregue")
                    .Get();

                return response.Models.Sum(p => p.Total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular total dos pedidos de hoje no Supabase");
                return 0;
            }
        }

        public async Task<string> GerarNumeroPedidoAsync()
        {
            try
            {
                var hoje = DateTime.Today;
                var amanha = hoje.AddDays(1);
                
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.DataPedido >= hoje && p.DataPedido < amanha)
                    .Get();

                var contadorHoje = response.Models.Count;
                return $"{hoje:yyyyMMdd}-{(contadorHoje + 1):D3}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar número do pedido, usando fallback");
                return $"{DateTime.Now:yyyyMMddHHmmss}";
            }
        }

        public async Task<bool> CancelarPedidoAsync(int pedidoId, string motivo = "")
        {
            try
            {
                _logger.LogDebug($"Cancelando pedido ID {pedidoId} no Supabase");

                var pedido = await GetPedidoByIdAsync(pedidoId);
                if (pedido == null || pedido.Status == StatusPedido.Entregue)
                {
                    return false;
                }

                var observacoes = string.IsNullOrEmpty(pedido.Observacoes)
                    ? $"Cancelado: {motivo}"
                    : $"{pedido.Observacoes} | Cancelado: {motivo}";

                await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.Id == pedidoId)
                    .Update(new SupabasePedido 
                    { 
                        Status = "Cancelado",
                        Observacoes = string.IsNullOrEmpty(motivo) ? null : observacoes,
                        DataAtualizacao = DateTime.Now
                    });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao cancelar pedido ID {pedidoId} no Supabase");
                return false;
            }
        }

        private async Task<List<ItemPedido>> GetItensPedidoAsync(int pedidoId)
        {
            try
            {
                var response = await _supabaseClient
                    .From<SupabaseItemPedido>()
                    .Where(i => i.PedidoId == pedidoId)
                    .Get();

                var itens = new List<ItemPedido>();
                foreach (var supabaseItem in response.Models)
                {
                    var item = supabaseItem.ToItemPedido();
                    // Buscar produto para preencher informações
                    item.Produto = await _produtoService.GetProdutoByIdAsync(item.ProdutoId);
                    itens.Add(item);
                }

                return itens;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar itens do pedido ID {pedidoId}");
                return new List<ItemPedido>();
            }
        }

        private List<Pedido> GetPedidosExemplo()
        {
            return new List<Pedido>
            {
                new Pedido
                {
                    Id = 1,
                    NumeroPedido = $"{DateTime.Today:yyyyMMdd}-001",
                    NomeCliente = "Cliente Exemplo",
                    TelefoneCliente = "(11) 99999-9999",
                    Status = StatusPedido.Preparando,
                    DataPedido = DateTime.Now.AddHours(-2),
                    Total = 25.50m,
                    Itens = new List<ItemPedido>
                    {
                        new ItemPedido
                        {
                            Id = 1,
                            PedidoId = 1,
                            ProdutoId = 1,
                            Quantidade = 2,
                            PrecoUnitario = 3.50m,
                            Produto = new Produto
                            {
                                Id = 1,
                                Nome = "Água Mineral 500ml",
                                Preco = 3.50m
                            }
                        },
                        new ItemPedido
                        {
                            Id = 2,
                            PedidoId = 1,
                            ProdutoId = 3,
                            Quantidade = 1,
                            PrecoUnitario = 12.00m,
                            Produto = new Produto
                            {
                                Id = 3,
                                Nome = "Sanduíche Natural",
                                Preco = 12.00m,
                                Categoria = "Comida"
                            }
                        }
                    }
                }
            };
        }
        
        public async Task UpdateStatusAsync(int pedidoId, StatusPedido novoStatus)
        {
            await UpdateStatusPedidoAsync(pedidoId, novoStatus);
        }
    }
}
