using FluentValidation;
using FormBuilderService.Models.DTOs;

namespace FormBuilderService.Validators;

/// <summary>
/// Validator for UpdateFormRequest
/// </summary>
public class UpdateFormRequestValidator : AbstractValidator<UpdateFormRequest>
{
    public UpdateFormRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters")
            .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Category));
    }
}

