namespace BasicMultiplayerTrickCardGames.Blazor;
public class Bootstrapper : MultiplayerBasicBootstrapper<BasicMultiplayerTrickCardGamesShellViewModel>
{
    public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
    {
    }
    

    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        //change view model for area if not using 2 player.
        register.RegisterType<TwoPlayerTrickObservable<EnumSuitList, BasicMultiplayerTrickCardGamesCardInformation, BasicMultiplayerTrickCardGamesPlayerItem, BasicMultiplayerTrickCardGamesSaveInfo>>();

        //if using misc deck, use this line
        //register.RegisterSingleton<IDeckCount, BasicMultiplayerTrickCardGamesDeckCount>();
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        //if using misc deck, then remove this line of code.
        Core.DIFinishProcesses.SpecializedRegularCardHelpers.RegisterRegularDeckOfCardClasses(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        //anything that needs to be registered will be here.
        return Task.CompletedTask;
    }

    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<BasicMultiplayerTrickCardGamesShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
    }
}