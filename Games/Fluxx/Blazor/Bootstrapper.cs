namespace Fluxx.Blazor;
public class Bootstrapper : MultiplayerBasicBootstrapper<FluxxShellViewModel>
{
    public Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode)
    {
    }

    //protected override Task RegisterTestsAsync()
    //{
    //    var container = GetDIContainer;
    //    TestData!.SaveOption = EnumTestSaveCategory.RestoreOnly;
    //    container.RegisterSingleton<ITestCardSetUp<FluxxCardInformation, FluxxPlayerItem>, TestCards>();
    //    return base.RegisterTestsAsync();
    //}
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        register.RegisterSingleton<IDeckCount, FluxxDeckCount>();
        //anything that needs to be registered will be here.
        return Task.CompletedTask;
    }

    //this part should not change
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<FluxxShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
    }
}