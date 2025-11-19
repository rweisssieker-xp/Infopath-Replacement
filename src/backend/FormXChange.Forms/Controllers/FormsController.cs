using Microsoft.AspNetCore.Mvc;
using MediatR;
using FormXChange.Forms.Queries;
using FormXChange.Forms.Commands;

namespace FormXChange.Forms.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FormsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FormsController> _logger;

    public FormsController(IMediator mediator, ILogger<FormsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetForms([FromQuery] Guid tenantId, [FromQuery] string? status = null)
    {
        var query = new GetFormsQuery { TenantId = tenantId, Status = status };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetForm(Guid id)
    {
        var query = new GetFormQuery { Id = id };
        var result = await _mediator.Send(query);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateForm([FromBody] CreateFormCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetForm), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateForm(Guid id, [FromBody] UpdateFormCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        
        if (result == null)
            return NotFound();
            
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteForm(Guid id)
    {
        var command = new DeleteFormCommand { Id = id };
        var result = await _mediator.Send(command);
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }

    [HttpPost("{id}/versions")]
    public async Task<IActionResult> CreateVersion(Guid id, [FromBody] CreateFormVersionCommand command)
    {
        command.FormId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/branches")]
    public async Task<IActionResult> CreateBranch(Guid id, [FromBody] CreateFormBranchCommand command)
    {
        command.FormId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}




