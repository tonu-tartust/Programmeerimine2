using System.Collections.Generic;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Employees
{
    public class EmployeesQuery : IRequest<OperationResult<PagedResult<Employee>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
