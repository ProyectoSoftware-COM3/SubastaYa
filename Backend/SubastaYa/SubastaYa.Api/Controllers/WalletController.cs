using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.UseCase.Queries.Wallet.GetWalletBalance;

namespace SubastaYa.Api.Controllers
{
    [ApiController]
    [Route("api/wallet")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public WalletController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance(CancellationToken ct)
            => Ok(await _mediator.Send(new GetWalletBalanceQuery(_currentUser.UserId), ct));
    }
}
