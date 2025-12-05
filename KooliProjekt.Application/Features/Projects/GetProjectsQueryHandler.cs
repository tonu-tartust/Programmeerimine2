using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Projects
{
    // Kasutab IProjectRepositoryt
    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, OperationResult<object>>
    {
        private readonly IProjectRepository _projectRepository;

        public GetProjectsQueryHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<OperationResult<object>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var result = new OperationResult<object>();
            var project = await _projectRepository.GetByIdAsync(request.Id);

            result.Value = new
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                Budget = project.Budget
            };

            return result;
        }
    }
}
