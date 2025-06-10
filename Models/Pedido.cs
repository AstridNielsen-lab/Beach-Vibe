using System.ComponentModel.DataAnnotations;

namespace AdmBeachApp.Models
{
    public enum StatusPedido
    {
        Pendente = 0,
        Preparando = 1,
        Pronto = 2,
        Entregue = 3,
        Cancelado = 4
    }
    
    public class Pedido
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Número do pedido é obrigatório")]
        public string NumeroPedido { get; set; } = string.Empty;
        
        public DateTime DataPedido { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Nome do cliente é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string NomeCliente { get; set; } = string.Empty;
        
        [StringLength(15, ErrorMessage = "Telefone deve ter no máximo 15 caracteres")]
        public string? TelefoneCliente { get; set; }
        
        [StringLength(10, ErrorMessage = "Mesa deve ter no máximo 10 caracteres")]
        public string? Mesa { get; set; }
        
        [Required(ErrorMessage = "Total é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total deve ser maior que zero")]
        public decimal Total { get; set; }
        
        public StatusPedido Status { get; set; } = StatusPedido.Pendente;
        
        public string? Observacoes { get; set; }
        
        public DateTime? DataEntrega { get; set; }
        
        // Lista de itens do pedido
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
        
        // Propriedades calculadas
        public int QuantidadeTotalItens => Itens.Sum(i => i.Quantidade);
        
        public string StatusTexto => Status switch
        {
            StatusPedido.Pendente => "Pendente",
            StatusPedido.Preparando => "Preparando",
            StatusPedido.Pronto => "Pronto",
            StatusPedido.Entregue => "Entregue",
            StatusPedido.Cancelado => "Cancelado",
            _ => "Desconhecido"
        };
        
        public string StatusCor => Status switch
        {
            StatusPedido.Pendente => "warning",
            StatusPedido.Preparando => "info",
            StatusPedido.Pronto => "success",
            StatusPedido.Entregue => "secondary",
            StatusPedido.Cancelado => "danger",
            _ => "dark"
        };
        
        public string TempoEspera
        {
            get
            {
                var agora = DateTime.Now;
                var diferenca = agora - DataPedido;
                
                if (diferenca.TotalMinutes < 1)
                    return "Agora";
                else if (diferenca.TotalMinutes < 60)
                    return $"{(int)diferenca.TotalMinutes}min";
                else
                    return $"{(int)diferenca.TotalHours}h {(int)(diferenca.TotalMinutes % 60)}min";
            }
        }
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

