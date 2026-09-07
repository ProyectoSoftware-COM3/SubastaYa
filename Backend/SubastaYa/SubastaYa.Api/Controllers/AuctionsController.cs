using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.UseCase.Queries.Auctions.GetAuctions;
using SubastaYa.Domain.Enums;

namespace SubastaYa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuctionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] AuctionStatus? status, [FromQuery] Guid? categoryId,
            [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice,
            [FromQuery] AuctionSortOrder sort = AuctionSortOrder.LeastTimeRemaining,
            [FromQuery] int page = 1, [FromQuery] int pageSize = 12,
            CancellationToken ct = default)
        {
            var query = new GetAuctionsQuery(status, categoryId, minPrice, maxPrice, sort, page, pageSize);
            return Ok(await _mediator.Send(query, ct));
        }

        
    }
}