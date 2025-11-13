using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Features.Employees
{
    public class EmployeesQueryHandler : IRequestHandler<EmployeesQuery, OperationResult<PagedResult<Employee>>>
    {
        private readonly ApplicationDbContext _dbContext;
        public EmployeesQueryHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult<PagedResult<Employee>>> Handle(EmployeesQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<PagedResult<Employee>>();

            result.Value = await _dbContext
                .Employees
                .OrderBy(list => list.Id)
                .GetPagedAsync(request.Page, request.PageSize);

            return result;
        }
    }
}
