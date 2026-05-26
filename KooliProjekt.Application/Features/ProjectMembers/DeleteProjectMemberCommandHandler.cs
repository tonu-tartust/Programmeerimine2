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
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult> Handle(DeleteProjectMemberCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult();

            if (request.Id <= 0)
            {
                return result;
            }

            var pm = await _dbContext.ProjectMembers.FindAsync(new object[] { request.Id }, cancellationToken);
            if (pm != null)
            {
                _dbContext.ProjectMembers.Remove(pm);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
    }
}
