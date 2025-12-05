using FluentValidation;
using KooliProjekt.Application.Data;

namespace KooliProjekt.Application.Features.Projects
{
    // Valideerimise klass SaveProjectsCommand käsu jaoks
    public class SaveProjectsCommandValidator : AbstractValidator<SaveProjectsCommand>
    {
        public SaveProjectsCommandValidator(ApplicationDbContext context)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required")
                .MaximumLength(200).WithMessage("Project name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be after start date");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .MaximumLength(50).WithMessage("Status cannot exceed 50 characters");

            RuleFor(x => x.Budget)
                .GreaterThanOrEqualTo(0).WithMessage("Budget must be a positive value");
        }
    }
}
