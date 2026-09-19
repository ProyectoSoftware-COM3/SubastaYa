using MediatR;
using SubastaYa.Application.Common.Filters;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.UseCase.Queries.Auctions.GetAuctions
{
    public class GetAuctionsQueryHandler : IRequestHandler<GetAuctionsQuery, PagedResult<AuctionCardDto>>
    {
        private readonly IAuctionRepository _auctionRepository;

        public GetAuctionsQueryHandler(IAuctionRepository auctionRepository)
            => _auctionRepository = auctionRepository;

        public async Task<PagedResult<AuctionCardDto>> Handle(GetAuctionsQuery request, CancellationToken cancellationToken)
        {
            var filter = new AuctionFilter(
                request.Status, request.CategoryId, request.MinPrice, request.MaxPrice,
                request.Sort, request.Page, request.PageSize);

            var (items, totalCount) = await _auctionRepository.GetFilteredAsync(filter, cancellationToken);

            var cards = items.Select(ToCard).ToList();
            return new PagedResult<AuctionCardDto>(cards, request.Page, request.PageSize, totalCount);
        }

        private static AuctionCardDto ToCard(Auction a)
        {
            var currentPrice = a.Bids.Count > 0 ? a.Bids.Max(b => b.Amount) : a.BasePrice;
            return new(a.Id, a.Title, a.ImageUrl, a.Category.Name, currentPrice, a.Bids.Count, a.EndDate, a.Status);
        }
    }
}