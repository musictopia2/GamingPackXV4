namespace MultiplayerGamesBlazorLoaderLibrary;
public static class Extensions
{
    public static void RegisterDefaultMultiplayerProcesses<V>(this IServiceCollection services)
            where V : class, ILoaderVM
    {
        services.AddTransient<ILoaderVM, V>();
        services.AddTransient<IStartUp, MainStartUp>();
        GlobalStartUp.KeysToSave.Clear(); //go ahead and clear just in case.
        GlobalClass.Multiplayer = true;//this is multiplayer.
        GlobalStartUp.KeysToSave.Add(GlobalDataModel.LocalStorageKey); //if i change it, will change everywhere.
    }
}