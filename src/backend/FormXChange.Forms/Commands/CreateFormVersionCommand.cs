using MediatR;
using FormXChange.Shared.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FormXChange.Shared.Data;

namespace FormXChange.Forms.Commands;

public class CreateFormVersionCommand : IRequest<FormVersion>
{
    public Guid FormId { get; set; }
    public string Schema { get; set; } = "{}";
    public string? ChangeDescription { get; set; }
    public Guid CreatedBy { get; set; }
    public string? BranchName { get; set; }
}

public class CreateFormVersionCommandValidator : AbstractValidator<CreateFormVersionCommand>
{
    public CreateFormVersionCommandValidator()
    {
        RuleFor(x => x.FormId).NotEmpty();
        RuleFor(x => x.Schema).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}

public class CreateFormVersionCommandHandler : IRequestHandler<CreateFormVersionCommand, FormVersion>
{
    private readonly FormXChange.Shared.Data.ApplicationDbContext _context;
    private readonly ILogger<CreateFormVersionCommandHandler> _logger;

    public CreateFormVersionCommandHandler(
        FormXChange.Shared.Data.ApplicationDbContext context,
        ILogger<CreateFormVersionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FormVersion> Handle(CreateFormVersionCommand request, CancellationToken cancellationToken)
    {
        var form = await _context.Forms.FindAsync(new object[] { request.FormId }, cancellationToken);
        if (form == null)
            throw new InvalidOperationException($"Form with ID {request.FormId} not found");

        // Get next version number
        var maxVersion = await _context.FormVersions
            .Where(v => v.FormId == request.FormId)
            .Select(v => v.VersionNumber)
            .DefaultIfEmpty(0)
            .MaxAsync(cancellationToken);

        var version = new FormVersion
        {
            Id = Guid.NewGuid(),
            FormId = request.FormId,
            VersionNumber = maxVersion + 1,
            Schema = request.Schema,
            ChangeDescription = request.ChangeDescription,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            BranchName = request.BranchName ?? form.BranchName
        };

        _context.FormVersions.Add(version);
        
        // Update form version
        form.Version = version.VersionNumber;
        form.Schema = request.Schema;
        form.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return version;
    }
}

