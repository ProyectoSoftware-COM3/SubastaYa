using MediatR;
using SubastaYa.Application.Common;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;

namespace SubastaYa.Application.UseCase.Queries.Auctions.GetAuctionBids
{
    public class GetAuctionBidsQueryHandler : IRequestHandler<GetAuctionBidsQuery, List<BidDto>>
    {
        private readonly IAuctionRepository _auctionRepository;

        public GetAuctionBidsQueryHandler(IAuctionRepository auctionRepository)
            => _auctionRepository = auctionRepository;

        public async Task<List<BidDto>> Handle(GetAuctionBidsQuery request, CancellationToken cancellationToken)
        {
            var auction = await _auctionRepository.GetByIdWithBidsAsync(request.AuctionId, cancellationToken)
                ?? throw new AuctionNotFoundException(request.AuctionId);

            return auction.Bids
                .OrderBy(b => b.PlacedAt)
                .Select(b => new BidDto(b.Id, BidAnonymizer.Anonymize(b.Bidder.Name), b.Amount, b.PlacedAt))
                .ToList();
        }
    }
}