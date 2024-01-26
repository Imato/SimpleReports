using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using SimpleReports.Components;
using SimpleReports.Services;
using System.Reflection;

namespace SimpleReports
{
    public static class ReportsBuilder
    {
        public static WebApplicationBuilder AddReports(this WebApplicationBuilder builder)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            builder.Configuration.AddJsonFile($"{path}{Path.DirectorySeparatorChar}reports.json", false, true);
            builder.Configuration.AddJsonFile($"{path}{Path.DirectorySeparatorChar}reports.{builder.Environment.EnvironmentName}.json", true, true);

            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddSingleton<ISqlService, SqlService>();
            builder.Services.AddSingleton<IReportService, ReportService>();

            return builder;
        }

        public static WebApplication UseReports(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                // app.UseExceptionHandler("/Error", createScopeForErrors: true);
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(
                        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
                        "wwwroot"))
            });
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            return app;
        }
    }
}