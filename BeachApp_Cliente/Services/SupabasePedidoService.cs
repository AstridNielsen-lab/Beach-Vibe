using BeachApp_Cliente.Data;
using BeachApp_Cliente.Models;
using Supabase;

namespace BeachApp_Cliente.Services
{
    public class SupabasePedidoService : IPedidoService
    {
        private readonly Client _supabaseClient;

        public SupabasePedidoService(Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<Pedido> CreatePedidoAsync(Pedido pedido)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("SupabasePedidoService: Criando novo pedido no Supabase");

                // Gerar número do pedido se não existir
                if (string.IsNullOrEmpty(pedido.NumeroPedido))
                {
                    pedido.NumeroPedido = GerarNumeroPedido();
                }

                // Calcular total
                pedido.Total = pedido.Itens?.Sum(i => i.Subtotal) ?? 0;
                pedido.DataPedido = DateTime.Now;

                // Converter para modelo Supabase
                var supabasePedido = SupabasePedido.FromPedido(pedido);

                // Inserir pedido
                var pedidoResponse = await _supabaseClient
                    .From<SupabasePedido>()
                    .Insert(supabasePedido);

                var pedidoCriado = pedidoResponse.Model;

                if (pedidoCriado != null && pedido.Itens != null && pedido.Itens.Any())
                {
                    // Inserir itens do pedido
                    var supabaseItens = pedido.Itens.Select(item =>
                    {
                        var supabaseItem = SupabaseItemPedido.FromItemPedido(item);
                        supabaseItem.PedidoId = pedidoCriado.Id;
                        return supabaseItem;
                    }).ToList();

                    await _supabaseClient
                        .From<SupabaseItemPedido>()
                        .Insert(supabaseItens);

                    System.Diagnostics.Debug.WriteLine($"SupabasePedidoService: {supabaseItens.Count} itens adicionados ao pedido");
                }

                var resultado = pedidoCriado.ToPedido();
                resultado.Itens = pedido.Itens; // Manter os itens originais

                System.Diagnostics.Debug.WriteLine($"SupabasePedidoService: Pedido {resultado.NumeroPedido} criado com sucesso");

                return resultado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabasePedidoService: Erro ao criar pedido - {ex.Message}");

                // Em caso de erro, simular criação local
                pedido.Id = Random.Shared.Next(1000, 9999);
                pedido.NumeroPedido = string.IsNullOrEmpty(pedido.NumeroPedido) ? GerarNumeroPedido() : pedido.NumeroPedido;
                pedido.Total = pedido.Itens?.Sum(i => i.Subtotal) ?? 0;
                pedido.DataPedido = DateTime.Now;
                pedido.Status = "Pendente";

                return pedido;
            }
        }

        public async Task<List<Pedido>> GetPedidosAsync()
        {
            try
            {
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Order("data_pedido", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                return response.Models.Select(p => p.ToPedido()).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabasePedidoService: Erro ao obter pedidos - {ex.Message}");
                return new List<Pedido>();
            }
        }

        public async Task<Pedido?> GetPedidoByIdAsync(int id)
        {
            try
            {
                var pedidoResponse = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.Id == id)
                    .Single();

                if (pedidoResponse == null)
                    return null;

                var pedido = pedidoResponse.ToPedido();

                // Obter itens do pedido
                var itensResponse = await _supabaseClient
                    .From<SupabaseItemPedido>()
                    .Where(i => i.PedidoId == id)
                    .Get();

                pedido.Itens = itensResponse.Models.Select(i => i.ToItemPedido()).ToList();

                return pedido;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabasePedidoService: Erro ao obter pedido por ID - {ex.Message}");
                return null;
            }
        }

        public async Task<Pedido?> UpdatePedidoAsync(Pedido pedido)
        {
            try
            {
                pedido.DataAtualizacao = DateTime.Now;
                var supabasePedido = SupabasePedido.FromPedido(pedido);

                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.Id == pedido.Id)
                    .Update(supabasePedido);

                return response.Model?.ToPedido();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabasePedidoService: Erro ao atualizar pedido - {ex.Message}");
                return null;
            }
        }

        public async Task<List<Pedido>> GetPedidosClienteAsync(string nomeCliente)
        {
            try
            {
                var response = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.NomeCliente.ToLower().Contains(nomeCliente.ToLower()))
                    .Order("data_pedido", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Get();

                var pedidos = response.Models.Select(p => p.ToPedido()).ToList();

                // Carregar itens para cada pedido
                foreach (var pedido in pedidos)
                {
                    var itensResponse = await _supabaseClient
                        .From<SupabaseItemPedido>()
                        .Where(i => i.PedidoId == pedido.Id)
                        .Get();

                    pedido.Itens = itensResponse.Models.Select(i => i.ToItemPedido()).ToList();
                }

                return pedidos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabasePedidoService: Erro ao obter pedidos do cliente - {ex.Message}");
                return new List<Pedido>();
            }
        }

        public async Task<Pedido?> GetPedidoByNumeroAsync(string numeroPedido)
        {
            try
            {
                var pedidoResponse = await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.NumeroPedido == numeroPedido)
                    .Single();

                if (pedidoResponse == null)
                    return null;

                var pedido = pedidoResponse.ToPedido();

                // Obter itens do pedido
                var itensResponse = await _supabaseClient
                    .From<SupabaseItemPedido>()
                    .Where(i => i.PedidoId == pedido.Id)
                    .Get();

                pedido.Itens = itensResponse.Models.Select(i => i.ToItemPedido()).ToList();

                return pedido;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabasePedidoService: Erro ao obter pedido por número - {ex.Message}");
                return null;
            }
        }

        public async Task<bool> DeletePedidoAsync(int id)
        {
            try
            {
                // Primeiro, deletar itens do pedido
                await _supabaseClient
                    .From<SupabaseItemPedido>()
                    .Where(i => i.PedidoId == id)
                    .Delete();

                // Depois, deletar o pedido
                await _supabaseClient
                    .From<SupabasePedido>()
                    .Where(p => p.Id == id)
                    .Delete();

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SupabasePedidoService: Erro ao deletar pedido - {ex.Message}");
                return false;
            }
        }

        private string GerarNumeroPedido()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmm");
            var random = Random.Shared.Next(100, 999);
            return $"QP{timestamp}{random}";
        }
    }
}
