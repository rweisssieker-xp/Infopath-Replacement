using Microsoft.EntityFrameworkCore;
using FormXChange.Shared.Data;
using FormXChange.AI.Services;
using FormXChange.AI.Workflows.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AI Services
builder.Services.AddScoped<ILLMService, LLMService>();
builder.Services.AddScoped<IConversationalEngine, ConversationalEngine>();
builder.Services.AddScoped<IConversationalWorkflowService, ConversationalWorkflowService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();




