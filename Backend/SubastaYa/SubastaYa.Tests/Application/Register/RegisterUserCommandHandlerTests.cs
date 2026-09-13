using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.UseCase.Commands.Auth.Register;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Exceptions;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class RegisterUserCommandHandlerTests
    {
        private class FakeUserRepository : IUserRepository
        {
            public Dictionary<string, User> UsersByEmail = new();
            public User? LastUserAdded;
            public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
                => Task.FromResult(UsersByEmail.TryGetValue(email, out var u) ? u : null);
            public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
            public Task AddAsync(User user, CancellationToken ct = default) { LastUserAdded = user; return Task.CompletedTask; }
        }

        private class FakePasswordHasher : IPasswordHasher
        {
            public string Hash(string plainPassword) => $"HASH:{plainPassword}";
            public bool Verify(string plainPassword, string passwordHash) => passwordHash == $"HASH:{plainPassword}";
        }

        private class FakeJwtTokenGenerator : IJwtTokenGenerator
        {
            public (string Token, DateTime ExpiresAt) Generate(User user) => ($"token-{user.Id}", DateTime.UtcNow.AddHours(2));
        }

        private class FakeUnitOfWork : IUnitOfWork
        {
            public Task BeginTransactionAsync(CancellationToken ct = default) => Task.CompletedTask;
            public Task CommitAsync(CancellationToken ct = default) => Task.CompletedTask;
            public Task RollbackAsync(CancellationToken ct = default) => Task.CompletedTask;
            public Task<int> SaveChangesAsync(CancellationToken ct = default) => Task.FromResult(1);
        }

        
        [Fact]
        public async Task Handle_EmailAlreadyRegistered_ThrowsEmailAlreadyRegisteredException()
        {
            var userRepository = new FakeUserRepository
            {
                UsersByEmail = { ["ana@test.com"] = new User { Id = Guid.NewGuid(), Email = "ana@test.com", Name = "Ana", PasswordHash = "x" } }
            };
            var handler = new RegisterUserCommandHandler(userRepository, new FakePasswordHasher(), new FakeJwtTokenGenerator(), new FakeUnitOfWork());

            var command = new RegisterUserCommand("ana@test.com", "New Ana", "123456");

            await Assert.ThrowsAsync<EmailAlreadyRegisteredException>(() => handler.Handle(command, CancellationToken.None));
        }

        
        [Fact]
        public async Task Handle_SuccessfulRegistration_CreatesZeroBalanceWalletAndReturnsGeneratedToken()
        {
            var userRepository = new FakeUserRepository();
            var handler = new RegisterUserCommandHandler(userRepository, new FakePasswordHasher(), new FakeJwtTokenGenerator(), new FakeUnitOfWork());

            var command = new RegisterUserCommand("bruno@test.com", "Bruno", "123456");
            var result = await handler.Handle(command, CancellationToken.None);

            Assert.NotNull(userRepository.LastUserAdded);
            Assert.Equal("HASH:123456", userRepository.LastUserAdded!.PasswordHash);
            Assert.Equal(0, userRepository.LastUserAdded.Wallet.TotalBalance);
            Assert.Equal(0, userRepository.LastUserAdded.Wallet.AvailableBalance);
            Assert.Equal($"token-{userRepository.LastUserAdded.Id}", result.Token);
            Assert.Equal("bruno@test.com", result.Email);
        }
    }
}
