using AnyRivals.WebAPI.ExceptionHandlers;
using Serilog;

namespace AnyRivals.Web.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddExceptionHandling(this IServiceCollection services)
    {
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<RequestExceptionHandler>();
        services.AddExceptionHandler<InternalExceptionHandler>();
    }

    public static void AddSerilogLogging(this IServiceCollection services)
    {
        services.AddSerilog(o =>
        {
            o.MinimumLevel.Information();
            o.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information);
            o.WriteTo.Console();
            o.WriteTo.Debug();
            o.Enrich.FromLogContext();
        });
    }
}
