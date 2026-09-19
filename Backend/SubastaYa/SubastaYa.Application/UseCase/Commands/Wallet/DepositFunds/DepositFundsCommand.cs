using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.UseCase.Commands.Wallet.DepositFunds
{
   
    public record DepositFundsCommand(Guid UserId, decimal Amount) : IRequest<WalletBalanceDto>;
}

