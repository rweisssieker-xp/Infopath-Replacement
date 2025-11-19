using MediatR;
using FormXChange.Shared.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FormXChange.Shared.Data;

namespace FormXChange.Forms.Commands;

public class CreateFormBranchCommand : IRequest<FormBranch>
{
    public Guid FormId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentBranchId { get; set; }
    public Guid CreatedBy { get; set; }
}

public class CreateFormBranchCommandValidator : AbstractValidator<CreateFormBranchCommand>
{
    public CreateFormBranchCommandValidator()
    {
        RuleFor(x => x.FormId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}

public class CreateFormBranchCommandHandler : IRequestHandler<CreateFormBranchCommand, FormBranch>
{
    private readonly FormXChange.Shared.Data.ApplicationDbContext _context;
    private readonly ILogger<CreateFormBranchCommandHandler> _logger;

    public CreateFormBranchCommandHandler(
        FormXChange.Shared.Data.ApplicationDbContext context,
        ILogger<CreateFormBranchCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<FormBranch> Handle(CreateFormBranchCommand request, CancellationToken cancellationToken)
    {
        // Check if form exists
        var form = await _context.Forms.FindAsync(new object[] { request.FormId }, cancellationToken);
        if (form == null)
            throw new InvalidOperationException($"Form with ID {request.FormId} not found");

        // Check if branch name already exists for this form
        var existingBranch = await _context.FormBranches
            .FirstOrDefaultAsync(b => b.FormId == request.FormId && b.Name == request.Name, cancellationToken);
            
        if (existingBranch != null)
            throw new InvalidOperationException($"Branch with name '{request.Name}' already exists for this form");

        var branch = new FormBranch
        {
            Id = Guid.NewGuid(),
            FormId = request.FormId,
            Name = request.Name,
            Description = request.Description,
            ParentBranchId = request.ParentBranchId,
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.FormBranches.Add(branch);
        await _context.SaveChangesAsync(cancellationToken);
        return branch;
    }
}

