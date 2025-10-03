using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AdmBeachApp.Models
{
    [Table("produtos")]
    public class SupabaseProduto : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }
        
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;
        
        [Column("descricao")]
        public string? Descricao { get; set; }
        
        [Column("preco")]
        public decimal Preco { get; set; }
        
        [Column("quantidade_estoque")]
        public int QuantidadeEstoque { get; set; }
        
        [Column("estoque_minimo")]
        public int EstoqueMinimo { get; set; } = 5;
        
        [Column("categoria")]
        public string? Categoria { get; set; }
        
        [Column("ativo")]
        public bool Ativo { get; set; } = true;
        
        [Column("data_criacao")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        
        [Column("data_atualizacao")]
        public DateTime? DataAtualizacao { get; set; }
        
        // Método para converter para o modelo local
        public Produto ToProduto()
        {
            return new Produto
            {
                Id = Id,
                Nome = Nome,
                Descricao = Descricao,
                Preco = Preco,
                QuantidadeEstoque = QuantidadeEstoque,
                EstoqueMinimo = EstoqueMinimo,
                Categoria = Categoria,
                Ativo = Ativo,
                DataCriacao = DataCriacao,
                DataAtualizacao = DataAtualizacao
            };
        }
        
        // Método para converter do modelo local
        public static SupabaseProduto FromProduto(Produto produto)
        {
            return new SupabaseProduto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Descricao = produto.Descricao,
                Preco = produto.Preco,
                QuantidadeEstoque = produto.QuantidadeEstoque,
                EstoqueMinimo = produto.EstoqueMinimo,
                Categoria = produto.Categoria,
                Ativo = produto.Ativo,
                DataCriacao = produto.DataCriacao,
                DataAtualizacao = produto.DataAtualizacao
            };
        }
    }
    
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
            StatusPedido status = Status switch
            {
                "Pendente" => StatusPedido.Pendente,
                "Preparando" => StatusPedido.Preparando,
                "Pronto" => StatusPedido.Pronto,
                "Entregue" => StatusPedido.Entregue,
                "Cancelado" => StatusPedido.Cancelado,
                _ => StatusPedido.Pendente
            };
            
            return new Pedido
            {
                Id = Id,
                NumeroPedido = NumeroPedido,
                NomeCliente = NomeCliente,
                TelefoneCliente = TelefoneCliente,
                Mesa = Mesa,
                Status = status,
                Total = Total,
                Observacoes = Observacoes,
                DataPedido = DataPedido
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
                Status = pedido.StatusTexto,
                Total = pedido.Total,
                Observacoes = pedido.Observacoes,
                DataPedido = pedido.DataPedido
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
