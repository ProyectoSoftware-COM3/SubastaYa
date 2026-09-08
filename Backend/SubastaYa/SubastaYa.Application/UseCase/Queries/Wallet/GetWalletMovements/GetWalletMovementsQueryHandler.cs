using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;

namespace SubastaYa.Application.UseCase.Queries.Wallet.GetWalletMovements
{
    public class GetWalletMovementsQueryHandler : IRequestHandler<GetWalletMovementsQuery, List<WalletMovementDto>>
    {
        private readonly IWalletRepository _walletRepository;

        public GetWalletMovementsQueryHandler(IWalletRepository walletRepository)
            => _walletRepository = walletRepository;

        public async Task<List<WalletMovementDto>> Handle(GetWalletMovementsQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken)
                ?? throw new WalletNotFoundException(request.UserId);

            var movements = await _walletRepository.GetMovementsAsync(wallet.Id, cancellationToken);

            return movements
                .OrderByDescending(m => m.OccurredAt)
                .Select(m => new WalletMovementDto(m.Id, m.Type, m.Amount, m.OccurredAt, m.AuctionId))
                .ToList();
        }
    }
}