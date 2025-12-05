using KooliProjekt.Application.Behaviors;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Task
{
    // Task kustutamise käsk
    public class DeleteTaskCommand : IRequest<OperationResult>, ITransactional
    {
        public int Id { get; set; }
    }
}
