namespace DiceDominos.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode,
    string? globalTestOptionsUrl) : MultiplayerBasicBootstrapper<DiceDominosShellViewModel>(starts, mode, globalTestOptionsUrl)
{
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
	    IBasicDiceGamesData<SimpleDice>.NeedsRollIncrement = true;
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        register.RegisterType<DominosBasicShuffler<SimpleDominoInfo>>();
        register.RegisterSingleton<IDeckCount, SimpleDominoInfo>(); return Task.CompletedTask;
    }

    //this part should not change
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<DiceDominosShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
    }
}