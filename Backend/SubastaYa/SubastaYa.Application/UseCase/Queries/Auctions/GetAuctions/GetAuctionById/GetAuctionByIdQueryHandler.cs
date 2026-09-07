using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SubastaYa.Application.Common;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;
using SubastaYa.Application.Exceptions;

namespace SubastaYa.Application.UseCase.Queries.Auctions.GetAuctionById
{
    public class GetAuctionByIdQueryHandler : IRequestHandler<GetAuctionByIdQuery, AuctionDetailDto>
    {
        private readonly IAuctionRepository _auctionRepository;

        public GetAuctionByIdQueryHandler(IAuctionRepository auctionRepository)
            => _auctionRepository = auctionRepository;

        public async Task<AuctionDetailDto> Handle(GetAuctionByIdQuery request, CancellationToken cancellationToken)
        {
            var auction = await _auctionRepository.GetByIdWithBidsAsync(request.AuctionId, cancellationToken)
                ?? throw new AuctionNotFoundException(request.AuctionId);

            var history = auction.Bids
                .OrderBy(b => b.PlacedAt)
                .Select(b => new BidDto(b.Id, BidAnonymizer.Anonymize(b.Bidder.Name), b.Amount, b.PlacedAt))
                .ToList();

            
            var currentPrice = auction.Bids.Count > 0 ? auction.Bids.Max(b => b.Amount) : auction.BasePrice;

            
            bool? isCurrentUserLeading = null;
            if (request.CurrentUserId is Guid userId)
            {
                var myHighestBid = auction.Bids.Where(b => b.BidderId == userId).MaxBy(b => b.Amount);
                if (myHighestBid is not null)
                    isCurrentUserLeading = myHighestBid.Amount == currentPrice;
            }

            return new AuctionDetailDto(
                auction.Id, auction.Title, auction.Description, auction.ImageUrl,
                auction.Category.Name, auction.BasePrice, auction.MinIncrement,
                currentPrice, currentPrice + auction.MinIncrement,
                auction.StartDate, auction.EndDate, auction.Status, history, isCurrentUserLeading);
        }
    }
}
