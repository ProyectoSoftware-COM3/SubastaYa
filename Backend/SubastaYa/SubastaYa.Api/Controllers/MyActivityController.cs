using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.UseCase.Queries.MyActivity.GetMyAuctions;
using SubastaYa.Application.UseCase.Queries.MyActivity.GetMyBids;

namespace SubastaYa.Api.Controllers
{
    [ApiController]
    [Route("api/my-activity")]
    [Authorize]
    public class MyActivityController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public MyActivityController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [HttpGet("bids")]
        public async Task<IActionResult> GetMyBids(CancellationToken ct)
            => Ok(await _mediator.Send(new GetMyBidsQuery(_currentUser.UserId), ct));

        [HttpGet("auctions")]
        public async Task<IActionResult> GetMyAuctions(CancellationToken ct)
        => Ok(await _mediator.Send(new GetMyAuctionsQuery(_currentUser.UserId), ct));
    }
}