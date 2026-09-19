using MediatR;

namespace SubastaYa.Application.UseCase.Commands.Auctions.UpdateAuction
{
    public record UpdateAuctionCommand(
        Guid AuctionId,
        Guid SellerId,
        string Title,
        string Description,
        string ImageUrl,
        Guid CategoryId,
        decimal BasePrice,
        decimal MinIncrement) : IRequest<Unit>;
}