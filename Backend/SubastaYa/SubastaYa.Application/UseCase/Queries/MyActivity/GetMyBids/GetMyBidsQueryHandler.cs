using MediatR;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Enums;

namespace SubastaYa.Application.UseCase.Queries.MyActivity.GetMyBids
{
    public class GetMyBidsQueryHandler : IRequestHandler<GetMyBidsQuery, List<MyBidDto>>
    {
        private readonly IBidRepository _bidRepository;

        public GetMyBidsQueryHandler(IBidRepository bidRepository)
            => _bidRepository = bidRepository;

        public async Task<List<MyBidDto>> Handle(GetMyBidsQuery request, CancellationToken cancellationToken)
        {
            var myBids = await _bidRepository.GetByBidderIdAsync(request.UserId, cancellationToken);

            return myBids
                .GroupBy(b => b.Auction)
                .Select(g =>
                {
                    var myLastBid = g.Max(b => b.Amount);
                    var isOpen = g.Key.Status == AuctionStatus.Active;
                    var currentPrice = g.Key.Bids.Count > 0 ? g.Key.Bids.Max(b => b.Amount) : g.Key.BasePrice;
                    var won = g.Key.Status == AuctionStatus.Finished && currentPrice == myLastBid;
                    var isCurrentlyWinning = isOpen && currentPrice == myLastBid;
                    return new MyBidDto(g.Key.Id, g.Key.Title, myLastBid, isOpen, isCurrentlyWinning, won);
                })
                .ToList();
        }
    }
}