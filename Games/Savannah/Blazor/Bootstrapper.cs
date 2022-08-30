namespace Savannah.Blazor;
public class Bootstrapper : MultiplayerBasicBootstrapper<SavannahShellViewModel>
{
    public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
    {
    }

    
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        register.RegisterSingleton<IDeckCount, CustomDeck>();
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterStandardDice(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegularCardHelpers.RegisterRegularDeckOfCardClasses(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        //anything that needs to be registered will be here.
        return Task.CompletedTask;
    }

    //this part should not change
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<SavannahShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
    }
}