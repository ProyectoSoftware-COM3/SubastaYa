using Microsoft.AspNetCore.Mvc;
using MediatR;
using SubastaYa.Application.UseCase.Queries.Categories.GetCategories;

namespace SubastaYa.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public CategoriesController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
            => Ok(await _mediator.Send(new GetCategoriesQuery(), ct));
    }
}
