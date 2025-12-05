using KooliProjekt.Application.Behaviors;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Projects
{
    // Projekti kustutamise käsk
    public class DeleteProjectCommand : IRequest<OperationResult>, ITransactional
    {
        public int Id { get; set; }
    }
}
