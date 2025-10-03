using Microsoft.EntityFrameworkCore;
using BeachApp_Cliente.Models;

namespace BeachApp_Cliente.Data
{
    public class BeachAppContext : DbContext
    {
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }

        public BeachAppContext(DbContextOptions<BeachAppContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração para Produto
            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nome).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Descricao).HasMaxLength(500);
                entity.Property(p => p.Preco).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Categoria).HasMaxLength(50);
            });

            // Configuração para Pedido
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.NumeroPedido).IsRequired().HasMaxLength(20);
                entity.Property(p => p.NomeCliente).IsRequired().HasMaxLength(100);
                entity.Property(p => p.TelefoneCliente).HasMaxLength(20);
                entity.Property(p => p.Mesa).HasMaxLength(50);
                entity.Property(p => p.Status).HasMaxLength(20);
                entity.Property(p => p.Total).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Observacoes).HasMaxLength(500);
                
                // Relacionamento um-para-muitos com ItemPedido
                entity.HasMany(p => p.Itens)
                      .WithOne(i => i.Pedido)
                      .HasForeignKey(i => i.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuração para ItemPedido
            modelBuilder.Entity<ItemPedido>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.PrecoUnitario).HasColumnType("decimal(18,2)");
                entity.Property(i => i.ObservacoesItem).HasMaxLength(200);
                
                // Relacionamento com Produto
                entity.HasOne(i => i.Produto)
                      .WithMany()
                      .HasForeignKey(i => i.ProdutoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Dados iniciais - produtos de exemplo
            modelBuilder.Entity<Produto>().HasData(
                new Produto
                {
                    Id = 1,
                    Nome = "Água de Coco",
                    Descricao = "Água de coco gelada e refrescante",
                    Preco = 8.00m,
                    QuantidadeEstoque = 50,
                    EstoqueMinimo = 10,
                    Categoria = "Bebidas",
                    Ativo = true,
                    DataCriacao = DateTime.Now
                },
                new Produto
                {
                    Id = 2,
                    Nome = "Caipirinha",
                    Descricao = "Caipirinha tradicional com limão",
                    Preco = 15.00m,
                    QuantidadeEstoque = 30,
                    EstoqueMinimo = 5,
                    Categoria = "Bebidas",
                    Ativo = true,
                    DataCriacao = DateTime.Now
                },
                new Produto
                {
                    Id = 3,
                    Nome = "Porção de Camarão",
                    Descricao = "Camarão empanado com molho especial",
                    Preco = 35.00m,
                    QuantidadeEstoque = 20,
                    EstoqueMinimo = 3,
                    Categoria = "Petiscos",
                    Ativo = true,
                    DataCriacao = DateTime.Now
                },
                new Produto
                {
                    Id = 4,
                    Nome = "Açaí na Tigela",
                    Descricao = "Açaí com granola, banana e mel",
                    Preco = 18.00m,
                    QuantidadeEstoque = 25,
                    EstoqueMinimo = 5,
                    Categoria = "Sobremesas",
                    Ativo = true,
                    DataCriacao = DateTime.Now
                },
                new Produto
                {
                    Id = 5,
                    Nome = "Sanduíche Natural",
                    Descricao = "Sanduíche natural com peito de peru",
                    Preco = 12.00m,
                    QuantidadeEstoque = 15,
                    EstoqueMinimo = 3,
                    Categoria = "Lanches",
                    Ativo = true,
                    DataCriacao = DateTime.Now
                }
            );
        }
    }
}
