using System.ComponentModel.DataAnnotations;

namespace BeachApp_Cliente.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        
        public string NumeroPedido { get; set; } = string.Empty;
        
        public DateTime DataPedido { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        public string NomeCliente { get; set; } = string.Empty;
        
        public string? TelefoneCliente { get; set; }
        
        public string? Mesa { get; set; }
        
        public string? Observacoes { get; set; }
        
        public string Status { get; set; } = "Pendente";
        
        public decimal Total { get; set; }
        
        public DateTime? DataAtualizacao { get; set; }
        
        // Lista de itens do pedido
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
        
        // Propriedade calculada para a quantidade total de itens
        public int QuantidadeTotalItens => Itens.Sum(i => i.Quantidade);
    }
    
    public class ItemPedido
    {
        public int Id { get; set; }
        
        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }
        
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }
        
        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }
        
        [Required(ErrorMessage = "Preço unitário é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço unitário deve ser maior que zero")]
        public decimal PrecoUnitario { get; set; }
        
        public string? ObservacoesItem { get; set; }
        
        // Propriedade calculada para o subtotal
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }
}
