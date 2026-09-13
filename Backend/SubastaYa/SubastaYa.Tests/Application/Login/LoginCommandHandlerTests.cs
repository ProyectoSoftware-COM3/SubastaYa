using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.UseCase.Commands.Auth.Login;
using SubastaYa.Domain.Entities;
using Xunit;

namespace SubastaYa.Tests.Application
{
    public class LoginCommandHandlerTests
    {
        private class FakeUserRepository : IUserRepository
        {
            public User? User;
            public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
                => Task.FromResult(User is not null && User.Email == email ? User : null);
            public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) => throw new NotImplementedException();
            public Task AddAsync(User user, CancellationToken ct = default) => throw new NotImplementedException();
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

       
        [Fact]
        public async Task Handle_EmailDoesNotExist_ThrowsInvalidCredentialsException()
        {
            var handler = new LoginCommandHandler(new FakeUserRepository { User = null }, new FakePasswordHasher(), new FakeJwtTokenGenerator());

            await Assert.ThrowsAsync<InvalidCredentialsException>(
                () => handler.Handle(new LoginCommand("doesnotexist@test.com", "123456"), CancellationToken.None));
        }

        
        [Fact]
        public async Task Handle_WrongPassword_ThrowsInvalidCredentialsException()
        {
            var user = new User { Id = Guid.NewGuid(), Email = "ana@test.com", Name = "Ana", PasswordHash = "HASH:correct" };
            var handler = new LoginCommandHandler(new FakeUserRepository { User = user }, new FakePasswordHasher(), new FakeJwtTokenGenerator());

            await Assert.ThrowsAsync<InvalidCredentialsException>(
                () => handler.Handle(new LoginCommand("ana@test.com", "wrong"), CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ValidCredentials_ReturnsAuthDtoWithToken()
        {
            var user = new User { Id = Guid.NewGuid(), Email = "ana@test.com", Name = "Ana", PasswordHash = "HASH:correct" };
            var handler = new LoginCommandHandler(new FakeUserRepository { User = user }, new FakePasswordHasher(), new FakeJwtTokenGenerator());

            var result = await handler.Handle(new LoginCommand("ana@test.com", "correct"), CancellationToken.None);

            Assert.Equal($"token-{user.Id}", result.Token);
            Assert.Equal("Ana", result.Name);
        }
    }
}
