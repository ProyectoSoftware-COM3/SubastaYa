using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SubastaYa.Application.DTOs;

namespace SubastaYa.Application.UseCase.Queries.Auctions.GetAuctionBids
{
    public record GetAuctionBidsQuery(Guid AuctionId) : IRequest<List<BidDto>>;
}
