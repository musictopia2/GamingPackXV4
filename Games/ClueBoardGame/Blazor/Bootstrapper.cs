namespace ClueBoardGame.Blazor;
public class Bootstrapper : MultiplayerBasicBootstrapper<ClueBoardGameShellViewModel>
{
    public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
    {
    }
    //protected override Task RegisterTestsAsync()
    //{
    //    TestData!.SaveOption = EnumTestSaveCategory.RestoreOnly;
    //    return base.RegisterTestsAsync();
    //}
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterStandardDice(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        return Task.CompletedTask;
    }
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<ClueBoardGameShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
    }
}