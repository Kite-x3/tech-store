using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechStore.Domain.Entities;

namespace TechStore.Infrastracture.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

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
        }
    }
}
