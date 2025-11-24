using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.DTOs.Tasks;

namespace TaskManager.Application.Validator.Tasks
{
    public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
    {
        public CreateTaskRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Task title is required.");
            RuleFor(x => x.Detail)
                .MaximumLength(2000)
                .When(x => !string.IsNullOrEmpty(x.Detail));
            RuleFor(x => x.DueDate)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.DueDate.HasValue)
                .WithMessage("Due date must be in the future");
        }
    }
}
