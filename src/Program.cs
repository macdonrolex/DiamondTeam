using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ContosoCrafts.WebSite
{
    /// <summary>
    /// Defines the application's entry point and configures its web host.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Builds and runs the web application.
        /// </summary>
        /// <param name="args">Command-line arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Creates the default host and connects ASP.NET Core to the application's startup class.
        /// </summary>
        /// <param name="args">Command-line arguments used to configure the host.</param>
        /// <returns>The configured web host builder.</returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
