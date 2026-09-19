using FluentValidation;

namespace SubastaYa.Application.UseCase.Commands.Auctions.UpdateAuction
{
    public class UpdateAuctionCommandValidator : AbstractValidator<UpdateAuctionCommand>
    {
        public UpdateAuctionCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(120);
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.ImageUrl).NotEmpty();
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.BasePrice).GreaterThan(0);
            RuleFor(x => x.MinIncrement).GreaterThan(0);
        }
    }
}
