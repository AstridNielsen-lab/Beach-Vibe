using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace BeachApp_Cliente.Models
{
    [Table("pedidos")]
    public class SupabasePedido : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
        
        [Column("numero_pedido")]
        public string NumeroPedido { get; set; } = string.Empty;
        
        [Column("nome_cliente")]
        public string NomeCliente { get; set; } = string.Empty;
        
        [Column("telefone_cliente")]
        public string? TelefoneCliente { get; set; }
        
        [Column("mesa")]
        public string? Mesa { get; set; }
        
        [Column("status")]
        public string Status { get; set; } = "Pendente";
        
        [Column("total")]
        public decimal Total { get; set; }
        
        [Column("observacoes")]
        public string? Observacoes { get; set; }
        
        [Column("data_pedido")]
        public DateTime DataPedido { get; set; } = DateTime.Now;
        
        [Column("data_atualizacao")]
        public DateTime? DataAtualizacao { get; set; }
        
        // Método para converter para o modelo local
        public Pedido ToPedido()
        {
            return new Pedido
            {
                Id = Id,
                NumeroPedido = NumeroPedido,
                NomeCliente = NomeCliente,
                TelefoneCliente = TelefoneCliente,
                Mesa = Mesa,
                Status = Status,
                Total = Total,
                Observacoes = Observacoes,
                DataPedido = DataPedido,
                DataAtualizacao = DataAtualizacao
            };
        }
        
        // Método para converter do modelo local
        public static SupabasePedido FromPedido(Pedido pedido)
        {
            return new SupabasePedido
            {
                Id = pedido.Id,
                NumeroPedido = pedido.NumeroPedido,
                NomeCliente = pedido.NomeCliente,
                TelefoneCliente = pedido.TelefoneCliente,
                Mesa = pedido.Mesa,
                Status = pedido.Status,
                Total = pedido.Total,
                Observacoes = pedido.Observacoes,
                DataPedido = pedido.DataPedido,
                DataAtualizacao = pedido.DataAtualizacao
            };
        }
    }
    
    [Table("itens_pedido")]
    public class SupabaseItemPedido : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
        
        [Column("pedido_id")]
        public int PedidoId { get; set; }
        
        [Column("produto_id")]
        public int ProdutoId { get; set; }
        
        [Column("quantidade")]
        public int Quantidade { get; set; }
        
        [Column("preco_unitario")]
        public decimal PrecoUnitario { get; set; }
        
        [Column("observacoes_item")]
        public string? ObservacoesItem { get; set; }
        
        // Propriedade calculada
        public decimal Subtotal => Quantidade * PrecoUnitario;
        
        // Método para converter para o modelo local
        public ItemPedido ToItemPedido()
        {
            return new ItemPedido
            {
                Id = Id,
                PedidoId = PedidoId,
                ProdutoId = ProdutoId,
                Quantidade = Quantidade,
                PrecoUnitario = PrecoUnitario,
                ObservacoesItem = ObservacoesItem
            };
        }
        
        // Método para converter do modelo local
        public static SupabaseItemPedido FromItemPedido(ItemPedido item)
        {
            return new SupabaseItemPedido
            {
                Id = item.Id,
                PedidoId = item.PedidoId,
                ProdutoId = item.ProdutoId,
                Quantidade = item.Quantidade,
                PrecoUnitario = item.PrecoUnitario,
                ObservacoesItem = item.ObservacoesItem
            };
        }
    }
}
