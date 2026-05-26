using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Task
{
    // Task kustutamise käsu händler
    public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteTaskCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
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

            var task = await _dbContext.Taskss.FindAsync(new object[] { request.Id }, cancellationToken);
            if (task != null)
            {
                _dbContext.Taskss.Remove(task);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return result;
        }
    }
}
