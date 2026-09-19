using MediatR;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.UseCase.Commands.Auth.Login
{
    // [Auth/JWT]
    public record LoginCommand(string Email, string Password) : IRequest<AuthDto>;
}
