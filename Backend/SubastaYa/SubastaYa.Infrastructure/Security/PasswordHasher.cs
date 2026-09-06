using SubastaYa.Application.Common.Interfaces;

namespace SubastaYa.Infrastructure.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string plainPassword)
            => BCrypt.Net.BCrypt.HashPassword(plainPassword);

        public bool Verify(string plainPassword, string passwordHash)
            => BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
    }
}