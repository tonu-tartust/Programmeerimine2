using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Task
{
    // Kasutab ITaskRepositoryt
    public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, OperationResult<object>>
    {
        private readonly ITaskRepository _taskRepository;

        public GetTasksQueryHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<OperationResult<object>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();
            var task = await _taskRepository.GetByIdAsync(request.Id);

            result.Value = new
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                Title = task.Title,
                Description = task.Description,
                AssignedTo = task.AssignedTo,
                StartDate = task.StartDate,
                DueDate = task.DueDate,
                Status = task.Status,
                Priority = task.Priority
            };

            return result;
        }
    }
}
