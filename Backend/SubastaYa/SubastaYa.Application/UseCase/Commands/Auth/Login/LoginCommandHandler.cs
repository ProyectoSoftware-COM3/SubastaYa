using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;

namespace SubastaYa.Application.UseCase.Commands.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken)
                ?? throw new InvalidCredentialsException();

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                throw new InvalidCredentialsException();

            var (token, expiresAt) = _jwtTokenGenerator.Generate(user);
            return new AuthDto(token, expiresAt, user.Id, user.Name, user.Email);
        }
    }
}