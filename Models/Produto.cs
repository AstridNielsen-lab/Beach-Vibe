using System.ComponentModel.DataAnnotations;

namespace AdmBeachApp.Models
{
    public class Produto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string? Descricao { get; set; }
        
        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Preco { get; set; }
        
        [Required(ErrorMessage = "Quantidade em estoque é obrigatória")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade não pode ser negativa")]
        public int QuantidadeEstoque { get; set; }
        
        [Range(0, int.MaxValue, ErrorMessage = "Estoque mínimo não pode ser negativo")]
        public int EstoqueMinimo { get; set; } = 5;
        
        public string? Categoria { get; set; }
        
        public bool Ativo { get; set; } = true;
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        
        public DateTime? DataAtualizacao { get; set; }
        
        // Propriedade calculada para verificar se está em estoque baixo
        public bool EstoqueBaixo => QuantidadeEstoque <= EstoqueMinimo;
    }
}

