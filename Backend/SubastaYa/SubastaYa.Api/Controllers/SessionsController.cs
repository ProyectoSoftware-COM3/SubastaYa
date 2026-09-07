using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.UseCase.Commands.Auth.Login;

namespace SubastaYa.Api.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    public class SessionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SessionsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LoginCommand command, CancellationToken ct)
            => Ok(await _mediator.Send(command, ct));
    }
}

