using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.UseCase.Commands.Auth.Register;

namespace SubastaYa.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterUserCommand command, CancellationToken ct)
            => Ok(await _mediator.Send(command, ct));
    }
}