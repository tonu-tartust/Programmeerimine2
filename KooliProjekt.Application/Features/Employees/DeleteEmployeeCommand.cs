using KooliProjekt.Application.Behaviors;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Employees
{
    // Töötaja kustutamise käsk
    public class DeleteEmployeeCommand : IRequest<OperationResult>, ITransactional
    {
        public int Id { get; set; }
    }
}
