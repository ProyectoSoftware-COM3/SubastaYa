using System;
using System.Linq;
using System.Threading.Tasks;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Enums;

namespace SubastaYa.Infrastructure.Persistence
{
    public static class DbSeeder
    {
        //Hash bcrypt de "Test123!" compartido por los 5 usuarios del seed porque todos usan la misma contraseña.
        //Pasos:
        // 1. Agregue temporalmente en Program.cs la linea:
        //    Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("Test123!"));
        //    (comentada mas abajo en Program.cs, a modo de referencia)
        // 2. Corri la app una vez y mire lo que imprimio por consola.
        // 3. Copie ese string y lo pegue aca abajo, como constante.
        // 4. Borre(comento) esa linea de Program.cs -- era solo para generar el hash una unica vez.
        
        
        private const string SeedPasswordHash = "$2b$11$FUkmKkoqcCKfw/O6gY7zlehLBa9tKYCR9ns/azlP/s34HspXUOk8C";

        public static async Task SeedAsync(SubastaYaDbContext context)
        {
            if (context.Users.Any()) return;

            var categorias = new[]
            {
                new Category { Name = "Tecnologia", IconUrl = "tech.svg" },
                new Category { Name = "Coleccionables", IconUrl = "collect.svg" },
                new Category { Name = "Indumentaria", IconUrl = "clothes.svg" },
                new Category { Name = "Vehiculos", IconUrl = "cars.svg" },
            };
            context.Categories.AddRange(categorias);

            var vendedor = new User { Email = "vendedor@test.com", Name = "Vendedor", PasswordHash = SeedPasswordHash, RegisteredAt = DateTime.UtcNow, Wallet = new Wallet { TotalBalance = 0, HeldBalance = 0, AvailableBalance = 0 } };
            var comprador1 = new User { Email = "comprador1@test.com", Name = "Comprador Uno", PasswordHash = SeedPasswordHash, RegisteredAt = DateTime.UtcNow, Wallet = new Wallet { TotalBalance = 150000, HeldBalance = 45000, AvailableBalance = 105000 } };
            var comprador2 = new User { Email = "comprador2@test.com", Name = "Comprador Dos", PasswordHash = SeedPasswordHash, RegisteredAt = DateTime.UtcNow, Wallet = new Wallet { TotalBalance = 200000, HeldBalance = 0, AvailableBalance = 200000 } };
            var sinFondos = new User { Email = "sinfondos@test.com", Name = "Sin Fondos", PasswordHash = SeedPasswordHash, RegisteredAt = DateTime.UtcNow, Wallet = new Wallet { TotalBalance = 500, HeldBalance = 0, AvailableBalance = 500 } };
            var comprador3 = new User { Email = "comprador3@test.com", Name = "Comprador Tres", PasswordHash = SeedPasswordHash, RegisteredAt = DateTime.UtcNow, Wallet = new Wallet { TotalBalance = 35000, HeldBalance = 25000, AvailableBalance = 10000 } };
            context.Users.AddRange(vendedor, comprador1, comprador2, sinFondos, comprador3);

            await context.SaveChangesAsync();

            var activaEstandar = new Auction
            {
                Id = Guid.NewGuid(),
                SellerId = vendedor.Id,
                CategoryId = categorias[0].Id,
                Title = "Notebook gamer",
                Description = "Notebook usada, buen estado",
                ImageUrl = "notebook.jpg",
                BasePrice = 30000,
                MinIncrement = 1000,
                StartDate = DateTime.UtcNow.AddHours(-1),
                EndDate = DateTime.UtcNow.AddMinutes(25),
                Status = AuctionStatus.Active
            };
            var activaCritica = new Auction
            {
                Id = Guid.NewGuid(),
                SellerId = vendedor.Id,
                CategoryId = categorias[1].Id,
                Title = "Figura coleccionable",
                Description = "Edicion limitada",
                ImageUrl = "figura.jpg",
                BasePrice = 5000,
                MinIncrement = 500,
                StartDate = DateTime.UtcNow.AddMinutes(-30),
                EndDate = DateTime.UtcNow.AddSeconds(90),
                Status = AuctionStatus.Active
            };
            var proxima = new Auction
            {
                Id = Guid.NewGuid(),
                SellerId = vendedor.Id,
                CategoryId = categorias[2].Id,
                Title = "Campera de cuero",
                Description = "Talle M, sin uso",
                ImageUrl = "campera.jpg",
                BasePrice = 8000,
                MinIncrement = 500,
                StartDate = DateTime.UtcNow.AddHours(24),
                EndDate = DateTime.UtcNow.AddHours(48),
                Status = AuctionStatus.Scheduled
            };
            var vencidaConGanador = new Auction
            {
                Id = Guid.NewGuid(),
                SellerId = vendedor.Id,
                CategoryId = categorias[3].Id,
                Title = "Bicicleta rodado 29",
                Description = "Poco uso",
                ImageUrl = "bici.jpg",
                BasePrice = 20000,
                MinIncrement = 1000,
                StartDate = DateTime.UtcNow.AddHours(-5),
                EndDate = DateTime.UtcNow.AddMinutes(-1),
                Status = AuctionStatus.Active
            };
            var vencidaDesierta = new Auction
            {
                Id = Guid.NewGuid(),
                SellerId = vendedor.Id,
                CategoryId = categorias[0].Id,
                Title = "Teclado mecanico",
                Description = "Sin ofertas esperadas",
                ImageUrl = "teclado.jpg",
                BasePrice = 10000,
                MinIncrement = 500,
                StartDate = DateTime.UtcNow.AddHours(-5),
                EndDate = DateTime.UtcNow.AddMinutes(-2),
                Status = AuctionStatus.Active
            };
            var vencidaLimpia = new Auction
            {
                Id = Guid.NewGuid(),
                SellerId = vendedor.Id,
                CategoryId = categorias[1].Id,
                Title = "Reloj vintage",
                Description = "Escenario limpio para probar el Worker",
                ImageUrl = "reloj.jpg",
                BasePrice = 2000,
                MinIncrement = 200,
                StartDate = DateTime.UtcNow.AddHours(-3),
                EndDate = DateTime.UtcNow.AddMinutes(-3),
                Status = AuctionStatus.Active
            };
            context.Auctions.AddRange(activaEstandar, activaCritica, proxima, vencidaConGanador, vencidaDesierta, vencidaLimpia);

            context.Bids.AddRange(
                new Bid { Id = Guid.NewGuid(), AuctionId = activaEstandar.Id, BidderId = comprador2.Id, Amount = 40000, PlacedAt = DateTime.UtcNow.AddMinutes(-40) },
                new Bid { Id = Guid.NewGuid(), AuctionId = activaEstandar.Id, BidderId = comprador1.Id, Amount = 45000, PlacedAt = DateTime.UtcNow.AddMinutes(-10) },
                new Bid { Id = Guid.NewGuid(), AuctionId = vencidaConGanador.Id, BidderId = comprador3.Id, Amount = 22000, PlacedAt = DateTime.UtcNow.AddHours(-2) },
                new Bid { Id = Guid.NewGuid(), AuctionId = vencidaLimpia.Id, BidderId = comprador3.Id, Amount = 3000, PlacedAt = DateTime.UtcNow.AddHours(-3).AddMinutes(30) }
            );

            context.LedgerTransactions.AddRange(
                new LedgerTransaction { Id = Guid.NewGuid(), WalletId = comprador1.Wallet.Id, Type = LedgerTransactionType.Deposit, Amount = 150000, OccurredAt = DateTime.UtcNow.AddDays(-1) },
                new LedgerTransaction { Id = Guid.NewGuid(), WalletId = comprador1.Wallet.Id, Type = LedgerTransactionType.Hold, Amount = 45000, OccurredAt = DateTime.UtcNow.AddMinutes(-10), AuctionId = activaEstandar.Id },
                new LedgerTransaction { Id = Guid.NewGuid(), WalletId = comprador2.Wallet.Id, Type = LedgerTransactionType.Deposit, Amount = 200000, OccurredAt = DateTime.UtcNow.AddDays(-1) },
                new LedgerTransaction { Id = Guid.NewGuid(), WalletId = comprador2.Wallet.Id, Type = LedgerTransactionType.Hold, Amount = 40000, OccurredAt = DateTime.UtcNow.AddMinutes(-40), AuctionId = activaEstandar.Id },
                new LedgerTransaction { Id = Guid.NewGuid(), WalletId = comprador2.Wallet.Id, Type = LedgerTransactionType.Release, Amount = 40000, OccurredAt = DateTime.UtcNow.AddMinutes(-10), AuctionId = activaEstandar.Id },
                new LedgerTransaction { Id = Guid.NewGuid(), WalletId = comprador3.Wallet.Id, Type = LedgerTransactionType.Deposit, Amount = 35000, OccurredAt = DateTime.UtcNow.AddDays(-1) },
                new LedgerTransaction { Id = Guid.NewGuid(), WalletId = comprador3.Wallet.Id, Type = LedgerTransactionType.Hold, Amount = 22000, OccurredAt = DateTime.UtcNow.AddHours(-2), AuctionId = vencidaConGanador.Id },
                new LedgerTransaction { Id = Guid.NewGuid(), WalletId = comprador3.Wallet.Id, Type = LedgerTransactionType.Hold, Amount = 3000, OccurredAt = DateTime.UtcNow.AddHours(-3).AddMinutes(30), AuctionId = vencidaLimpia.Id }
            );

            await context.SaveChangesAsync();
        }
    }
}