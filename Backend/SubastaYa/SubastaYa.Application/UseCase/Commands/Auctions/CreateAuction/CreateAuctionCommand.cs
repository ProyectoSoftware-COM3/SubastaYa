using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SubastaYa.Application.UseCase.Commands.Auctions.CreateAuction
{
    public record CreateAuctionCommand(
        Guid SellerId,
        string Title,
        string Description,
        string ImageUrl,
        Guid CategoryId,
        decimal BasePrice,
        decimal MinIncrement,
        DateTime StartDate,
        DateTime EndDate) : IRequest<Guid>;
}
