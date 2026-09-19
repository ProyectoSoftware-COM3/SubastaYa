using MediatR;
using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Enums;

namespace SubastaYa.Application.UseCase.Queries.Auctions.GetAuctions
{
    public record GetAuctionsQuery(
        AuctionStatus? Status,
        Guid? CategoryId,
        decimal? MinPrice,
        decimal? MaxPrice,
        AuctionSortOrder Sort,
        int Page = 1,
        int PageSize = 12) : IRequest<PagedResult<AuctionCardDto>>;
}