using FluentValidation;
using KooliProjekt.Application.Data;

namespace KooliProjekt.Application.Features.Employees
{
    // Valideerimise klass SaveEmployeesCommand käsu jaoks
    public class SaveEmployeesCommandValidator : AbstractValidator<SaveEmployeesCommand>
    {
        public SaveEmployeesCommandValidator(ApplicationDbContext context)
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required")
                .MaximumLength(50).WithMessage("Phone cannot exceed 50 characters");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .MaximumLength(100).WithMessage("Role cannot exceed 100 characters");
        }
    }
}
