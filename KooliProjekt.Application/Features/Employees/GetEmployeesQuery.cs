using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Employees
{
    public class GetEmployeesQuery : IRequest<OperationResult<object>>
    {
        public int Id { get; set; }
    }
}
