using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.ToDoLists
{
    public class GetProjectMembersQueryHandler : IRequestHandler<GetProjectMembersQuery, OperationResult<object>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetProjectMembersQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<object>> Handle(GetProjectMembersQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            result.Value = await _dbContext
                .ProjectMembers
                .Include(list => list.Id)
                .Where(list => list.Id == request.Id)
                .Select(list => new
                {
                    Id = list.Id,
                })
                .FirstOrDefaultAsync();

            return result;
        }
    }
}
