using MediatR;
using FormXChange.Shared.Models;
using Microsoft.EntityFrameworkCore;
using FormXChange.Shared.Data;

namespace FormXChange.Forms.Queries;

public class GetFormsQuery : IRequest<List<Form>>
{
    public Guid TenantId { get; set; }
    public string? Status { get; set; }
}

public class GetFormsQueryHandler : IRequestHandler<GetFormsQuery, List<Form>>
{
    private readonly FormXChange.Shared.Data.ApplicationDbContext _context;
    private readonly ILogger<GetFormsQueryHandler> _logger;

    public GetFormsQueryHandler(
        FormXChange.Shared.Data.ApplicationDbContext context,
        ILogger<GetFormsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Form>> Handle(GetFormsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Forms
            .Include(f => f.Tenant)
            .Include(f => f.Creator)
            .Where(f => f.TenantId == request.TenantId && f.IsActive);

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = query.Where(f => f.Status == request.Status);
        }

        return await query
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

