using MediatR;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.UseCase.Commands.Auth.Register
{
    public record RegisterUserCommand(string Email, string Name, string Password) : IRequest<AuthDto>;
}