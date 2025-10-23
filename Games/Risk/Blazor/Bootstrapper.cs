//i think this is the most common things i like to do
namespace Risk.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : MultiplayerBasicBootstrapper<RiskShellViewModel>(starts, mode)
{
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterStandardDice(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        register.RegisterType<DrawShuffleClass<RiskCardInfo, RiskPlayerItem>>(); //hopefully this does not have to be replaced.
        register.RegisterType<GenericCardShuffler<RiskCardInfo>>(); //this is iffy too.
        register.RegisterSingleton<IDeckCount, RiskCardCount>();
        return Task.CompletedTask;
    }

    //this part should not change
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<RiskShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
        rr1.Register();
    }
}