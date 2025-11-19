using FluentValidation;
using FormRuntimeService.Models.DTOs;

namespace FormRuntimeService.Validators;

/// <summary>
/// Validator for CreateSubmissionRequest
/// </summary>
public class CreateSubmissionRequestValidator : AbstractValidator<CreateSubmissionRequest>
{
    public CreateSubmissionRequestValidator()
    {
        RuleFor(x => x.FormId)
            .NotEmpty().WithMessage("FormId is required");

        RuleFor(x => x.Data)
            .NotNull().WithMessage("Data is required")
            .Must(d => d != null && d.Count > 0).WithMessage("Data must not be empty");
    }
}

