using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace SubastaYa.Application.UseCase.Commands.Wallet.DepositFunds
{
    public class DepositFundsCommandValidator : AbstractValidator<DepositFundsCommand>
    {
        public DepositFundsCommandValidator()
        {
            RuleFor(x => x.Amount).GreaterThan(0)
                .WithMessage("El monto a depositar debe ser positivo.");
        }
    }
}

