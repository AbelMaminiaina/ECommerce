using MongoDB.Driver;
using ECommerce.Domain.Entities;
using BCrypt.Net;

namespace ECommerce.API;

public static class SeedData
{
    public static async Task Initialize(IMongoDatabase database)
    {
        await SeedUsers(database);
        await SeedCategories(database);
        await SeedProducts(database);
        await SeedOrders(database);
        await SeedSupportTickets(database);
        await SeedWarrantyClaims(database);
        await SeedShippingMethods(database);
    }

    private static async Task SeedUsers(IMongoDatabase database)
    {
        var usersCollection = database.GetCollection<User>("users");

        // Vérifier si un admin existe déjà
        var adminExists = await usersCollection
            .Find(u => u.Role == "Admin")
            .AnyAsync();

        if (!adminExists)
        {
            // Créer un utilisateur admin par défaut
            var adminUser = new User
            {
                Email = "admin@example.com",
                FirstName = "Admin",
                LastName = "User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = "Admin",
                PhoneNumber = "+1234567890",
                Addresses = new List<Address>(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await usersCollection.InsertOneAsync(adminUser);
            Console.WriteLine("✅ Utilisateur admin créé avec succès!");
            Console.WriteLine("📧 Email: admin@example.com");
            Console.WriteLine("🔑 Mot de passe: Admin123!");
        }

        // Créer un utilisateur test normal
        var userExists = await usersCollection
            .Find(u => u.Email == "user@example.com")
            .AnyAsync();

        if (!userExists)
        {
            var testUser = new User
            {
                Email = "user@example.com",
                FirstName = "John",
                LastName = "Doe",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                Role = "Customer",
                PhoneNumber = "+1234567891",
                Addresses = new List<Address>
                {
                    new Address
                    {
                        Street = "123 Main St",
                        City = "New York",
                        State = "NY",
                        ZipCode = "10001",
                        Country = "USA",
                        IsDefault = true
                    }
                },
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await usersCollection.InsertOneAsync(testUser);
            Console.WriteLine("✅ Utilisateur test créé avec succès!");
            Console.WriteLine("📧 Email: user@example.com");
            Console.WriteLine("🔑 Mot de passe: User123!");
        }
    }

    private static async Task SeedCategories(IMongoDatabase database)
    {
        var categoriesCollection = database.GetCollection<Category>("categories");
        var count = await categoriesCollection.CountDocumentsAsync(FilterDefinition<Category>.Empty);

        if (count == 0)
        {
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "Électronique",
                    Description = "Appareils électroniques et gadgets",
                    ImageUrl = "https://images.unsplash.com/photo-1498049794561-7780e7231661",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Vêtements",
                    Description = "Mode et habillement pour homme et femme",
                    ImageUrl = "https://images.unsplash.com/photo-1441986300917-64674bd600d8",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Livres",
                    Description = "Livres et publications",
                    ImageUrl = "https://images.unsplash.com/photo-1495446815901-a7297e633e8d",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Maison & Jardin",
                    Description = "Articles pour la maison et le jardin",
                    ImageUrl = "https://images.unsplash.com/photo-1484101403633-562f891dc89a",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Name = "Sports & Loisirs",
                    Description = "Équipements sportifs et de loisirs",
                    ImageUrl = "https://images.unsplash.com/photo-1461896836934-ffe607ba8211",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await categoriesCollection.InsertManyAsync(categories);
            Console.WriteLine($"✅ {categories.Count} catégories créées avec succès!");
        }
    }

    private static async Task SeedProducts(IMongoDatabase database)
    {
        var productsCollection = database.GetCollection<Product>("products");
        var categoriesCollection = database.GetCollection<Category>("categories");

        var count = await productsCollection.CountDocumentsAsync(FilterDefinition<Product>.Empty);

        if (count == 0)
        {
            var categories = await categoriesCollection.Find(FilterDefinition<Category>.Empty).ToListAsync();
            var electronicsId = categories.First(c => c.Name == "Électronique").Id;
            var clothingId = categories.First(c => c.Name == "Vêtements").Id;
            var booksId = categories.First(c => c.Name == "Livres").Id;
            var homeId = categories.First(c => c.Name == "Maison & Jardin").Id;
            var sportsId = categories.First(c => c.Name == "Sports & Loisirs").Id;

            var products = new List<Product>
            {
                // Électronique
                new Product
                {
                    Name = "Smartphone XPro 14",
                    Description = "Smartphone haut de gamme avec écran OLED 6.7\", 256GB",
                    Price = 999.99m,
                    Stock = 50,
                    CategoryId = electronicsId!,
                    Images = new List<string> { "https://images.unsplash.com/photo-1511707171634-5f897ff02aa9" },
                    IsFeatured = true,
                    IsActive = true,
                    Specifications = new Dictionary<string, string>
                    {
                        { "Écran", "6.7\" OLED" },
                        { "Stockage", "256GB" },
                        { "RAM", "8GB" },
                        { "Caméra", "48MP" }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Laptop Pro 15",
                    Description = "Ordinateur portable professionnel, Intel i7, 16GB RAM, 512GB SSD",
                    Price = 1499.99m,
                    Stock = 30,
                    CategoryId = electronicsId!,
                    Images = new List<string> { "https://images.unsplash.com/photo-1496181133206-80ce9b88a853" },
                    IsFeatured = true,
                    IsActive = true,
                    Specifications = new Dictionary<string, string>
                    {
                        { "Processeur", "Intel i7" },
                        { "RAM", "16GB" },
                        { "Stockage", "512GB SSD" },
                        { "Écran", "15.6\" Full HD" }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Écouteurs Sans Fil Premium",
                    Description = "Écouteurs sans fil avec réduction de bruit active",
                    Price = 299.99m,
                    Stock = 100,
                    CategoryId = electronicsId!,
                    Images = new List<string> { "https://images.unsplash.com/photo-1505740420928-5e560c06d30e" },
                    IsFeatured = false,
                    IsActive = true,
                    Specifications = new Dictionary<string, string>
                    {
                        { "Autonomie", "30h" },
                        { "Réduction de bruit", "Active" },
                        { "Connectivité", "Bluetooth 5.0" }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Vêtements
                new Product
                {
                    Name = "T-Shirt Coton Bio",
                    Description = "T-shirt en coton biologique, disponible en plusieurs couleurs",
                    Price = 29.99m,
                    Stock = 200,
                    CategoryId = clothingId!,
                    Images = new List<string> { "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab" },
                    IsFeatured = false,
                    IsActive = true,
                    Specifications = new Dictionary<string, string>
                    {
                        { "Matière", "100% Coton Bio" },
                        { "Tailles", "S, M, L, XL" },
                        { "Entretien", "Lavable en machine" }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Jean Slim Fit",
                    Description = "Jean slim fit en denim stretch confortable",
                    Price = 79.99m,
                    Stock = 150,
                    CategoryId = clothingId!,
                    Images = new List<string> { "https://images.unsplash.com/photo-1542272604-787c3835535d" },
                    IsFeatured = true,
                    IsActive = true,
                    Specifications = new Dictionary<string, string>
                    {
                        { "Matière", "98% Coton, 2% Élasthanne" },
                        { "Coupe", "Slim Fit" },
                        { "Tailles", "28-42" }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Livres
                new Product
                {
                    Name = "Clean Code - Robert C. Martin",
                    Description = "Guide pratique pour écrire du code propre et maintenable",
                    Price = 44.99m,
                    Stock = 75,
                    CategoryId = booksId!,
                    Images = new List<string> { "https://images.unsplash.com/photo-1532012197267-da84d127e765" },
                    IsFeatured = true,
                    IsActive = true,
                    Specifications = new Dictionary<string, string>
                    {
                        { "Auteur", "Robert C. Martin" },
                        { "Pages", "464" },
                        { "Langue", "Anglais" },
                        { "Format", "Broché" }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Maison & Jardin
                new Product
                {
                    Name = "Cafetière Expresso Automatique",
                    Description = "Machine à café expresso avec broyeur intégré",
                    Price = 599.99m,
                    Stock = 25,
                    CategoryId = homeId!,
                    Images = new List<string> { "https://images.unsplash.com/photo-1517668808822-9ebb02f2a0e6" },
                    IsFeatured = true,
                    IsActive = true,
                    Specifications = new Dictionary<string, string>
                    {
                        { "Type", "Expresso automatique" },
                        { "Pression", "15 bars" },
                        { "Capacité", "1.8L" }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Set de Couteaux Professionnel",
                    Description = "Set de 5 couteaux de cuisine en acier inoxydable",
                    Price = 149.99m,
                    Stock = 60,
                    CategoryId = homeId!,
                    Images = new List<string> { "https://images.unsplash.com/photo-1593618998160-e34014e67546" },
                    IsFeatured = false,
                    IsActive = true,
                    Specifications = new Dictionary<string, string>
                    {
                        { "Matière", "Acier inoxydable" },
                        { "Pièces", "5 couteaux" },
                        { "Entretien", "Lavage à la main recommandé" }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },

                // Sports & Loisirs
                new Product
                {
                    Name = "Tapis de Yoga Premium",
                    Description = "Tapis de yoga antidérapant en TPE écologique",
                    Price = 49.99m,
                    Stock = 120,
                    CategoryId = sportsId!,
                    Images = new List<string> { "https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f" },
                    IsFeatured = false,
                    IsActive = true,
                    Specifications = new Dictionary<string, string>
                    {
                        { "Matière", "TPE écologique" },
                        { "Dimensions", "183 x 61 cm" },
                        { "Épaisseur", "6mm" }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Name = "Vélo VTT 29 pouces",
                    Description = "VTT tout-terrain avec suspension avant et 21 vitesses",
                    Price = 899.99m,
                    Stock = 15,
                    CategoryId = sportsId!,
                    Images = new List<string> { "https://images.unsplash.com/photo-1576435728678-68d0fbf94e91" },
                    IsFeatured = true,
                    IsActive = true,
                    Specifications = new Dictionary<string, string>
                    {
                        { "Roues", "29 pouces" },
                        { "Vitesses", "21" },
                        { "Cadre", "Aluminium" },
                        { "Suspension", "Avant" }
                    },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await productsCollection.InsertManyAsync(products);
            Console.WriteLine($"✅ {products.Count} produits créés avec succès!");
        }
    }

    private static async Task SeedOrders(IMongoDatabase database)
    {
        var ordersCollection = database.GetCollection<Order>("orders");
        var usersCollection = database.GetCollection<User>("users");
        var productsCollection = database.GetCollection<Product>("products");

        // TEMPORAIRE: Supprimer toutes les commandes existantes pour recréer avec les nouvelles données
        await ordersCollection.DeleteManyAsync(FilterDefinition<Order>.Empty);
        Console.WriteLine("🗑️ Anciennes commandes supprimées!");

        var count = await ordersCollection.CountDocumentsAsync(FilterDefinition<Order>.Empty);

        if (count == 0)
        {
            var testUser = await usersCollection.Find(u => u.Email == "user@example.com").FirstOrDefaultAsync();
            if (testUser != null)
            {
                var products = await productsCollection.Find(FilterDefinition<Product>.Empty).Limit(5).ToListAsync();

                var orders = new List<Order>
                {
                    // 1. Commande LIVRÉE (avec possibilité de retour)
                    new Order
                    {
                        UserId = testUser.Id!,
                        Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                ProductId = products[0].Id!,
                                ProductName = products[0].Name,
                                Quantity = 1,
                                Price = products[0].Price
                            }
                        },
                        TotalAmount = products[0].Price,
                        Status = OrderStatus.Delivered,
                        PaymentStatus = PaymentStatus.Completed,
                        ShippingAddress = testUser.Addresses.First(),
                        PaymentIntentId = "pi_test_12345",
                        DeliveredAt = DateTime.UtcNow.AddDays(-5),
                        TrackingNumber = "COL123456789FR",
                        CarrierName = "Colissimo",
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        UpdatedAt = DateTime.UtcNow.AddDays(-5)
                    },

                    // 2. Commande EN TRAITEMENT
                    new Order
                    {
                        UserId = testUser.Id!,
                        Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                ProductId = products[1].Id!,
                                ProductName = products[1].Name,
                                Quantity = 2,
                                Price = products[1].Price
                            }
                        },
                        TotalAmount = products[1].Price * 2,
                        Status = OrderStatus.Processing,
                        PaymentStatus = PaymentStatus.Completed,
                        ShippingAddress = testUser.Addresses.First(),
                        PaymentIntentId = "pi_test_67890",
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    },

                    // 3. Commande EN ATTENTE (paiement en attente)
                    new Order
                    {
                        UserId = testUser.Id!,
                        Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                ProductId = products[2].Id!,
                                ProductName = products[2].Name,
                                Quantity = 1,
                                Price = products[2].Price
                            }
                        },
                        TotalAmount = products[2].Price,
                        Status = OrderStatus.Pending,
                        PaymentStatus = PaymentStatus.Pending,
                        ShippingAddress = testUser.Addresses.First(),
                        PaymentIntentId = "pi_test_11111",
                        CreatedAt = DateTime.UtcNow.AddHours(-6),
                        UpdatedAt = DateTime.UtcNow.AddHours(-6)
                    },

                    // 4. Commande EXPÉDIÉE (en transit)
                    new Order
                    {
                        UserId = testUser.Id!,
                        Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                ProductId = products[3].Id!,
                                ProductName = products[3].Name,
                                Quantity = 1,
                                Price = products[3].Price
                            },
                            new OrderItem
                            {
                                ProductId = products[4].Id!,
                                ProductName = products[4].Name,
                                Quantity = 1,
                                Price = products[4].Price
                            }
                        },
                        TotalAmount = products[3].Price + products[4].Price,
                        Status = OrderStatus.Shipped,
                        PaymentStatus = PaymentStatus.Completed,
                        ShippingAddress = testUser.Addresses.First(),
                        PaymentIntentId = "pi_test_22222",
                        ShippedAt = DateTime.UtcNow.AddDays(-1),
                        EstimatedDeliveryDate = DateTime.UtcNow.AddDays(2),
                        TrackingNumber = "CHR987654321FR",
                        CarrierName = "Chronopost",
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    },

                    // 5. Commande ANNULÉE
                    new Order
                    {
                        UserId = testUser.Id!,
                        Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                ProductId = products[0].Id!,
                                ProductName = products[0].Name,
                                Quantity = 3,
                                Price = products[0].Price
                            }
                        },
                        TotalAmount = products[0].Price * 3,
                        Status = OrderStatus.Cancelled,
                        PaymentStatus = PaymentStatus.Refunded,
                        ShippingAddress = testUser.Addresses.First(),
                        PaymentIntentId = "pi_test_33333",
                        CreatedAt = DateTime.UtcNow.AddDays(-15),
                        UpdatedAt = DateTime.UtcNow.AddDays(-14)
                    },

                    // 6. Commande avec RETOUR DEMANDÉ
                    new Order
                    {
                        UserId = testUser.Id!,
                        Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                ProductId = products[1].Id!,
                                ProductName = products[1].Name,
                                Quantity = 1,
                                Price = products[1].Price
                            }
                        },
                        TotalAmount = products[1].Price,
                        Status = OrderStatus.ReturnRequested,
                        PaymentStatus = PaymentStatus.Completed,
                        ShippingAddress = testUser.Addresses.First(),
                        PaymentIntentId = "pi_test_44444",
                        DeliveredAt = DateTime.UtcNow.AddDays(-8),
                        ReturnRequestedAt = DateTime.UtcNow.AddDays(-2),
                        ReturnReason = "Le produit ne correspond pas à la description",
                        ReturnStatus = ReturnStatus.Requested,
                        TrackingNumber = "COL555666777FR",
                        CarrierName = "Colissimo",
                        CreatedAt = DateTime.UtcNow.AddDays(-12),
                        UpdatedAt = DateTime.UtcNow.AddDays(-2)
                    },

                    // 7. Commande RETOURNÉE (remboursée)
                    new Order
                    {       
                        UserId = testUser.Id!,
                        Items = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                ProductId = products[2].Id!,
                                ProductName = products[2].Name,
                                Quantity = 2,
                                Price = products[2].Price
                            }
                        },
                        TotalAmount = products[2].Price * 2,
                        Status = OrderStatus.Returned,
                        PaymentStatus = PaymentStatus.Refunded,
                        ShippingAddress = testUser.Addresses.First(),
                        PaymentIntentId = "pi_test_55555",
                        DeliveredAt = DateTime.UtcNow.AddDays(-20),
                        ReturnRequestedAt = DateTime.UtcNow.AddDays(-18),
                        ReturnReason = "Défaut de fabrication",
                        ReturnStatus = ReturnStatus.Refunded,
                        TrackingNumber = "COL888999000FR",
                        CarrierName = "Colissimo",
                        CreatedAt = DateTime.UtcNow.AddDays(-25),
                        UpdatedAt = DateTime.UtcNow.AddDays(-16)
                    }
                };

                await ordersCollection.InsertManyAsync(orders);
                Console.WriteLine($"✅ {orders.Count} commandes de test créées avec succès!");
            }
        }
    }

    private static async Task SeedSupportTickets(IMongoDatabase database)
    {
        var supportTicketsCollection = database.GetCollection<SupportTicket>("supportTickets");
        var usersCollection = database.GetCollection<User>("users");

        var count = await supportTicketsCollection.CountDocumentsAsync(FilterDefinition<SupportTicket>.Empty);

        if (count == 0)
        {
            var testUser = await usersCollection.Find(u => u.Email == "user@example.com").FirstOrDefaultAsync();
            if (testUser != null)
            {
                var tickets = new List<SupportTicket>
                {
                    // Ticket OUVERT - Problème de livraison
                    new SupportTicket
                    {
                        UserId = testUser.Id!,
                        Subject = "Problème avec ma livraison",
                        Description = "Je n'ai toujours pas reçu ma commande COL123456789FR. Pouvez-vous m'aider ?",
                        Status = TicketStatus.Open,
                        Priority = TicketPriority.High,
                        Category = TicketCategory.Delivery,
                        Messages = new List<TicketMessage>
                        {
                            new TicketMessage
                            {
                                SenderId = testUser.Id!,
                                SenderName = $"{testUser.FirstName} {testUser.LastName}",
                                IsFromAdmin = false,
                                Message = "Je n'ai toujours pas reçu ma commande COL123456789FR. Pouvez-vous m'aider ?",
                                CreatedAt = DateTime.UtcNow.AddDays(-2)
                            }
                        },
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                        UpdatedAt = DateTime.UtcNow.AddDays(-2)
                    },

                    // Ticket EN COURS - Question produit
                    new SupportTicket
                    {
                        UserId = testUser.Id!,
                        Subject = "Question sur les spécifications du Smartphone XPro 14",
                        Description = "Bonjour, je voudrais savoir si le smartphone est compatible avec la 5G et s'il est possible d'étendre la mémoire ?",
                        Status = TicketStatus.InProgress,
                        Priority = TicketPriority.Medium,
                        Category = TicketCategory.Product,
                        AssignedToAdminId = "admin",
                        Messages = new List<TicketMessage>
                        {
                            new TicketMessage
                            {
                                SenderId = testUser.Id!,
                                SenderName = $"{testUser.FirstName} {testUser.LastName}",
                                IsFromAdmin = false,
                                Message = "Bonjour, je voudrais savoir si le smartphone est compatible avec la 5G et s'il est possible d'étendre la mémoire ?",
                                CreatedAt = DateTime.UtcNow.AddDays(-3)
                            },
                            new TicketMessage
                            {
                                SenderId = "admin",
                                SenderName = "Admin Support",
                                IsFromAdmin = true,
                                Message = "Bonjour, le Smartphone XPro 14 est bien compatible 5G. Concernant la mémoire, elle n'est pas extensible mais dispose de 256GB de stockage interne.",
                                CreatedAt = DateTime.UtcNow.AddHours(-6)
                            }
                        },
                        CreatedAt = DateTime.UtcNow.AddDays(-3),
                        UpdatedAt = DateTime.UtcNow.AddHours(-6)
                    },

                    // Ticket RÉSOLU - Remboursement
                    new SupportTicket
                    {
                        UserId = testUser.Id!,
                        Subject = "Demande de remboursement pour commande annulée",
                        Description = "J'ai annulé ma commande pi_test_33333 mais je n'ai pas encore reçu le remboursement.",
                        Status = TicketStatus.Resolved,
                        Priority = TicketPriority.High,
                        Category = TicketCategory.Payment,
                        AssignedToAdminId = "admin",
                        Messages = new List<TicketMessage>
                        {
                            new TicketMessage
                            {
                                SenderId = testUser.Id!,
                                SenderName = $"{testUser.FirstName} {testUser.LastName}",
                                IsFromAdmin = false,
                                Message = "J'ai annulé ma commande pi_test_33333 mais je n'ai pas encore reçu le remboursement.",
                                CreatedAt = DateTime.UtcNow.AddDays(-7)
                            },
                            new TicketMessage
                            {
                                SenderId = "admin",
                                SenderName = "Admin Support",
                                IsFromAdmin = true,
                                Message = "Votre remboursement a bien été traité. Le montant devrait apparaître sur votre compte sous 3-5 jours ouvrables.",
                                CreatedAt = DateTime.UtcNow.AddDays(-5)
                            }
                        },
                        CreatedAt = DateTime.UtcNow.AddDays(-7),
                        UpdatedAt = DateTime.UtcNow.AddDays(-4)
                    },

                    // Ticket FERMÉ - Problème résolu
                    new SupportTicket
                    {
                        UserId = testUser.Id!,
                        Subject = "Impossible de me connecter à mon compte",
                        Description = "J'ai oublié mon mot de passe et je ne peux plus me connecter.",
                        Status = TicketStatus.Closed,
                        Priority = TicketPriority.Medium,
                        Category = TicketCategory.Technical,
                        AssignedToAdminId = "admin",
                        ClosedAt = DateTime.UtcNow.AddDays(-9),
                        Messages = new List<TicketMessage>
                        {
                            new TicketMessage
                            {
                                SenderId = testUser.Id!,
                                SenderName = $"{testUser.FirstName} {testUser.LastName}",
                                IsFromAdmin = false,
                                Message = "J'ai oublié mon mot de passe et je ne peux plus me connecter.",
                                CreatedAt = DateTime.UtcNow.AddDays(-12)
                            },
                            new TicketMessage
                            {
                                SenderId = "admin",
                                SenderName = "Admin Support",
                                IsFromAdmin = true,
                                Message = "Un email de réinitialisation vous a été envoyé. Merci de vérifier votre boîte mail.",
                                CreatedAt = DateTime.UtcNow.AddDays(-10)
                            }
                        },
                        CreatedAt = DateTime.UtcNow.AddDays(-12),
                        UpdatedAt = DateTime.UtcNow.AddDays(-9)
                    },

                    // Ticket OUVERT - Produit défectueux
                    new SupportTicket
                    {
                        UserId = testUser.Id!,
                        Subject = "Produit défectueux - Écouteurs sans fil",
                        Description = "Les écouteurs que j'ai reçus ne fonctionnent pas correctement. L'oreillette droite ne charge plus.",
                        Status = TicketStatus.Open,
                        Priority = TicketPriority.High,
                        Category = TicketCategory.Product,
                        Messages = new List<TicketMessage>
                        {
                            new TicketMessage
                            {
                                SenderId = testUser.Id!,
                                SenderName = $"{testUser.FirstName} {testUser.LastName}",
                                IsFromAdmin = false,
                                Message = "Les écouteurs que j'ai reçus ne fonctionnent pas correctement. L'oreillette droite ne charge plus.",
                                CreatedAt = DateTime.UtcNow.AddHours(-12)
                            }
                        },
                        CreatedAt = DateTime.UtcNow.AddHours(-12),
                        UpdatedAt = DateTime.UtcNow.AddHours(-12)
                    }
                };

                await supportTicketsCollection.InsertManyAsync(tickets);
                Console.WriteLine($"✅ {tickets.Count} tickets de support créés avec succès!");
            }
        }
    }

    private static async Task SeedWarrantyClaims(IMongoDatabase database)
    {
        var warrantyClaimsCollection = database.GetCollection<WarrantyClaim>("warrantyClaims");
        var usersCollection = database.GetCollection<User>("users");
        var ordersCollection = database.GetCollection<Order>("orders");

        var count = await warrantyClaimsCollection.CountDocumentsAsync(FilterDefinition<WarrantyClaim>.Empty);

        if (count == 0)
        {
            var testUser = await usersCollection.Find(u => u.Email == "user@example.com").FirstOrDefaultAsync();
            var deliveredOrder = await ordersCollection.Find(o => o.Status == OrderStatus.Delivered).FirstOrDefaultAsync();

            if (testUser != null && deliveredOrder != null)
            {
                var claims = new List<WarrantyClaim>
                {
                    // Demande SOUMISE - Smartphone défectueux
                    new WarrantyClaim
                    {
                        UserId = testUser.Id!,
                        OrderId = deliveredOrder.Id!,
                        ProductId = deliveredOrder.Items.First().ProductId,
                        ProductName = deliveredOrder.Items.First().ProductName,
                        IssueDescription = "L'écran du smartphone affiche des lignes verticales depuis 2 jours. Le téléphone a été acheté il y a 8 mois.",
                        Status = WarrantyClaimStatus.Submitted,
                        PurchaseDate = DateTime.UtcNow.AddMonths(-8),
                        WarrantyExpirationDate = DateTime.UtcNow.AddMonths(16), // Garantie 2 ans
                        Photos = new List<string>(),
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    },

                    // Demande EN EXAMEN - Laptop
                    new WarrantyClaim
                    {
                        UserId = testUser.Id!,
                        OrderId = deliveredOrder.Id!,
                        ProductId = deliveredOrder.Items.First().ProductId,
                        ProductName = "Laptop Pro 15",
                        IssueDescription = "Le ventilateur fait un bruit anormal et l'ordinateur surchauffe rapidement.",
                        Status = WarrantyClaimStatus.UnderReview,
                        PurchaseDate = DateTime.UtcNow.AddMonths(-6),
                        WarrantyExpirationDate = DateTime.UtcNow.AddMonths(18),
                        AdminNotes = "Demande en cours d'examen. Vérification de l'éligibilité à la garantie.",
                        Photos = new List<string>(),
                        CreatedAt = DateTime.UtcNow.AddDays(-10),
                        UpdatedAt = DateTime.UtcNow.AddDays(-8)
                    },

                    // Demande APPROUVÉE - Écouteurs
                    new WarrantyClaim
                    {
                        UserId = testUser.Id!,
                        OrderId = deliveredOrder.Id!,
                        ProductId = deliveredOrder.Items.First().ProductId,
                        ProductName = "Écouteurs Sans Fil Premium",
                        IssueDescription = "L'oreillette gauche ne se connecte plus en Bluetooth.",
                        Status = WarrantyClaimStatus.Approved,
                        PurchaseDate = DateTime.UtcNow.AddMonths(-4),
                        WarrantyExpirationDate = DateTime.UtcNow.AddMonths(20),
                        AdminNotes = "Garantie valide. Réparation approuvée. Un technicien prendra contact avec le client sous 48h.",
                        Photos = new List<string>(),
                        CreatedAt = DateTime.UtcNow.AddDays(-15),
                        UpdatedAt = DateTime.UtcNow.AddDays(-12)
                    },

                    // Demande RÉSOLUE - Cafetière réparée
                    new WarrantyClaim
                    {
                        UserId = testUser.Id!,
                        OrderId = deliveredOrder.Id!,
                        ProductId = deliveredOrder.Items.First().ProductId,
                        ProductName = "Cafetière Expresso Automatique",
                        IssueDescription = "La machine ne chauffe plus l'eau correctement.",
                        Status = WarrantyClaimStatus.Resolved,
                        PurchaseDate = DateTime.UtcNow.AddMonths(-10),
                        WarrantyExpirationDate = DateTime.UtcNow.AddMonths(14),
                        Resolution = "Résistance chauffante remplacée. Produit testé et renvoyé au client.",
                        AdminNotes = "Réparation effectuée avec succès. Garantie prolongée de 3 mois.",
                        ResolvedAt = DateTime.UtcNow.AddDays(-20),
                        Photos = new List<string>(),
                        CreatedAt = DateTime.UtcNow.AddDays(-30),
                        UpdatedAt = DateTime.UtcNow.AddDays(-20)
                    },

                    // Demande REJETÉE - Hors garantie
                    new WarrantyClaim
                    {
                        UserId = testUser.Id!,
                        OrderId = deliveredOrder.Id!,
                        ProductId = deliveredOrder.Items.First().ProductId,
                        ProductName = "Vélo VTT 29 pouces",
                        IssueDescription = "Les freins ne fonctionnent plus correctement.",
                        Status = WarrantyClaimStatus.Rejected,
                        PurchaseDate = DateTime.UtcNow.AddMonths(-26),
                        WarrantyExpirationDate = DateTime.UtcNow.AddMonths(-2),
                        Resolution = "Demande rejetée : garantie expirée.",
                        AdminNotes = "La garantie a expiré depuis 2 mois. Pour toute réparation, veuillez contacter un centre de réparation agréé.",
                        ResolvedAt = DateTime.UtcNow.AddDays(-4),
                        Photos = new List<string>(),
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        UpdatedAt = DateTime.UtcNow.AddDays(-4)
                    }
                };

                await warrantyClaimsCollection.InsertManyAsync(claims);
                Console.WriteLine($"✅ {claims.Count} demandes de garantie créées avec succès!");
            }
        }
    }

    private static async Task SeedShippingMethods(IMongoDatabase database)
    {
        var shippingMethodsCollection = database.GetCollection<ShippingMethod>("shippingMethods");

        var count = await shippingMethodsCollection.CountDocumentsAsync(FilterDefinition<ShippingMethod>.Empty);

        if (count == 0)
        {
            var shippingMethods = new List<ShippingMethod>
            {
                new ShippingMethod
                {
                    Name = "Colissimo Standard",
                    Description = "Livraison standard sous 3-5 jours ouvrés",
                    MinDeliveryDays = 3,
                    MaxDeliveryDays = 5,
                    CarrierName = "La Poste - Colissimo",
                    Price = 5.99m,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ShippingMethod
                {
                    Name = "Chronopost Express",
                    Description = "Livraison express sous 24-48h",
                    MinDeliveryDays = 1,
                    MaxDeliveryDays = 2,
                    CarrierName = "Chronopost",
                    Price = 12.99m,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ShippingMethod
                {
                    Name = "Point Relais",
                    Description = "Retrait en point relais sous 2-4 jours",
                    MinDeliveryDays = 2,
                    MaxDeliveryDays = 4,
                    CarrierName = "Mondial Relay",
                    Price = 3.99m,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ShippingMethod
                {
                    Name = "Livraison Gratuite",
                    Description = "Livraison gratuite pour les commandes de plus de 50€",
                    MinDeliveryDays = 4,
                    MaxDeliveryDays = 7,
                    CarrierName = "La Poste",
                    Price = 0m,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await shippingMethodsCollection.InsertManyAsync(shippingMethods);
            Console.WriteLine($"✅ {shippingMethods.Count} méthodes de livraison créées avec succès!");
        }
    }
}
