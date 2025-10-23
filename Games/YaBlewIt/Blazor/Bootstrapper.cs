namespace YaBlewIt.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : MultiplayerBasicBootstrapper<YaBlewItShellViewModel>(starts, mode)
{
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        register.RegisterSingleton<IDeckCount, YaBlewItDeckCount>();
        //anything that needs to be registered will be here.
        return Task.CompletedTask;
    }

    //this part should not change
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<YaBlewItShellViewModel>(); //has to use interface part to make it work with source generators.
        register.RegisterType<StandardRollProcesses<EightSidedDice, YaBlewItPlayerItem>>();
        register.RegisterSingleton<IGenerateDice<int>, EightSidedDice>();
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
        rr1.Register();
    }
}