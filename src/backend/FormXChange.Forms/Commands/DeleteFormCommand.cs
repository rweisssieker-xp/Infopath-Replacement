using MediatR;
using Microsoft.EntityFrameworkCore;
using FormXChange.Shared.Data;

namespace FormXChange.Forms.Commands;

public class DeleteFormCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class DeleteFormCommandHandler : IRequestHandler<DeleteFormCommand, bool>
{
    private readonly FormXChange.Shared.Data.ApplicationDbContext _context;
    private readonly ILogger<DeleteFormCommandHandler> _logger;

    public DeleteFormCommandHandler(
        FormXChange.Shared.Data.ApplicationDbContext context,
        ILogger<DeleteFormCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteFormCommand request, CancellationToken cancellationToken)
    {
        var form = await _context.Forms.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (form == null)
            return false;

        // Soft delete
        form.IsActive = false;
        form.Status = "Archived";
        form.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

