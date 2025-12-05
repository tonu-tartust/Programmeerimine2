using KooliProjekt.Application.Behaviors;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.ProjectMembers
{
    // ProjectMember kustutamise käsk
    public class DeleteProjectMemberCommand : IRequest<OperationResult>, ITransactional
    {
        public int Id { get; set; }
    }
}
