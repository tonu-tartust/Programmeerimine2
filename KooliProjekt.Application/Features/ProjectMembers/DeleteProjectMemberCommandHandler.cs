using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.ProjectMembers
{
    // ProjectMember kustutamise käsu händler
    public class DeleteProjectMemberCommandHandler : IRequestHandler<DeleteProjectMemberCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteProjectMemberCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(DeleteProjectMemberCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            await _dbContext
                .ProjectMembers
                .Where(pm => pm.Id == request.Id)
                .ExecuteDeleteAsync();

            return result;
        }
    }
}
