using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaYa.Domain.Enums;

namespace SubastaYa.Application.DTOs
{

    public record AuctionCardDto(
        Guid Id,
        string Title,
        string ImageUrl,
        string CategoryName,
        decimal CurrentPrice,
        int BidCount,
        DateTime EndDate,
        AuctionStatus Status);
}
