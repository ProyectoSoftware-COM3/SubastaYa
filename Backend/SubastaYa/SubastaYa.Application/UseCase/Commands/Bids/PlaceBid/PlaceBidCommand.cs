using MediatR;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.UseCase.Commands.Bids.PlaceBid
{
    public record PlaceBidCommand(Guid AuctionId, Guid BidderId, decimal Amount) : IRequest<BidDto>;
}