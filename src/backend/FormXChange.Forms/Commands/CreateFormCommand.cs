using MediatR;
using FormXChange.Shared.Models;
using FluentValidation;
using FormXChange.Shared.Data;

namespace FormXChange.Forms.Commands;

public class CreateFormCommand : IRequest<Form>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Schema { get; set; } = "{}";
    public Guid TenantId { get; set; }
    public Guid CreatedBy { get; set; }
    public string? BranchName { get; set; }
    public bool IsTemplate { get; set; } = false;
}

public class CreateFormCommandValidator : AbstractValidator<CreateFormCommand>
{
    public CreateFormCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Schema).NotEmpty();
        RuleFor(x => x.TenantId).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}

public class CreateFormCommandHandler : IRequestHandler<CreateFormCommand, Form>
{
    private readonly FormXChange.Shared.Data.ApplicationDbContext _context;
    private readonly ILogger<CreateFormCommandHandler> _logger;

    public CreateFormCommandHandler(
        FormXChange.Shared.Data.ApplicationDbContext context,
        ILogger<CreateFormCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Form> Handle(CreateFormCommand request, CancellationToken cancellationToken)
    {
        var form = new Form
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Schema = request.Schema,
            TenantId = request.TenantId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            Version = 1,
            BranchName = request.BranchName,
            IsActive = true,
            IsTemplate = request.IsTemplate,
            Status = "Draft"
        };

        _context.Forms.Add(form);
        await _context.SaveChangesAsync(cancellationToken);

        // Create initial version
        var version = new FormVersion
        {
            Id = Guid.NewGuid(),
            FormId = form.Id,
            VersionNumber = 1,
            Schema = form.Schema,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            BranchName = request.BranchName
        };

        _context.FormVersions.Add(version);
        await _context.SaveChangesAsync(cancellationToken);

        return form;
    }
}

