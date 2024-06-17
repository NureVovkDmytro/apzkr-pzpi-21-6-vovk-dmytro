using Discerniy.Domain.Entity.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Discerniy.Infrastructure.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection LoadCOSR(this IServiceCollection services)
        {
            var corsOptions = services.BuildServiceProvider().GetService<CorsConfig>() ?? throw new ArgumentNullException(nameof(CorsConfig));
            var logger = services.BuildServiceProvider().GetService<ILogger<CorsConfig>>() ?? throw new ArgumentNullException(nameof(ILogger<CorsConfig>));
            foreach (var cors in corsOptions.Servers)
            {
                logger.LogInformation($"CORS: {cors.Name}[{string.Join(";", cors.AllowedMethors)}] => {string.Join(";", cors.AllowedOrigins)}");
                services.AddCors(options =>
                {
                    options.AddPolicy(cors.Name, builder =>
                    {
                        builder.WithOrigins(cors.AllowedOrigins)
                                .WithMethods(cors.AllowedMethors);
                        if (cors.AllowedHeaders != null)
                        {
                            if (cors.AllowedHeaders.FirstOrDefault() == "*")
                            {
                                builder.WithHeaders(cors.AllowedHeaders);
                            }
                            else
                            {
                                builder.WithHeaders(cors.AllowedHeaders);
                            }
                        }
                        if (cors.AllowCredentials == true)
                        {
                            builder.AllowCredentials();
                        }
                    });

                });
            }
            return services;
        }

        public static IApplicationBuilder UseCORS(this IApplicationBuilder app)
        {
            var corsOptions = app.ApplicationServices.GetService<CorsConfig>() ?? throw new ArgumentNullException(nameof(CorsConfig));
            foreach (var cors in corsOptions.Servers)
            {
                app.UseCors(cors.Name);
            }
            return app;
        }
    }
}
