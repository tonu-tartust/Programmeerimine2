using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Task
{
    public class TasksQueryHandler : IRequestHandler<TasksQuery, OperationResult<PagedResult<Tasks>>>
    {
        private readonly ApplicationDbContext _dbContext;
        public TasksQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<PagedResult<Tasks>>> Handle(TasksQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<PagedResult<Tasks>>();
            result.Value = await _dbContext
                .Taskss
                .OrderBy(list => list.Id)
                .GetPagedAsync(request.Page, request.PageSize);

            return result;
        }
    }
}
