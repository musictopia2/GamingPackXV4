namespace MultiplayerGamesBlazorLoaderLibrary;
public class NetworkStartUp : IRegisterNetworks
{
    void IRegisterNetworks.RegisterMultiplayerClasses(
        IGamePackageDIContainer container)
    {
        container.RegisterType<SignalRMessageService>();

        if (HomeNetworkConfiguration.DoUseHome == false)
        {
            container.RegisterType<RemoteSignalREndPoint>();
        }
        else
        {
            container.RegisterType<HomeSignalREndPoint>();
        }
        Core.DIFinishProcesses.GlobalDIFinishClass
            .FinishDIRegistrations(container);
    }
}