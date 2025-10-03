using System.ComponentModel.DataAnnotations;

namespace AdmBeachApp.Models
{
    public enum TipoSetor
    {
        Cozinha = 1,
        Bar = 2,
        Balcao = 3, // Para produtos que são vendidos diretamente no balcão
        Todos = 4
    }

    public enum TipoNotificacao
    {
        NovoPedido = 1,
        StatusAtualizado = 2,
        PedidoCancelado = 3,
        ItemPronto = 4
    }

    public class PedidoNotificacao
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public Pedido? Pedido { get; set; }
        
        public TipoSetor SetorDestino { get; set; }
        public TipoNotificacao TipoNotificacao { get; set; }
        
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public bool Visualizada { get; set; } = false;
        public DateTime? DataVisualizacao { get; set; }
        
        public bool Ativa { get; set; } = true;
        
        // Para identificar itens específicos do pedido
        public string? ItensPedido { get; set; } // JSON com os itens relevantes para o setor
    }

    public class ItemPedidoDetalhado
    {
        public int ItemId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public string? Observacoes { get; set; }
        public TipoSetor SetorResponsavel { get; set; }
        public StatusItemPedido Status { get; set; } = StatusItemPedido.Pendente;
        public DateTime? HoraInicio { get; set; }
        public DateTime? HoraConclusao { get; set; }
    }

    public enum StatusItemPedido
    {
        Pendente = 0,
        EmPreparo = 1,
        Pronto = 2,
        Entregue = 3
    }

    // Extensão do produto para incluir setor responsável
    public static class ProdutoExtensions
    {
        public static TipoSetor DeterminarSetor(this Produto produto)
        {
            if (produto.Categoria == null)
                return TipoSetor.Balcao;

            return produto.Categoria.ToLower() switch
            {
                "bebida" or "cerveja" or "refrigerante" or "água" or "suco" or "drink" => TipoSetor.Bar,
                "comida" or "sanduíche" or "lanche" or "petisco" or "porção" or "prato" => TipoSetor.Cozinha,
                "protetor solar" or "acessório" or "diversos" => TipoSetor.Balcao,
                _ => TipoSetor.Balcao
            };
        }

        public static string ObterIconeSetor(this TipoSetor setor)
        {
            return setor switch
            {
                TipoSetor.Cozinha => "🍳",
                TipoSetor.Bar => "🍺",
                TipoSetor.Balcao => "🛒",
                TipoSetor.Todos => "📢",
                _ => "❓"
            };
        }

        public static string ObterCorSetor(this TipoSetor setor)
        {
            return setor switch
            {
                TipoSetor.Cozinha => "warning", // Amarelo
                TipoSetor.Bar => "info", // Azul
                TipoSetor.Balcao => "success", // Verde
                TipoSetor.Todos => "primary", // Azul primário
                _ => "secondary"
            };
        }
    }
}
