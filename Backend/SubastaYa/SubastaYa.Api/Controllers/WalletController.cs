using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using SubastaYa.Application.Common.Interfaces;
using SubastaYa.Application.UseCase.Queries.Wallet.GetWalletBalance;
using SubastaYa.Application.UseCase.Queries.Wallet.GetWalletMovements;
using SubastaYa.Application.UseCase.Commands.Wallet.DepositFunds;

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

        [HttpGet("movements")]
        public async Task<IActionResult> GetMovements(CancellationToken ct)
    => Ok(await _mediator.Send(new GetWalletMovementsQuery(_currentUser.UserId), ct));

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] decimal amount, CancellationToken ct)
    => Ok(await _mediator.Send(new DepositFundsCommand(_currentUser.UserId, amount), ct));
    }
}
