using FluentValidation;
using KooliProjekt.Application.Data;

namespace KooliProjekt.Application.Features.Task
{
    // Valideerimise klass SaveTasksCommand käsu jaoks
    public class SaveTasksCommandValidator : AbstractValidator<SaveTasksCommand>
    {
        public SaveTasksCommandValidator(ApplicationDbContext context)
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0).WithMessage("Project ID is required");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

            RuleFor(x => x.AssignedTo)
                .GreaterThan(0).WithMessage("AssignedTo (Employee ID) is required");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required");

            RuleFor(x => x.DueDate)
                .NotEmpty().WithMessage("Due date is required")
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Due date must be after start date");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required")
                .MaximumLength(50).WithMessage("Status cannot exceed 50 characters");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required")
                .MaximumLength(50).WithMessage("Priority cannot exceed 50 characters");
        }
    }
}
