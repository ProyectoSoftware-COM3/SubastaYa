using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.DTOs
{
    public record BidDto(Guid Id, string BidderAlias, decimal Amount, DateTime PlacedAt);
}
