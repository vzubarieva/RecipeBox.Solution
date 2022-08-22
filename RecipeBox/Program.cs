using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace RecipeBox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .ConfigureLogging(
                    logBuilder =>
                    {
                        logBuilder.ClearProviders(); // removes all providers from LoggerFactory
                        logBuilder.AddConsole();
                    }
                )
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
