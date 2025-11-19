using MediatR;
using FormXChange.Shared.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using FormXChange.Shared.Data;

namespace FormXChange.Forms.Commands;

public class UpdateFormCommand : IRequest<Form?>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Schema { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? Status { get; set; }
}

public class UpdateFormCommandValidator : AbstractValidator<UpdateFormCommand>
{
    public UpdateFormCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        When(x => !string.IsNullOrEmpty(x.Name), () =>
        {
            RuleFor(x => x.Name).MaximumLength(200);
        });
    }
}

public class UpdateFormCommandHandler : IRequestHandler<UpdateFormCommand, Form?>
{
    private readonly FormXChange.Shared.Data.ApplicationDbContext _context;
    private readonly ILogger<UpdateFormCommandHandler> _logger;

    public UpdateFormCommandHandler(
        FormXChange.Shared.Data.ApplicationDbContext context,
        ILogger<UpdateFormCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Form?> Handle(UpdateFormCommand request, CancellationToken cancellationToken)
    {
        var form = await _context.Forms.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (form == null)
            return null;

        if (!string.IsNullOrEmpty(request.Name))
            form.Name = request.Name;
            
        if (request.Description != null)
            form.Description = request.Description;
            
        if (!string.IsNullOrEmpty(request.Schema))
        {
            form.Schema = request.Schema;
            form.Version++;
            
            // Create new version
            var version = new FormVersion
            {
                Id = Guid.NewGuid(),
                FormId = form.Id,
                VersionNumber = form.Version,
                Schema = form.Schema,
                CreatedBy = request.UpdatedBy ?? form.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                BranchName = form.BranchName
            };
            
            _context.FormVersions.Add(version);
        }
        
        if (request.UpdatedBy.HasValue)
            form.UpdatedBy = request.UpdatedBy.Value;
            
        if (!string.IsNullOrEmpty(request.Status))
            form.Status = request.Status;
            
        form.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return form;
    }
}

