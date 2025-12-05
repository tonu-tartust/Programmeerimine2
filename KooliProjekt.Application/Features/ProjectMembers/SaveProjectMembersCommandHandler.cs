using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.ProjectMembers
{
    // Kasutab IProjectMemberRepositoryt
    public class SaveProjectMembersCommandHandler : IRequestHandler<SaveProjectMembersCommand, OperationResult>
    {
        private readonly IProjectMemberRepository _projectMemberRepository;

        public SaveProjectMembersCommandHandler(IProjectMemberRepository projectMemberRepository)
        {
            _projectMemberRepository = projectMemberRepository;
        }

        public async Task<OperationResult> Handle(SaveProjectMembersCommand request, CancellationToken cancellationToken)
        {
            var result = new OperationResult();

            var projectMember = new ProjectMember();
            if (request.Id != 0)
            {
                projectMember = await _projectMemberRepository.GetByIdAsync(request.Id);
            }

            projectMember.ProjectId = request.ProjectId;
            projectMember.EmployeeId = request.EmployeeId;
            projectMember.RoleInProject = request.RoleInProject;

            await _projectMemberRepository.SaveAsync(projectMember);

            return result;
        }
    }
}
