using AnyRivals.Web.Extensions;
using AnyRivals.Web.Middlewares;
using AnyRivals.Application.Common.Extensions;
using AnyRivals.Infrastructure;
using AnyRivals.Infrastructure.Data.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilogLogging();
builder.Services.AddCors();

builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddExceptionHandling();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.ConfigureCors(false);
app.UseSerilogRequestLogging();
await app.UseApplicationDbContext();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseAuthorization();

app.MapSignalRHubs();
app.MapApiEndpoints();

app.Run();