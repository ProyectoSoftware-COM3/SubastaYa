using MediatR;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.UseCase.Queries.Wallet.GetWalletMovements
{
    public record GetWalletMovementsQuery(Guid UserId) : IRequest<List<WalletMovementDto>>;
}