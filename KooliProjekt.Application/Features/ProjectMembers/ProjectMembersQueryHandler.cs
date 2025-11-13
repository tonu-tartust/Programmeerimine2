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

namespace KooliProjekt.Application.Features.ProjectMembers
{
    public class ProjectMembersQueryHandler : IRequestHandler<ProjectMembersQuery, OperationResult<PagedResult<ProjectMember>>>
    {
        private readonly ApplicationDbContext _dbContext;
        public ProjectMembersQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<PagedResult<ProjectMember>>> Handle(ProjectMembersQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<PagedResult<ProjectMember>>();
            result.Value = await _dbContext
                .ProjectMembers
                .OrderBy(list => list.Id)
                .GetPagedAsync(request.Page, request.PageSize);

            return result;
        }
    }
}
