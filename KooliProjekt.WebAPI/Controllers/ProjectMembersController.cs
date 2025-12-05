using System.Threading.Tasks;
using KooliProjekt.Application.Features.ProjectMembers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KooliProjekt.WebAPI.Controllers
{
    public class ProjectMembersController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectMembersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("List")]
        public async Task<IActionResult> List([FromQuery] ProjectMembersQuery query)
        {
            var response = await _mediator.Send(query);

            return Result(response);
        }

        [HttpGet]
        [Route("Get")]
        public async Task<IActionResult> Get(int id)
        {
            var query = new GetProjectMembersQuery { Id = id };
            var response = await _mediator.Send(query);

            return Result(response);
        }

        [HttpPost]
        [Route("Save")]
        public async Task<IActionResult> Save(SaveProjectMembersCommand command)
        {
            var response = await _mediator.Send(command);

            return Result(response);
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteProjectMemberCommand { Id = id };
            var response = await _mediator.Send(command);

            return Result(response);
        }
    }
}
