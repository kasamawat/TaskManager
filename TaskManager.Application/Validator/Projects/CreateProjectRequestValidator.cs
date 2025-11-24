using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Projects;

namespace TaskManager.Application.Validator.Projects
{
    public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
    {
        public CreateProjectRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Project name is required.")
                .MaximumLength(200);
            RuleFor(x => x.Description)
                .MaximumLength(1000)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));
        }
    }
}
