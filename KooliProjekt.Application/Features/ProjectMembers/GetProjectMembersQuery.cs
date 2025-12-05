using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.ProjectMembers
{
    public class GetProjectMembersQuery : IRequest<OperationResult<object>>
    {
        public int Id { get; set; }
    }
}
