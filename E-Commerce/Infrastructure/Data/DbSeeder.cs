using Domain.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<WaffarXEcommerceDBContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<BaseUser>>();

            // Ensure database is created and migrations are applied
            await context.Database.MigrateAsync();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Test User
            await SeedUsersAsync(userManager);

            // Seed Categories
            await SeedCategoriesAsync(context);

            // Seed Products
            await SeedProductsAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Customer"};

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"✅ Role '{role}' created");
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<BaseUser> userManager)
        {
            // Test Customer User
            var testCustomer = await userManager.FindByEmailAsync("customer@test.com");
            if (testCustomer == null)
            {
                testCustomer = new BaseUser
                {
                    UserName = "test_customer",
                    Email = "customer@test.com",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(testCustomer, "Test123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(testCustomer, "Customer");
                    Console.WriteLine("✅ Test customer user created (customer@test.com / Test123!)");
                }
            }

        }

        private static async Task SeedCategoriesAsync(WaffarXEcommerceDBContext context)
        {
            if (await context.Categories.AnyAsync())
            {
                return;
            }

            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Electronics",
                    Description = "Electronic devices and gadgets",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Clothing",
                    Description = "Fashion and apparel",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Books",
                    Description = "Books and educational materials",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Home & Kitchen",
                    Description = "Home appliances and kitchen tools",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Sports & Outdoors",
                    Description = "Sports equipment and outdoor gear",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {categories.Count} categories seeded");
        }

        private static async Task SeedProductsAsync(WaffarXEcommerceDBContext context)
        {
            if (await context.Products.AnyAsync())
            {
                return; // Already seeded
            }

            // Get categories
            var electronics = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
            var clothing = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Clothing");
            var books = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Books");
            var homeKitchen = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Home & Kitchen");
            var sports = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Sports & Outdoors");

            var products = new List<Product>
            {
                // Electronics
                new Product
                {
                    Name = "iPhone 15 Pro",
                    Description = "Latest Apple smartphone with A17 Pro chip, 256GB storage",
                    Price = 999.99m,
                    StockQuantity = 50,
                    CategoryId = electronics?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Samsung Galaxy S24",
                    Description = "Flagship Android smartphone with advanced AI features",
                    Price = 899.99m,
                    StockQuantity = 45,
                    CategoryId = electronics?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "MacBook Pro 16-inch",
                    Description = "Powerful laptop with M3 Pro chip, 512GB SSD",
                    Price = 2499.99m,
                    StockQuantity = 20,
                    CategoryId = electronics?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Sony WH-1000XM5 Headphones",
                    Description = "Premium noise-canceling wireless headphones",
                    Price = 349.99m,
                    StockQuantity = 75,
                    CategoryId = electronics?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "iPad Air",
                    Description = "Versatile tablet with M1 chip, 64GB",
                    Price = 599.99m,
                    StockQuantity = 60,
                    CategoryId = electronics?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Clothing
                new Product
                {
                    Name = "Nike Air Max Sneakers",
                    Description = "Comfortable running shoes with Max Air cushioning",
                    Price = 129.99m,
                    StockQuantity = 100,
                    CategoryId = clothing?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Levi's 501 Original Jeans",
                    Description = "Classic straight-fit denim jeans",
                    Price = 69.99m,
                    StockQuantity = 150,
                    CategoryId = clothing?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Patagonia Fleece Jacket",
                    Description = "Warm and sustainable outdoor jacket",
                    Price = 149.99m,
                    StockQuantity = 80,
                    CategoryId = clothing?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Books
                new Product
                {
                    Name = "Clean Code by Robert Martin",
                    Description = "A handbook of agile software craftsmanship",
                    Price = 39.99m,
                    StockQuantity = 200,
                    CategoryId = books?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Design Patterns",
                    Description = "Elements of reusable object-oriented software",
                    Price = 44.99m,
                    StockQuantity = 150,
                    CategoryId = books?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "The Pragmatic Programmer",
                    Description = "Your journey to mastery, 20th Anniversary Edition",
                    Price = 42.99m,
                    StockQuantity = 175,
                    CategoryId = books?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Home & Kitchen
                new Product
                {
                    Name = "Instant Pot Duo 7-in-1",
                    Description = "Electric pressure cooker, 6 quart",
                    Price = 89.99m,
                    StockQuantity = 120,
                    CategoryId = homeKitchen?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Ninja Professional Blender",
                    Description = "1000-watt blender with 72oz pitcher",
                    Price = 79.99m,
                    StockQuantity = 90,
                    CategoryId = homeKitchen?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "KitchenAid Stand Mixer",
                    Description = "5-quart tilt-head stand mixer",
                    Price = 299.99m,
                    StockQuantity = 40,
                    CategoryId = homeKitchen?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Sports & Outdoors
                new Product
                {
                    Name = "Yoga Mat Premium",
                    Description = "Extra thick 6mm non-slip exercise mat",
                    Price = 29.99m,
                    StockQuantity = 200,
                    CategoryId = sports?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Adjustable Dumbbell Set",
                    Description = "52.5 lbs adjustable dumbbells (pair)",
                    Price = 199.99m,
                    StockQuantity = 50,
                    CategoryId = sports?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Camping Tent 4-Person",
                    Description = "Waterproof family camping tent",
                    Price = 159.99m,
                    StockQuantity = 35,
                    CategoryId = sports?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Low stock items (for testing low stock alerts)
                new Product
                {
                    Name = "Limited Edition Smartwatch",
                    Description = "Special edition fitness tracker - few left!",
                    Price = 249.99m,
                    StockQuantity = 5,
                    CategoryId = electronics?.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Out of stock item (for testing stock validation)
                new Product
                {
                    Name = "Sold Out Wireless Earbuds",
                    Description = "Currently out of stock - check back soon",
                    Price = 179.99m,
                    StockQuantity = 0,
                    CategoryId = electronics?.Id,
                    IsActive = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ {products.Count} products seeded");
        }
    }
}