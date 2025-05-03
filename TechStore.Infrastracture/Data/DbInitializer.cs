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
                var testUsers = new List<User>
                {
                    new User { UserName = "tech_expert", Email = "expert@example.com" },
                    new User { UserName = "gadget_lover", Email = "gadget@example.com" }
                };

                foreach (var user in testUsers)
                {
                    await userManager.CreateAsync(user, "TestPass123!");
                    await userManager.AddToRoleAsync(user, "user");
                }
            }
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Ноутбуки", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Смартфоны", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Планшеты", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Наушники", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Умные часы", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                    new Category { Name = "Аксессуары", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                var products = new List<Product>
                {
                    // Ноутбуки (6 товаров)
                    new Product {
                        Name = "Apple MacBook Pro 16 M3 Pro",
                        Description = "Ноутбук с дисплеем Liquid Retina XDR, чипом M3 Pro, 36GB RAM, 1TB SSD",
                        Price = 329990,
                        ImageUrls = new List<string> { "/images/macbook_pro_16.jpg" },
                        CategoryId = categories[0].CategoryId
                    },
                    new Product {
                        Name = "Dell XPS 15 9530",
                        Description = "Ноутбук с 15.6\" 4K OLED, Intel Core i9-13900H, 32GB RAM, RTX 4070, 1TB SSD",
                        Price = 289990,
                        ImageUrls = new List<string> { "/images/dell_xps_15.jpg" },
                        CategoryId = categories[0].CategoryId
                    },
                    new Product {
                        Name = "ASUS ROG Strix G18",
                        Description = "Игровой ноутбук 18\" QHD+ 240Hz, Intel i9-13980HX, RTX 4080, 32GB RAM, 1TB SSD",
                        Price = 279990,
                        ImageUrls = new List<string> { "/images/asus_rog_strix.jpg" },
                        CategoryId = categories[0].CategoryId
                    },
                    new Product {
                        Name = "Lenovo ThinkPad X1 Carbon Gen 11",
                        Description = "Бизнес-ноутбук 14\" WUXGA, Intel Core i7-1365U, 16GB RAM, 512GB SSD",
                        Price = 189990,
                        ImageUrls = new List<string> { "/images/thinkpad_x1.jpg" },
                        CategoryId = categories[0].CategoryId
                    },
                    new Product {
                        Name = "HP Spectre x360 14",
                        Description = "Премиум 2-в-1 ноутбук 14\" 3K2K OLED, Intel Core i7-1360P, 16GB RAM, 1TB SSD",
                        Price = 199990,
                        ImageUrls = new List<string> { "/images/hp_spectre.jpg" },
                        CategoryId = categories[0].CategoryId
                    },
                    new Product {
                        Name = "Acer Predator Helios 16",
                        Description = "Игровой ноутбук 16\" WQXGA 240Hz, Intel i9-13900HX, RTX 4080, 32GB RAM, 1TB SSD",
                        Price = 269990,
                        ImageUrls = new List<string> { "/images/acer_predator.jpg" },
                        CategoryId = categories[0].CategoryId
                    },

                    // Смартфоны 
                    new Product {
                        Name = "iPhone 15 Pro Max 256GB",
                        Description = "Смартфон с 6.7\" Super Retina XDR, титановым корпусом, A17 Pro, камерой 48MP",
                        Price = 149990,
                        ImageUrls = new List<string> { "/images/iphone_15_pro_max.jpg" },
                        CategoryId = categories[1].CategoryId
                    },
                    new Product {
                        Name = "Samsung Galaxy S24 Ultra 12/512GB",
                        Description = "Смартфон с 6.8\" Dynamic AMOLED 2X, Snapdragon 8 Gen 3, S-Pen, камерой 200MP",
                        Price = 139990,
                        ImageUrls = new List<string> { "/images/samsung_s24_ultra.jpg" },
                        CategoryId = categories[1].CategoryId
                    },
                    new Product {
                        Name = "Xiaomi 14 Pro 12/512GB",
                        Description = "Смартфон с 6.73\" AMOLED, Snapdragon 8 Gen 3, камерой Leica 50MP",
                        Price = 89990,
                        ImageUrls = new List<string> { "/images/xiaomi_14_pro.jpg" },
                        CategoryId = categories[1].CategoryId
                    },
                    new Product {
                        Name = "Google Pixel 8 Pro 12/256GB",
                        Description = "Смартфон с 6.7\" OLED, Tensor G3, камерой 50MP с ИИ-обработкой",
                        Price = 99990,
                        ImageUrls = new List<string> { "/images/pixel_8_pro.jpg" },
                        CategoryId = categories[1].CategoryId
                    },
                    new Product {
                        Name = "OnePlus 12 16/512GB",
                        Description = "Смартфон с 6.82\" AMOLED, Snapdragon 8 Gen 3, 100W зарядкой",
                        Price = 79990,
                        ImageUrls = new List<string> { "/images/oneplus_12.jpg" },
                        CategoryId = categories[1].CategoryId
                    },
                    new Product {
                        Name = "Honor Magic6 Pro 12/512GB",
                        Description = "Смартфон с 6.8\" OLED, Snapdragon 8 Gen 3, камерой 180MP",
                        Price = 84990,
                        ImageUrls = new List<string> { "/images/honor_magic6_pro.jpg" },
                        CategoryId = categories[1].CategoryId
                    },

                    // Планшеты 
                    new Product {
                        Name = "iPad Pro 12.9\" M2 1TB",
                        Description = "Планшет с mini-LED дисплеем, чипом M2, поддержкой Apple Pencil",
                        Price = 149990,
                        ImageUrls = new List<string> { "/images/ipad_pro_12.jpg" },
                        CategoryId = categories[2].CategoryId
                    },
                    new Product {
                        Name = "Samsung Galaxy Tab S9 Ultra 16/1TB",
                        Description = "Планшет с 14.6\" AMOLED, S-Pen, Snapdragon 8 Gen 2",
                        Price = 129990,
                        ImageUrls = new List<string> { "/images/samsung_tab_s9.jpg" },
                        CategoryId = categories[2].CategoryId
                    },
                    new Product {
                        Name = "Xiaomi Pad 6 Max 14 12/512GB",
                        Description = "Планшет с 14\" LCD, Snapdragon 8+ Gen 1, стилусом",
                        Price = 69990,
                        ImageUrls = new List<string> { "/images/xiaomi_pad_6.jpg" },
                        CategoryId = categories[2].CategoryId
                    },
                    new Product {
                        Name = "Huawei MatePad Pro 13.2\" 12/512GB",
                        Description = "Планшет с OLED дисплеем, процессором Kirin 9000s, стилусом M-Pencil",
                        Price = 89990,
                        ImageUrls = new List<string> { "/images/huawei_matepad.jpg" },
                        CategoryId = categories[2].CategoryId
                    },

                    // Наушники 
                    new Product {
                        Name = "Apple AirPods Pro 2 (USB-C)",
                        Description = "Наушники с активным шумоподавлением, пространственным аудио",
                        Price = 24990,
                        ImageUrls = new List<string> { "/images/airpods_pro_2.jpg" },
                        CategoryId = categories[3].CategoryId
                    },
                    new Product {
                        Name = "Sony WH-1000XM5",
                        Description = "Наушники с продвинутым шумоподавлением, 30ч работы",
                        Price = 34990,
                        ImageUrls = new List<string> { "/images/sony_xm5.jpg" },
                        CategoryId = categories[3].CategoryId
                    },
                    new Product {
                        Name = "Bose QuietComfort Ultra",
                        Description = "Наушники с иммерсивным звуком и шумоподавлением",
                        Price = 37990,
                        ImageUrls = new List<string> { "/images/bose_qc_ultra.jpg" },
                        CategoryId = categories[3].CategoryId
                    },
                    new Product {
                        Name = "Sennheiser Momentum 4",
                        Description = "Наушники с Hi-Res звуком, автономностью 60ч",
                        Price = 29990,
                        ImageUrls = new List<string> { "/images/sennheiser_momentum.jpg" },
                        CategoryId = categories[3].CategoryId
                    },

                    // Умные часы 
                    new Product {
                        Name = "Apple Watch Ultra 2",
                        Description = "Умные часы с дисплеем 49mm, трекингом для спорта",
                        Price = 59990,
                        ImageUrls = new List<string> { "/images/apple_watch_ultra.jpg" },
                        CategoryId = categories[4].CategoryId
                    },
                    new Product {
                        Name = "Samsung Galaxy Watch6 Classic 47mm",
                        Description = "Умные часы с вращающимся безелем, Wear OS",
                        Price = 39990,
                        ImageUrls = new List<string> { "/images/samsung_watch6.jpg" },
                        CategoryId = categories[4].CategoryId
                    },
                    new Product {
                        Name = "Garmin Epix Pro (Gen 2) 51mm",
                        Description = "Умные часы для спорта с AMOLED дисплеем",
                        Price = 89990,
                        ImageUrls = new List<string> { "/images/garmin_epix.jpg" },
                        CategoryId = categories[4].CategoryId
                    },
                    new Product {
                        Name = "Xiaomi Watch 2 Pro",
                        Description = "Умные часы с Wear OS, Snapdragon W5+ Gen 1",
                        Price = 24990,
                        ImageUrls = new List<string> { "/images/xiaomi_watch_2.jpg" },
                        CategoryId = categories[4].CategoryId
                    },

                    // Аксессуары 
                    new Product {
                        Name = "Чехол Apple iPhone 15 Pro Silicone Case",
                        Description = "Оригинальный силиконовый чехол MagSafe",
                        Price = 4990,
                        ImageUrls = new List<string> { "/images/iphone_case.jpg" },
                        CategoryId = categories[5].CategoryId
                    },
                    new Product {
                        Name = "Рюкзак Lenovo Legion Recon",
                        Description = "Рюкзак для ноутбука до 16\" с защитой от дождя",
                        Price = 7990,
                        ImageUrls = new List<string> { "/images/lenovo_backpack.jpg" },
                        CategoryId = categories[5].CategoryId
                    },
                    new Product {
                        Name = "Док-станция CalDigit TS4",
                        Description = "Док-станция с 18 портами для Mac и PC",
                        Price = 34990,
                        ImageUrls = new List<string> { "/images/caldigit_dock.jpg" },
                        CategoryId = categories[5].CategoryId
                    },
                    new Product {
                        Name = "Карта памяти SanDisk Extreme 1TB",
                        Description = "Карта памяти UHS-I U3 V30 A2 160MB/s",
                        Price = 9990,
                        ImageUrls = new List<string> { "/images/sandisk_card.jpg" },
                        CategoryId = categories[5].CategoryId
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }

            if (!context.Reviews.Any())
            {
                var products = await context.Products.ToListAsync();
                var users = await userManager.Users.ToListAsync();
                var random = new Random();

                var reviews = new List<Review>();

                foreach (var product in products)
                {
                    // От 0 до 3 отзывов на товар
                    int reviewsCount = random.Next(0, 4);

                    for (int i = 0; i < reviewsCount; i++)
                    {
                        var user = users[random.Next(users.Count)];
                        var rating = random.Next(3, 6);

                        reviews.Add(new Review
                        {
                            Author = user.UserName,
                            AuthorID = user.Id, 
                            Rating = rating,
                            Comment = GetReviewComment(product.Name, rating),
                            Date = DateTime.UtcNow.AddDays(-random.Next(1, 30)),
                            ProductId = product.ProductId
                        });
                    }
                }

                await context.Reviews.AddRangeAsync(reviews);
                await context.SaveChangesAsync();
            }
        }

        private static string GetReviewComment(string productName, int rating)
        {
            var goodComments = new[]
            {
                $"Отличный {productName}, полностью соответствует описанию!",
                $"Очень доволен покупкой {productName}, рекомендую!",
                $"{productName} превзошел все мои ожидания",
                $"Пользуюсь {productName} уже месяц - все отлично работает"
            };

            var averageComments = new[]
            {
                $"Неплохой {productName}, но есть небольшие недочеты",
                $"{productName} хороший, но цена завышена",
                $"В целом {productName} неплох, но ожидал большего"
            };

            return rating >= 4
                ? goodComments[new Random().Next(goodComments.Length)]
                : averageComments[new Random().Next(averageComments.Length)];
        }
    }
}
