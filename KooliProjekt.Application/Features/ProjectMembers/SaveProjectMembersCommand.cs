using KooliProjekt.Application.Behaviors;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.ProjectMembers
{
    public class SaveProjectMembersCommand : IRequest<OperationResult>, ITransactional
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string RoleInProject { get; set; }
    }
}
