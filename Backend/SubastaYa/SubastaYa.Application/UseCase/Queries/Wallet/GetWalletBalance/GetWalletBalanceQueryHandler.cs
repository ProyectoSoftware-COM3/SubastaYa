using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;

namespace SubastaYa.Application.UseCase.Queries.Wallet.GetWalletBalance
{
    public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, WalletBalanceDto>
    {
        private readonly IWalletRepository _walletRepository;

        public GetWalletBalanceQueryHandler(IWalletRepository walletRepository)
            => _walletRepository = walletRepository;

        public async Task<WalletBalanceDto> Handle(GetWalletBalanceQuery request, CancellationToken cancellationToken)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(request.UserId, cancellationToken)
                ?? throw new WalletNotFoundException(request.UserId);

            return new WalletBalanceDto(wallet.TotalBalance, wallet.HeldBalance, wallet.AvailableBalance);
        }
    }
}

