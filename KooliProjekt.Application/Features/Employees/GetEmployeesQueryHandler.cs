using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;

namespace KooliProjekt.Application.Features.Employees
{
    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, OperationResult<object>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetEmployeesQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult<object>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Id <= 0)
            {
                return result;
            }

            var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (employee == null)
            {
                return result;
            }

            result.Value = new
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                Role = employee.Role
            };

            return result;
        }
    }
}
