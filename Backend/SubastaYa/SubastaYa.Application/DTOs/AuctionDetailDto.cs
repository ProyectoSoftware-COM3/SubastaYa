using SubastaYa.Domain.Enums;

namespace SubastaYa.Application.DTOs
{
 
    public record AuctionDetailDto(
        Guid Id,
        string Title,
        string Description,
        string ImageUrl,
        string CategoryName,
        decimal BasePrice,
        decimal MinIncrement,
        decimal CurrentPrice,
        decimal SuggestedNextBid,
        DateTime StartDate,
        DateTime EndDate,
        AuctionStatus Status,
        IReadOnlyList<BidDto> BidHistory,
        
        bool? IsCurrentUserLeading);
}

