using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using TaskEntity = KooliProjekt.Application.Data.Tasks;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace KooliProjekt.Application.Features.Task
{
    public class GetTasksQueryHandler : IRequestHandler<GetTasksQuery, OperationResult<object>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetTasksQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult<object>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Id <= 0)
            {
                return result;
            }

            var task = await _dbContext.Set<TaskEntity>().FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (task == null)
            {
                return result;
            }

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
