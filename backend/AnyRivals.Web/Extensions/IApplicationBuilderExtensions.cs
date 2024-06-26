﻿namespace AnyRivals.Web.Extensions;

public static class IApplicationBuilderExtensions
{
    public static void ConfigureCors(this IApplicationBuilder app, bool isInDevelopment)
    {
        if (isInDevelopment)
        {
            app.UseCors(builder => builder
               .AllowAnyMethod()
               .AllowAnyHeader()
               .WithExposedHeaders("Token-Expired")
               .AllowAnyOrigin());
        }
        else
        {
            var origin = app.ApplicationServices.GetRequiredService<IConfiguration>()["AllowedOrigin"]
                ?? throw new ArgumentNullException("AllowedOrigin");

            var test = app.ApplicationServices.GetRequiredService<ILogger<IConfiguration>>();
            test.LogInformation("Origin: {Origin}", origin);

            app.UseCors(builder => builder
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("Token-Expired")
                .AllowCredentials()
                .WithOrigins(origin, "http://localhost", "https://localhost"));
        }
    }
}
