using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;
using SubastaYa.Domain.Exceptions;

namespace SubastaYa.Application.UseCase.Commands.Auth.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, AuthDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existing is not null)
                throw new EmailAlreadyRegisteredException(request.Email);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    Name = request.Name,
                    PasswordHash = _passwordHasher.Hash(request.Password),
                    RegisteredAt = DateTime.UtcNow,
                    Wallet = new Domain.Entities.Wallet { Id = Guid.NewGuid(), TotalBalance = 0, HeldBalance = 0, AvailableBalance = 0 }
                };

                await _userRepository.AddAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitAsync(cancellationToken);

                var (token, expiresAt) = _jwtTokenGenerator.Generate(user);
                return new AuthDto(token, expiresAt, user.Id, user.Name, user.Email);
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}