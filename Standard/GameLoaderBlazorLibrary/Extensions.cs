namespace GameLoaderBlazorLibrary;
public static class Extensions
{
    extension <V> (IServiceCollection services)
        where V : class, ILoaderVM
    {
        public void RegisterDefaultSinglePlayerProcesses()
        {
            services.AddTransient<ILoaderVM, V>();
            services.AddTransient<IStartUp, SinglePlayerStartUpClass>();
        }
    }
}