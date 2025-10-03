using AdmBeachApp.Models;
using System.Text.Json;

namespace AdmBeachApp.Services
{
    public interface IEventNotificacaoService
    {
        event EventHandler<PedidoNotificacaoEventArgs>? NotificacaoRecebida;
        Task NotificarNovoPedido(Pedido pedido);
        Task NotificarStatusPedido(int pedidoId, StatusPedido novoStatus);
        Task NotificarItemPronto(int pedidoId, int itemId);
        Task NotificarPedidoCancelado(int pedidoId);
        Task<List<PedidoNotificacao>> ObterNotificacoesPorSetor(TipoSetor setor);
        Task MarcarNotificacaoComoVisualizada(int notificacaoId);
        void IniciarEscutaSetor(TipoSetor setor);
        void PararEscutaSetor(TipoSetor setor);
    }

    public class PedidoNotificacaoEventArgs : EventArgs
    {
        public PedidoNotificacao Notificacao { get; }
        public TipoSetor SetorDestino { get; }
        public List<ItemPedido>? ItensRelevantes { get; }

        public PedidoNotificacaoEventArgs(PedidoNotificacao notificacao, TipoSetor setorDestino, List<ItemPedido>? itensRelevantes = null)
        {
            Notificacao = notificacao;
            SetorDestino = setorDestino;
            ItensRelevantes = itensRelevantes;
        }
    }

    public class EventNotificacaoService : IEventNotificacaoService
    {
        private readonly IPedidoService _pedidoService;
        private readonly IProdutoService _produtoService;
        private readonly ILogger<EventNotificacaoService> _logger;

        // Lista em memória para as notificações
        private static readonly List<PedidoNotificacao> _notificacoes = new();
        private readonly HashSet<TipoSetor> _setoresEscutando = new();

        public event EventHandler<PedidoNotificacaoEventArgs>? NotificacaoRecebida;

        public EventNotificacaoService(
            IPedidoService pedidoService,
            IProdutoService produtoService,
            ILogger<EventNotificacaoService> logger)
        {
            _pedidoService = pedidoService;
            _produtoService = produtoService;
            _logger = logger;
        }

        public async Task NotificarNovoPedido(Pedido pedido)
        {
            try
            {
                _logger.LogInformation($"Processando novo pedido #{pedido.NumeroPedido} para notificações");

                // Agrupar itens por setor
                var itensPorSetor = await AgruparItensPorSetor(pedido.Itens);

                // Criar notificações para cada setor que tem itens
                foreach (var (setor, itens) in itensPorSetor)
                {
                    var notificacao = new PedidoNotificacao
                    {
                        Id = _notificacoes.Count + 1,
                        PedidoId = pedido.Id,
                        Pedido = pedido,
                        SetorDestino = setor,
                        TipoNotificacao = TipoNotificacao.NovoPedido,
                        Titulo = $"🆕 Novo Pedido #{pedido.NumeroPedido}",
                        Mensagem = $"👤 {pedido.NomeCliente} | 🪑 Mesa {pedido.Mesa ?? "N/A"} | 📦 {itens.Sum(i => i.Quantidade)} itens",
                        ItensPedido = JsonSerializer.Serialize(itens.Select(i => new ItemPedidoDetalhado
                        {
                            ItemId = i.Id,
                            NomeProduto = i.Produto?.Nome ?? "",
                            Quantidade = i.Quantidade,
                            PrecoUnitario = i.PrecoUnitario,
                            Observacoes = i.ObservacoesItem,
                            SetorResponsavel = setor,
                            Status = StatusItemPedido.Pendente
                        }))
                    };

                    _notificacoes.Add(notificacao);

                    // Disparar evento para o setor específico
                    DispararEventoNotificacao(notificacao, setor, itens);
                }

                // Notificar também o caixa sobre o pedido completo
                await NotificarCaixa(pedido);

                _logger.LogInformation($"Pedido #{pedido.NumeroPedido} notificado com sucesso para {itensPorSetor.Count} setores");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao processar notificações para o pedido #{pedido.NumeroPedido}");
            }
        }

        public async Task NotificarStatusPedido(int pedidoId, StatusPedido novoStatus)
        {
            try
            {
                var pedido = await _pedidoService.GetPedidoByIdAsync(pedidoId);
                if (pedido == null) return;

                var notificacao = new PedidoNotificacao
                {
                    Id = _notificacoes.Count + 1,
                    PedidoId = pedidoId,
                    Pedido = pedido,
                    SetorDestino = TipoSetor.Todos,
                    TipoNotificacao = TipoNotificacao.StatusAtualizado,
                    Titulo = $"📋 Status Atualizado - #{pedido.NumeroPedido}",
                    Mensagem = $"🔄 {pedido.StatusTexto}"
                };

                _notificacoes.Add(notificacao);

                // Disparar para todos os setores
                DispararEventoNotificacao(notificacao, TipoSetor.Todos);

                _logger.LogInformation($"Status do pedido #{pedido.NumeroPedido} atualizado para {novoStatus}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao notificar mudança de status do pedido {pedidoId}");
            }
        }

