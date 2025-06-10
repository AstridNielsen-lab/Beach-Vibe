using System.ComponentModel.DataAnnotations;

namespace AdmBeachApp.Models
{
    public class Venda
    {
        public int Id { get; set; }
        
        public DateTime DataVenda { get; set; } = DateTime.Now;
        
        [Required(ErrorMessage = "Total é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total deve ser maior que zero")]
        public decimal Total { get; set; }
        
        public string? Observacoes { get; set; }
        
        public string? ClienteNome { get; set; }
        
        public string MetodoPagamento { get; set; } = "Dinheiro";
        
        // Lista de itens da venda
        public List<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
        
        // Propriedade calculada para a quantidade total de itens
        public int QuantidadeTotalItens => Itens.Sum(i => i.Quantidade);
    }
    
    public class ItemVenda
    {
        public int Id { get; set; }
        
        public int VendaId { get; set; }
        public Venda? Venda { get; set; }
        
        public int ProdutoId { get; set; }
        public Produto? Produto { get; set; }
        
        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }
        
        [Required(ErrorMessage = "Preço unitário é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço unitário deve ser maior que zero")]
        public decimal PrecoUnitario { get; set; }
        
        // Propriedade calculada para o subtotal
        public decimal Subtotal => Quantidade * PrecoUnitario;
    }
}

