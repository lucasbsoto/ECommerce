using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração da Sale
            modelBuilder.Entity<Sale>().HasKey(s => s.Identifier);

            // Configuração de precisão decimal (requisito)
            modelBuilder.Entity<Sale>().Property(s => s.DiscountAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Sale>().Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Sale>().Property(s => s.FinalAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Sale>()
                .Property(s => s.Status)
                .HasConversion<byte>();

            // 1. RELACIONAMENTO CLIENTE -> VENDA (One-to-Many)
            modelBuilder.Entity<Sale>()
                // Uma Venda pertence a Um Cliente (a foreign key CustomerId está aqui)
                .HasOne(s => s.Customer)
                // Um Cliente pode ter muitas Vendas (opcional se não tiver a lista 'Sales' em Customer)
                .WithMany(c => c.Sales)
                //.WithMany()
                // Usa a propriedade CustomerId como chave estrangeira
                .HasForeignKey(s => s.CustomerId)
                .IsRequired();

            // --- Configuração da Entidade SaleItem ---
            // Relacionamento Venda -> Itens (One-to-Many)
            modelBuilder.Entity<Sale>()
                .HasMany(s => s.Items)
                .WithOne() // Se SaleItem não tiver uma propriedade de navegação para Sale
                .HasForeignKey(i => i.SaleIdentifier) // A FK SaleIdentifier está em SaleItem
                .IsRequired();

            // Configuração de precisão decimal para SaleItem
            modelBuilder.Entity<SaleItem>().HasKey(i => i.ProductId);
            modelBuilder.Entity<SaleItem>().Property(i => i.Quantity).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<SaleItem>().Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        }
    }
}