using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace SubastaYa.Application.UseCase.Commands.Auctions.CreateAuction
{
    public class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand>
    {
        public CreateAuctionCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.ImageUrl).NotEmpty();
            RuleFor(x => x.BasePrice).GreaterThan(0);
            RuleFor(x => x.MinIncrement).GreaterThan(0);
            RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate)
                .WithMessage("La fecha de finalizacion debe ser posterior a la de inicio.");
        }
    }
}
