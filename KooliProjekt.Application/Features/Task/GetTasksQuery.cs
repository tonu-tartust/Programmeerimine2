using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.ToDoLists
{
    public class GetTasksQuery : IRequest<OperationResult<object>>
    {
        public int Id { get; set; }
    }
}
