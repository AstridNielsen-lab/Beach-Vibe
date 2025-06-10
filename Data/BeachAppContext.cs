using Microsoft.EntityFrameworkCore;
using AdmBeachApp.Models;

namespace AdmBeachApp.Data
{
    public class BeachAppContext : DbContext
    {
        public BeachAppContext(DbContextOptions<BeachAppContext> options) : base(options)
        {
        }
        
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<ItemPedido> ItensPedido { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var dbPath = Path.Combine(FileSystem.AppDataDirectory, "beachapp.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configurações para Produto
            modelBuilder.Entity<Produto>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Nome).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Descricao).HasMaxLength(500);
                entity.Property(p => p.Preco).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Categoria).HasMaxLength(50);
            });
            
            // Configurações para Venda
            modelBuilder.Entity<Venda>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.Total).HasColumnType("decimal(18,2)");
                entity.Property(v => v.ClienteNome).HasMaxLength(100);
                entity.Property(v => v.MetodoPagamento).HasMaxLength(50);
                entity.Property(v => v.Observacoes).HasMaxLength(1000);
                
                // Relacionamento com ItemVenda
                entity.HasMany(v => v.Itens)
                      .WithOne(i => i.Venda)
                      .HasForeignKey(i => i.VendaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Configurações para ItemVenda
            modelBuilder.Entity<ItemVenda>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.PrecoUnitario).HasColumnType("decimal(18,2)");
                
                // Relacionamento com Produto
                entity.HasOne(i => i.Produto)
                      .WithMany()
                      .HasForeignKey(i => i.ProdutoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Configurações para Pedido
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.NumeroPedido).IsRequired().HasMaxLength(20);
                entity.Property(p => p.NomeCliente).IsRequired().HasMaxLength(100);
                entity.Property(p => p.TelefoneCliente).HasMaxLength(15);
                entity.Property(p => p.Mesa).HasMaxLength(10);
                entity.Property(p => p.Total).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Observacoes).HasMaxLength(1000);
                
                // Relacionamento com ItemPedido
                entity.HasMany(p => p.Itens)
                      .WithOne(i => i.Pedido)
                      .HasForeignKey(i => i.PedidoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            
            // Configurações para ItemPedido
            modelBuilder.Entity<ItemPedido>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.PrecoUnitario).HasColumnType("decimal(18,2)");
                entity.Property(i => i.ObservacoesItem).HasMaxLength(500);
                
                // Relacionamento com Produto
                entity.HasOne(i => i.Produto)
                      .WithMany()
                      .HasForeignKey(i => i.ProdutoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Dados iniciais (seed data)
            SeedData(modelBuilder);
        }
        
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Produtos iniciais
            modelBuilder.Entity<Produto>().HasData(
                new Produto
                {
                    Id = 1,
                    Nome = "Água Mineral 500ml",
                    Descricao = "Água mineral natural",
                    Preco = 3.50m,
                    QuantidadeEstoque = 100,
                    EstoqueMinimo = 20,
                    Categoria = "Bebidas",
                    DataCriacao = DateTime.Now
                },
                new Produto
                {
                    Id = 2,
                    Nome = "Refrigerante Lata 350ml",
                    Descricao = "Refrigerante gelado",
                    Preco = 5.00m,
                    QuantidadeEstoque = 80,
                    EstoqueMinimo = 15,
                    Categoria = "Bebidas",
                    DataCriacao = DateTime.Now
                },
                new Produto
                {
                    Id = 3,
                    Nome = "Sanduíche Natural",
                    Descricao = "Sanduíche com ingredientes frescos",
                    Preco = 12.00m,
                    QuantidadeEstoque = 30,
                    EstoqueMinimo = 5,
                    Categoria = "Alimentação",
                    DataCriacao = DateTime.Now
                },
                new Produto
                {
                    Id = 4,
                    Nome = "Protetor Solar FPS 60",
                    Descricao = "Proteção solar para toda família",
                    Preco = 25.00m,
                    QuantidadeEstoque = 15,
                    EstoqueMinimo = 3,
                    Categoria = "Cuidados Pessoais",
                    DataCriacao = DateTime.Now
                },
                new Produto
                {
                    Id = 5,
                    Nome = "Cerveja Long Neck",
                    Descricao = "Cerveja gelada 330ml",
                    Preco = 8.00m,
                    QuantidadeEstoque = 60,
                    EstoqueMinimo = 12,
                    Categoria = "Bebidas Alcoólicas",
                    DataCriacao = DateTime.Now
                }
            );
        }
    }
}

