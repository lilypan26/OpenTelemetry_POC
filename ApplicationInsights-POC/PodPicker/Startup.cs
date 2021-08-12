using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using OpenTelemetry.Exporter;
using OpenTelemetry.Exporter.Geneva;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace PodPicker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddOpenTelemetryTracing((builder) => builder
                 .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("AppInsights-Service3"))
                 .AddAspNetCoreInstrumentation()
                 .AddSource("Call")
                 .AddHttpClientInstrumentation()
                 .AddAzureMonitorTraceExporter(o =>
                 {
                     o.ConnectionString = "InstrumentationKey=33ee40f4-dd05-4075-8cac-d00b849c0a23;IngestionEndpoint=https://westus2-0.in.applicationinsights.azure.com/";
                 }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
