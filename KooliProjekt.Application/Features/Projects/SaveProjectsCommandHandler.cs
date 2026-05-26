using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Infrastructure.Results;
using MediatR;

namespace KooliProjekt.Application.Features.Projects
{
    // Kasutab IProjectRepositoryt
    public class SaveProjectsCommandHandler : IRequestHandler<SaveProjectsCommand, OperationResult>
    {
        private readonly IProjectRepository _projectRepository;

        public SaveProjectsCommandHandler(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        }

        public async Task<OperationResult> Handle(SaveProjectsCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result = new OperationResult();

            var project = new Project();
            if (request.Id != 0)
            {
                project = await _projectRepository.GetByIdAsync(request.Id);
                if (project == null)
                {
                    result.AddError("Not found");
                    return result;
                }
            }

            project.Name = request.Name;
            project.Description = request.Description;
            project.StartDate = request.StartDate;
            project.EndDate = request.EndDate;
            project.Status = request.Status;
            project.Budget = request.Budget;

            await _projectRepository.SaveAsync(project);

            return result;
        }
    }
}
