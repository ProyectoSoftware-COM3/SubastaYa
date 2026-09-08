using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.UseCase.Queries.Auctions.GetAuctionBids;
using SubastaYa.Application.UseCase.Commands.Auctions.CreateAuction;
using SubastaYa.Application.UseCase.Queries.Auctions.GetAuctionById;
using SubastaYa.Application.UseCase.Queries.Auctions.GetAuctions;
using SubastaYa.Domain.Enums;


namespace SubastaYa.Api.Controllers

{
    [ApiController]
        [Route("api/[controller]")]
        public class AuctionsController : ControllerBase
        {
            private readonly IMediator _mediator;
            private readonly ICurrentUserService _currentUser;

            public AuctionsController(IMediator mediator, ICurrentUserService currentUser)
            {
                _mediator = mediator;
                _currentUser = currentUser;
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateAuctionCommand command, CancellationToken ct)
        {
            var auctionId = await _mediator.Send(command with { SellerId = _currentUser.UserId }, ct);
            return CreatedAtAction(nameof(GetById), new { id = auctionId }, new { id = auctionId });
        }


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var currentUserId = _currentUser.IsAuthenticated ? _currentUser.UserId : (Guid?)null;
            return Ok(await _mediator.Send(new GetAuctionByIdQuery(id, currentUserId), ct));
        }

        [HttpGet("{id:guid}/bids")]
        public async Task<IActionResult> GetBids(Guid id, CancellationToken ct)
           => Ok(await _mediator.Send(new GetAuctionBidsQuery(id), ct));


    }
}