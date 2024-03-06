//i think this is the most common things i like to do
namespace SorryDicedGame.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : MultiplayerBasicBootstrapper<SorryDicedGameShellViewModel>(starts, mode)
{
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        register.RegisterSingleton<IGenerateDice<int>, SorryDiceModel>();
        return Task.CompletedTask;
    }
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<SorryDicedGameShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
    }
}