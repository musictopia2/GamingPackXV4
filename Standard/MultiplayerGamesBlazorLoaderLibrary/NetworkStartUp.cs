namespace MultiplayerGamesBlazorLoaderLibrary;
public class NetworkStartUp : IRegisterNetworks
{
    void IRegisterNetworks.RegisterMultiplayerClasses(IGamePackageDIContainer container)
    {
        container.RegisterType<SignalRMessageService>();
        if (vv.DoUseHome == false)
        {
            container.RegisterType<SignalRAzureEndPoint>();
        }
        else
        {
            container.RegisterType<HomeSignalREndPoint>();
        }
        //if i do the tcp stuff, then here is where i have to figure the tcp part at.  that can come later though.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(container); //has to be here.  otherwise, the timing causes major issues.
    }
}