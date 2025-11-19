using MediatR;
using FormXChange.Shared.Models;
using Microsoft.EntityFrameworkCore;
using FormXChange.Shared.Data;

namespace FormXChange.Forms.Queries;

public class GetFormQuery : IRequest<Form?>
{
    public Guid Id { get; set; }
}

public class GetFormQueryHandler : IRequestHandler<GetFormQuery, Form?>
{
    private readonly FormXChange.Shared.Data.ApplicationDbContext _context;
    private readonly ILogger<GetFormQueryHandler> _logger;

    public GetFormQueryHandler(
        FormXChange.Shared.Data.ApplicationDbContext context,
        ILogger<GetFormQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Form?> Handle(GetFormQuery request, CancellationToken cancellationToken)
    {
        return await _context.Forms
            .Include(f => f.Tenant)
            .Include(f => f.Creator)
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);
    }
}

