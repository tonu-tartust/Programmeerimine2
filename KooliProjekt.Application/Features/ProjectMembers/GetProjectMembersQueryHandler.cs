using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.ProjectMembers
{
    // Kasutab IProjectMemberRepositoryt
    public class GetProjectMembersQueryHandler : IRequestHandler<GetProjectMembersQuery, OperationResult<object>>
    {
        private readonly IProjectMemberRepository _projectMemberRepository;

        public GetProjectMembersQueryHandler(IProjectMemberRepository projectMemberRepository)
        {
            _projectMemberRepository = projectMemberRepository;
        }

        public async Task<OperationResult<object>> Handle(GetProjectMembersQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();
            var projectMember = await _projectMemberRepository.GetByIdAsync(request.Id);

            result.Value = new
            {
                Id = projectMember.Id,
                ProjectId = projectMember.ProjectId,
                EmployeeId = projectMember.EmployeeId,
                RoleInProject = projectMember.RoleInProject
            };

            return result;
        }
    }
}
