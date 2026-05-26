using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Projects
{
    // Projekti kustutamise käsu händler
    public class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteProjectCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
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

            var project = await _dbContext.Projects.FindAsync(new object[] { request.Id }, cancellationToken);
            if (project == null)
            {
                return result;
            }

            // Kustuta kõigepealt seotud Tasks ja ProjectMembers
            var tasks = await _dbContext.Taskss.Where(t => t.ProjectId == request.Id).ToListAsync(cancellationToken);
            _dbContext.Taskss.RemoveRange(tasks);

            var members = await _dbContext.ProjectMembers.Where(pm => pm.ProjectId == request.Id).ToListAsync(cancellationToken);
            _dbContext.ProjectMembers.RemoveRange(members);

            _dbContext.Projects.Remove(project);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return result;
        }
    }
}
