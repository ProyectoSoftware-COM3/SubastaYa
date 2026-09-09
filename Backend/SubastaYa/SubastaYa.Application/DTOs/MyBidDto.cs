using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.DTOs
{
    public record MyBidDto(Guid AuctionId, string AuctionTitle, decimal MyLastBid, bool IsOpen, bool IsCurrentlyWinning, bool Won);
}
