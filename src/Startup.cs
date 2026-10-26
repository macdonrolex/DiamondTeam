using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using ContosoCrafts.WebSite.Services;


namespace ContosoCrafts.WebSite
{
    /// <summary>
    /// Registers application services and configures the HTTP request pipeline.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Creates the startup configuration using the application's configuration settings.
        /// </summary>
        /// <param name="configuration">Configuration values available to the application.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration values supplied to the application at startup.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Registers the services that pages, controllers, and other application components use.
        /// </summary>
        /// <param name="services">The application's service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Register the web frameworks and the product data service used by the pages.
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddServerSideBlazor();
            services.AddHttpClient();
            services.AddControllers();
            services.AddTransient<JsonFileProductService>();
        }

        /// <summary>
        /// Sets up middleware and routes used to handle incoming HTTP requests.
        /// </summary>
        /// <param name="app">Builder used to configure the request pipeline.</param>
        /// <param name="env">The current hosting environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Show detailed errors in development and use the error page and HSTS elsewhere.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Routing must run before authorization and endpoint mapping.
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapBlazorHub();

                // endpoints.MapGet("/products", (context) => 
                // {
                //     var products = app.ApplicationServices.GetService<JsonFileProductService>().GetProducts();
                //     var json = JsonSerializer.Serialize<IEnumerable<Product>>(products);
                //     return context.Response.WriteAsync(json);
                // });
            });
        }
    }
}
