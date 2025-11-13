using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.ToDoLists
{
    public class GetProjectMembersQuery : IRequest<OperationResult<object>>
    {
        public int Id { get; set; }
    }
}
