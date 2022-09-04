namespace GameLoaderBlazorLibrary;
public static class Extensions
{
    public static void RegisterDefaultSinglePlayerProcesses<V>(this IServiceCollection services)
        where V : class, ILoaderVM
    {
        services.AddTransient<ILoaderVM, V>();
        services.AddTransient<IStartUp, SinglePlayerStartUpClass>();
    }
}