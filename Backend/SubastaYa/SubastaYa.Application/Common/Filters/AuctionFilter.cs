using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubastaYa.Domain.Enums;

namespace SubastaYa.Application.Common.Filters
{
    public record AuctionFilter(
       AuctionStatus? Status,
       Guid? CategoryId,
       decimal? MinPrice,
       decimal? MaxPrice,
       AuctionSortOrder Sort,
       int Page,
       int PageSize);
}
