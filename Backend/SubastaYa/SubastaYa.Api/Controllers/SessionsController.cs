using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubastaYa.Application.UseCase.Commands.Auth.Login;

namespace SubastaYa.Api.Controllers
{
    // Antes POST /api/auth/login (verbo en la URL, prohibido). Loguearse es, en
    // terminos REST, crear un recurso Session nuevo (la sesion autenticada representada
    // por el JWT que se devuelve): POST /api/sessions es el patron estandar para esto,
    // el mismo que usan por ejemplo las Rails apps para modelar el login sin verbos.
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

