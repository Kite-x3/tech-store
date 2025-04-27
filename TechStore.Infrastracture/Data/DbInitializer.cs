using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechStore.Domain.Entities;
using TechStore.Infrastracture.Data;

namespace TechStore.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.Migrate();
            context.Database.EnsureCreated();

            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            if (!userManager.Users.Any())
            {
                var adminUser = new User { UserName = "admin", Email = "admin@gmail.com" };
                var userCreationResult = await userManager.CreateAsync(adminUser, "Admin250!");
                if (userCreationResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "admin");
                }

                var normalUser = new User { UserName = "user", Email = "user@gmail.com" };
                var normalUserCreationResult = await userManager.CreateAsync(normalUser, "User@123");
                if (normalUserCreationResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "user");
                }
            }
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Ноутбуки", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Смартфоны", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Аксессуары", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                var products = new List<Product>
                {
                    new Product { Name = "MacBook Pro 16", Description = "Мощный ноутбук Apple", Price = 250000, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, CategoryId = categories[0].CategoryId },
                    new Product { Name = "Dell XPS 15", Description = "Премиальный ноутбук", Price = 180000, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, CategoryId = categories[0].CategoryId },

                    new Product { Name = "iPhone 14 Pro", Description = "Флагман Apple", Price = 120000, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, CategoryId = categories[1].CategoryId },
                    new Product { Name = "Samsung Galaxy S23", Description = "Флагман Samsung", Price = 110000, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, CategoryId = categories[1].CategoryId },

                    new Product { Name = "Apple AirPods Pro", Description = "Беспроводные наушники", Price = 20000, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, CategoryId = categories[2].CategoryId },
                    new Product { Name = "Samsung Galaxy Watch 5", Description = "Умные часы", Price = 30000, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, CategoryId = categories[2].CategoryId }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
            if (!context.Reviews.Any())
            {
                var products = await context.Products.ToListAsync();
                var users = await userManager.Users.ToListAsync();

                var reviews = new List<Review>
                {
                    new Review {
                        Author = users[0].UserName,
                        Rating = 5,
                        Comment = "Отличный ноутбук! Быстрый и удобный.",
                        Date = DateTime.UtcNow.AddDays(-10),
                        ProductId = products[0].ProductId
                    },
                    new Review {
                        Author = users[1].UserName,
                        Rating = 4,
                        Comment = "Хороший ноутбук, но дорогой.",
                        Date = DateTime.UtcNow.AddDays(-5),
                        ProductId = products[0].ProductId
                    },
                    new Review {
                        Author = "Аноним",
                        Rating = 3,
                        Comment = "Не впечатлил. Есть более дешевые альтернативы.",
                        Date = DateTime.UtcNow.AddDays(-2),
                        ProductId = products[1].ProductId
                    },
                    new Review {
                        Author = users[0].UserName,
                        Rating = 5,
                        Comment = "Лучший смартфон на рынке!",
                        Date = DateTime.UtcNow.AddDays(-7),
                        ProductId = products[2].ProductId
                    },
                    new Review {
                        Author = users[1].UserName,
                        Rating = 4,
                        Comment = "Хорошая камера, но батарея держит недолго.",
                        Date = DateTime.UtcNow.AddDays(-3),
                        ProductId = products[3].ProductId
                    },
                    new Review {
                        Author = "Тестовый пользователь",
                        Rating = 5,
                        Comment = "Наушники просто супер! Шумоподавление на высоте.",
                        Date = DateTime.UtcNow.AddDays(-1),
                        ProductId = products[4].ProductId
                    }
                };

                await context.Reviews.AddRangeAsync(reviews);
                await context.SaveChangesAsync();
            }

        }
    }
}
