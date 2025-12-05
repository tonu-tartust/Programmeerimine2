using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Employees
{
    // Töötaja kustutamise käsu händler
    public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteEmployeeCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            await _dbContext
                .Employees
                .Where(e => e.Id == request.Id)
                .ExecuteDeleteAsync();

            return result;
        }
    }
}
