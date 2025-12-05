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
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            await _dbContext
                .Taskss
                .Where(t => t.Id == request.Id)
                .ExecuteDeleteAsync();

            return result;
        }
    }
}
