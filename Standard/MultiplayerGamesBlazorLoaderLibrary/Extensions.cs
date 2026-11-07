using BasicGameFrameworkLibrary.Core.SpecializedGameTypes.StockClasses;
using System.ComponentModel;

namespace MultiplayerGamesBlazorLoaderLibrary;
public static class Extensions
{
    extension <V> (IServiceCollection services)
        where V : class, ILoaderVM
    {
        public void RegisterDefaultMultiplayerProcesses()
        {
            services.AddTransient<ILoaderVM, V>();
            services.AddTransient<IStartUp, MainStartUp>();
            GlobalStartUp.KeysToSave.Clear(); //go ahead and clear just in case.
            GlobalClass.Multiplayer = true;//this is multiplayer.
            GlobalStartUp.KeysToSave.Add(GlobalDataModel.LocalStorageKey); //if i change it, will change everywhere.
            GlobalStartUp.KeysToSave.Add("latestgame"); //needs this so can always know the last game saved.
        }   
    }
    extension (IServiceCollection services)
    {
        public void RegisterSignalRGameDiscoveryServices()
        {
            services.AddTransient<HomeSignalREndPoint>();
            services.AddTransient<RemoteSignalREndPoint>();
            GlobalClass.HasServer = true;
            services.AddTransient<ISignalRInfo, SelectedSignalREndPoint>();

            services.AddTransient<
                IHostedGameDiscoveryClient,
                HostedGameDiscoveryClient>();
        }
    }
}