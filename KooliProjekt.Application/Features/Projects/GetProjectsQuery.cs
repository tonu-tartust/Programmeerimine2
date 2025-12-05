using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Projects
{
    public class GetProjectsQuery : IRequest<OperationResult<object>>
    {
        public int Id { get; set; }
    }
}
