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

namespace KooliProjekt.Application.Features.Projects
{
    public class ProjectsQueryHandler : IRequestHandler<ProjectsQuery, OperationResult<PagedResult<Project>>>
    {
        private readonly ApplicationDbContext _dbContext;
        public ProjectsQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<PagedResult<Project>>> Handle(ProjectsQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<PagedResult<Project>>();
            result.Value = await _dbContext
                .Projects
                .OrderBy(list => list.Id)
                .GetPagedAsync(request.Page, request.PageSize);

            return result;
        }
    }
}