        public async Task NotificarItemPronto(int pedidoId, int itemId)
        {
            try
            {
                var pedido = await _pedidoService.GetPedidoByIdAsync(pedidoId);
                if (pedido == null) return;

                var item = pedido.Itens.FirstOrDefault(i => i.Id == itemId);
                if (item == null) return;

                var notificacao = new PedidoNotificacao
                {
                    Id = _notificacoes.Count + 1,
                    PedidoId = pedidoId,
                    Pedido = pedido,
                    SetorDestino = TipoSetor.Todos,
                    TipoNotificacao = TipoNotificacao.ItemPronto,
                    Titulo = $"✅ Item Pronto - #{pedido.NumeroPedido}",
                    Mensagem = $"🍽️ {item.Produto?.Nome} (Qtd: {item.Quantidade})"
                };

                _notificacoes.Add(notificacao);

                DispararEventoNotificacao(notificacao, TipoSetor.Todos);

                _logger.LogInformation($"Item {item.Produto?.Nome} do pedido #{pedido.NumeroPedido} marcado como pronto");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao notificar item pronto do pedido {pedidoId}, item {itemId}");
            }
        }

        public async Task NotificarPedidoCancelado(int pedidoId)
        {
            try
            {
                var pedido = await _pedidoService.GetPedidoByIdAsync(pedidoId);
                if (pedido == null) return;

                var notificacao = new PedidoNotificacao
                {
                    Id = _notificacoes.Count + 1,
                    PedidoId = pedidoId,
                    Pedido = pedido,
                    SetorDestino = TipoSetor.Todos,
                    TipoNotificacao = TipoNotificacao.PedidoCancelado,
                    Titulo = $"❌ Pedido Cancelado #{pedido.NumeroPedido}",
                    Mensagem = $"👤 Cliente: {pedido.NomeCliente}"
                };

                _notificacoes.Add(notificacao);

                DispararEventoNotificacao(notificacao, TipoSetor.Todos);

                _logger.LogInformation($"Pedido #{pedido.NumeroPedido} cancelado e notificado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao notificar cancelamento do pedido {pedidoId}");
            }
        }

        public Task<List<PedidoNotificacao>> ObterNotificacoesPorSetor(TipoSetor setor)
        {
            var notificacoes = _notificacoes
                .Where(n => n.SetorDestino == setor || n.SetorDestino == TipoSetor.Todos)
                .Where(n => n.Ativa && !n.Visualizada)
                .OrderByDescending(n => n.DataCriacao)
                .ToList();

            return Task.FromResult(notificacoes);
        }

        public Task MarcarNotificacaoComoVisualizada(int notificacaoId)
        {
            var notificacao = _notificacoes.FirstOrDefault(n => n.Id == notificacaoId);
            if (notificacao != null)
            {
                notificacao.Visualizada = true;
                notificacao.DataVisualizacao = DateTime.Now;
            }

            return Task.CompletedTask;
        }

        public void IniciarEscutaSetor(TipoSetor setor)
        {
            _setoresEscutando.Add(setor);
            _logger.LogInformation($"Iniciada escuta de notificações para o setor: {setor}");
        }

        public void PararEscutaSetor(TipoSetor setor)
        {
            _setoresEscutando.Remove(setor);
            _logger.LogInformation($"Parada escuta de notificações para o setor: {setor}");
        }

        private async Task<Dictionary<TipoSetor, List<ItemPedido>>> AgruparItensPorSetor(List<ItemPedido> itens)
        {
            var itensPorSetor = new Dictionary<TipoSetor, List<ItemPedido>>();

            foreach (var item in itens)
            {
                if (item.Produto == null && item.ProdutoId > 0)
                {
                    item.Produto = await _produtoService.GetProdutoByIdAsync(item.ProdutoId);
                }

                var setor = item.Produto?.DeterminarSetor() ?? TipoSetor.Balcao;

                if (!itensPorSetor.ContainsKey(setor))
                {
                    itensPorSetor[setor] = new List<ItemPedido>();
                }

                itensPorSetor[setor].Add(item);
            }

            return itensPorSetor;
        }

        private void DispararEventoNotificacao(PedidoNotificacao notificacao, TipoSetor setor, List<ItemPedido>? itens = null)
        {
            try
            {
                // Só dispara o evento se alguém está escutando esse setor ou se é para todos
                if (_setoresEscutando.Contains(setor) || setor == TipoSetor.Todos || _setoresEscutando.Contains(TipoSetor.Todos))
                {
                    var eventArgs = new PedidoNotificacaoEventArgs(notificacao, setor, itens);
                    NotificacaoRecebida?.Invoke(this, eventArgs);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao disparar evento de notificação para setor {setor}");
            }
        }

        private Task NotificarCaixa(Pedido pedido)
        {
            var notificacao = new PedidoNotificacao
            {
                Id = _notificacoes.Count + 1,
                PedidoId = pedido.Id,
                Pedido = pedido,
                SetorDestino = TipoSetor.Todos,
                TipoNotificacao = TipoNotificacao.NovoPedido,
                Titulo = $"💰 Novo Pedido - Caixa #{pedido.NumeroPedido}",
                Mensagem = $"💵 Total: R$ {pedido.Total:F2} | 👤 {pedido.NomeCliente}"
            };

            _notificacoes.Add(notificacao);

            DispararEventoNotificacao(notificacao, TipoSetor.Todos);
            return Task.CompletedTask;
        }
    }
}
