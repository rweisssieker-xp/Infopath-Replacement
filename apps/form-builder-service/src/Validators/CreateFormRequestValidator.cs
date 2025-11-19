using FluentValidation;
using FormBuilderService.Models.DTOs;

namespace FormBuilderService.Validators;

/// <summary>
/// Validator for CreateFormRequest
/// </summary>
public class CreateFormRequestValidator : AbstractValidator<CreateFormRequest>
{
    public CreateFormRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(255).WithMessage("Name must not exceed 255 characters");

        RuleFor(x => x.Schema)
            .NotNull().WithMessage("Schema is required")
            .Must(BeValidJson).WithMessage("Schema must be valid JSON");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category must not exceed 100 characters")
            .When(x => !string.IsNullOrEmpty(x.Category));
    }

    private static bool BeValidJson(Dictionary<string, object> schema)
    {
        // Basic validation - schema should not be empty
        // More sophisticated JSON Schema validation can be added later
        return schema != null && schema.Count > 0;
    }
}

