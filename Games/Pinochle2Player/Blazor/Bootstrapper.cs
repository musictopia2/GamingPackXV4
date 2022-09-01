namespace Pinochle2Player.Blazor;
public class Bootstrapper : MultiplayerBasicBootstrapper<Pinochle2PlayerShellViewModel>
{
    public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
    {
    }
    

    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        //change view model for area if not using 2 player.
        register.RegisterType<TwoPlayerTrickObservable<EnumSuitList, Pinochle2PlayerCardInformation, Pinochle2PlayerPlayerItem, Pinochle2PlayerSaveInfo>>();

        //if using misc deck, use this line
        register.RegisterSingleton<IDeckCount, CustomDeck>();
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
        register.RegisterType<Pinochle2PlayerShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
    }
}