using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations;

namespace BeachApp_Cliente.Models
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
        
        // Propriedade calculada para verificar se está em estoque baixo
        public bool EstoqueBaixo => QuantidadeEstoque <= EstoqueMinimo;
        
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
}
