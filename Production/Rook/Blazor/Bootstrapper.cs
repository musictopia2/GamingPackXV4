namespace Rook.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode,
    string? globalTestOptionsUrl) : MultiplayerBasicBootstrapper<RookShellViewModel>(starts, mode, globalTestOptionsUrl)
{
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        register.RegisterSingleton<IDeckCount, RookDeckCount>();
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        //if using misc deck, then remove this line of code.
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        //anything that needs to be registered will be here.
        return Task.CompletedTask;
    }

    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<RookShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
        rr1.Register();
    }
}