
namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Appelle la méthode CreateHostBuilder avec les arguments passés en paramètre, puis construit et exécute l'hôte
            CreateHostBuilder(args).Build().Run();
        }

        // Configure l'hôte pour qu'il utilise les valeurs par défaut et configure le service web pour utiliser la classe Startup
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
