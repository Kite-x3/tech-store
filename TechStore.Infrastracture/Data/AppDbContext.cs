using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechStore.Domain.Entities;

namespace TechStore.Infrastracture.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Первичный ключ для Category
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)  // Один продукт принадлежит одной категории
                .WithMany(c => c.Products) // Одна категория содержит много продуктов
                .HasForeignKey(p => p.CategoryId) // Внешний ключ в таблице Product
                .OnDelete(DeleteBehavior.Restrict); // Удаление категории запрещено, если есть связанные продукты

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.ImageUrlsJson)
                    .HasDefaultValue("[]") // Значение по умолчанию
                    .IsRequired(); // Обязательное поле
            });

            modelBuilder.Entity<Review>()
                .HasKey(r => r.ReviewId);

            modelBuilder.Entity<Review>()
               .HasOne(r => r.Product)
               .WithMany(p => p.Reviews)
               .HasForeignKey(r => r.ProductId)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasKey(c => c.CartId);

            modelBuilder.Entity<CartItem>()
                .HasKey(ci => ci.CartItemId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.Items)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId);
            
            modelBuilder.Entity<OrderStatus>().HasData(
            new OrderStatus { OrderStatusId = 1, Name = "Pending", Description = "Order is pending processing" },
            new OrderStatus { OrderStatusId = 2, Name = "Processing", Description = "Order is being processed" },
            new OrderStatus { OrderStatusId = 3, Name = "Shipped", Description = "Order has been shipped" },
            new OrderStatus { OrderStatusId = 4, Name = "Delivered", Description = "Order has been delivered" },
            new OrderStatus { OrderStatusId = 5, Name = "Cancelled", Description = "Order has been cancelled" }
            );

            modelBuilder.Entity<Order>()
        .HasKey(o => o.OrderId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Status)  // Каждый заказ имеет один статус
                .WithMany()             // Один статус может быть у многих заказов
                .HasForeignKey(o => o.OrderStatusId)
                .OnDelete(DeleteBehavior.Restrict); // Запрещаем удаление статуса, если он используется в заказах

            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => oi.OrderItemId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);
        }
    }
}
