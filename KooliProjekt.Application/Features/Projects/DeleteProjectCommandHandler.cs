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
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            // Kustuta kõigepealt seotud Tasks ja ProjectMembers
            await _dbContext
                .Taskss
                .Where(t => t.ProjectId == request.Id)
                .ExecuteDeleteAsync();

            await _dbContext
                .ProjectMembers
                .Where(pm => pm.ProjectId == request.Id)
                .ExecuteDeleteAsync();

            await _dbContext
                .Projects
                .Where(p => p.Id == request.Id)
                .ExecuteDeleteAsync();

            return result;
        }
    }
}
