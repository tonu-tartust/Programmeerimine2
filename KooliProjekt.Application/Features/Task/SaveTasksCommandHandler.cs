using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Task
{
    // Kasutab ITaskRepositoryt
    public class SaveTasksCommandHandler : IRequestHandler<SaveTasksCommand, OperationResult>
    {
        private readonly ITaskRepository _taskRepository;

        public SaveTasksCommandHandler(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<OperationResult> Handle(SaveTasksCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            var task = new Tasks();
            if (request.Id != 0)
            {
                task = await _taskRepository.GetByIdAsync(request.Id);
            }

            task.ProjectId = request.ProjectId;
            task.Title = request.Title;
            task.Description = request.Description;
            task.AssignedTo = request.AssignedTo;
            task.StartDate = request.StartDate;
            task.DueDate = request.DueDate;
            task.Status = request.Status;
            task.Priority = request.Priority;

            await _taskRepository.SaveAsync(task);

            return result;
        }
    }
}
