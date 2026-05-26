using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace KooliProjekt.Application.Features.ProjectMembers
{
    public class GetProjectMembersQueryHandler : IRequestHandler<GetProjectMembersQuery, OperationResult<object>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetProjectMembersQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult<object>> Handle(GetProjectMembersQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Id <= 0)
            {
                return result;
            }

            var projectMember = await _dbContext.ProjectMembers.FirstOrDefaultAsync(pm => pm.Id == request.Id, cancellationToken); // Note: Should be pm.Id

            if (projectMember == null)
            {
                return result;
            }

            result.Value = new
            {
                Id = projectMember.Id,
                ProjectId = projectMember.ProjectId,
                EmployeeId = projectMember.EmployeeId,
                RoleInProject = projectMember.RoleInProject
            };

            return result;
        }
    }
}
