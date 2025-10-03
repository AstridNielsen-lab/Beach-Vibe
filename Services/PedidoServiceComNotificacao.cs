using AdmBeachApp.Models;

namespace AdmBeachApp.Services
{
    public class PedidoServiceComNotificacao : IPedidoService
    {
        private readonly IPedidoService _pedidoService;
        private readonly IEventNotificacaoService _notificacaoService;
        private readonly ILogger<PedidoServiceComNotificacao> _logger;

        public PedidoServiceComNotificacao(
            IPedidoService pedidoService, 
            IEventNotificacaoService notificacaoService,
            ILogger<PedidoServiceComNotificacao> logger)
        {
            _pedidoService = pedidoService;
            _notificacaoService = notificacaoService;
            _logger = logger;
        }

        public async Task<Pedido> CreatePedidoAsync(Pedido pedido)
        {
            try
            {
                // Criar o pedido usando o serviço base
                var pedidoCriado = await _pedidoService.CreatePedidoAsync(pedido);
                
                // Disparar notificação para os setores
                await _notificacaoService.NotificarNovoPedido(pedidoCriado);
                
                _logger.LogInformation($"Pedido #{pedidoCriado.NumeroPedido} criado e notificado com sucesso");
                
                return pedidoCriado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao criar pedido com notificações");
                throw;
            }
        }

        public async Task<bool> CancelarPedidoAsync(int pedidoId, string motivo = "")
        {
            try
            {
                var resultado = await _pedidoService.CancelarPedidoAsync(pedidoId, motivo);
                
                if (resultado)
                {
                    await _notificacaoService.NotificarPedidoCancelado(pedidoId);
                    _logger.LogInformation($"Pedido {pedidoId} cancelado e notificado com sucesso");
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao cancelar pedido {pedidoId} com notificações");
                throw;
            }
        }

        public async Task<Pedido> UpdateStatusPedidoAsync(int pedidoId, StatusPedido novoStatus)
        {
            try
            {
                var pedido = await _pedidoService.UpdateStatusPedidoAsync(pedidoId, novoStatus);
                
                await _notificacaoService.NotificarStatusPedido(pedidoId, novoStatus);
                
                _logger.LogInformation($"Status do pedido {pedidoId} atualizado para {novoStatus} e notificado");
                
                return pedido;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao atualizar status do pedido {pedidoId} com notificações");
                throw;
            }
        }

        public async Task UpdateStatusAsync(int pedidoId, StatusPedido novoStatus)
        {
            await UpdateStatusPedidoAsync(pedidoId, novoStatus);
        }

        // Todos os outros métodos são apenas passthrough para o serviço base
        public Task<List<Pedido>> GetPedidosAsync() => _pedidoService.GetPedidosAsync();
        
        public Task<List<Pedido>> GetPedidosAtivoAsync() => _pedidoService.GetPedidosAtivoAsync();
        
        public Task<Pedido?> GetPedidoByIdAsync(int id) => _pedidoService.GetPedidoByIdAsync(id);
        
        public Task<Pedido?> GetPedidoByNumeroAsync(string numeroPedido) => _pedidoService.GetPedidoByNumeroAsync(numeroPedido);
        
        public Task<List<Pedido>> GetPedidosPorStatusAsync(StatusPedido status) => _pedidoService.GetPedidosPorStatusAsync(status);
        
        public Task<List<Pedido>> GetPedidosHojeAsync() => _pedidoService.GetPedidosHojeAsync();
        
        public Task<decimal> GetTotalPedidosHojeAsync() => _pedidoService.GetTotalPedidosHojeAsync();
        
        public Task<string> GerarNumeroPedidoAsync() => _pedidoService.GerarNumeroPedidoAsync();
    }
}
