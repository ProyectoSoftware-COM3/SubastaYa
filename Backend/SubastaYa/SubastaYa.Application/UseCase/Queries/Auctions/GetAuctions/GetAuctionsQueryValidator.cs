using FluentValidation;

namespace SubastaYa.Application.UseCase.Queries.Auctions.GetAuctions
{
    public class GetAuctionsQueryValidator : AbstractValidator<GetAuctionsQuery>
    {
        public GetAuctionsQueryValidator()
        {
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
            RuleFor(x => x.MinPrice).GreaterThanOrEqualTo(0).When(x => x.MinPrice is not null);
            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(x => x.MinPrice ?? 0)
                .When(x => x.MaxPrice is not null)
                .WithMessage("El precio maximo debe ser mayor o igual al precio minimo.");
        }
    }
}