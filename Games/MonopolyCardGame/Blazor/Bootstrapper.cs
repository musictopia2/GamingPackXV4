namespace MonopolyCardGame.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : MultiplayerBasicBootstrapper<MonopolyCardGameShellViewModel>(starts, mode)
{
    //protected override Task RegisterTestsAsync()
    //{
        //TestData!.WhoStarts = 2;
        //TestData.PlayCategory = EnumTestPlayCategory.NoShuffle;

        //TestData!.SaveOption = EnumTestSaveCategory.RestoreOnly; //so i can test going out with wilds everytime.
        //var container = GetDIContainer;
        //container.RegisterSingleton<ITestCardSetUp<MonopolyCardGameCardInformation, MonopolyCardGamePlayerItem>, TestCards>();
        //return base.RegisterTestsAsync();
    //}
    //protected override Task RegisterTestsAsync()
    //{
    //    TestData!.SaveOption = EnumTestSaveCategory.RestoreOnly;
    //    return base.RegisterTestsAsync();
    //}
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        register.RegisterSingleton<IDeckCount, MonopolyCardGameDeckCount>();
        //anything that needs to be registered will be here.
        return Task.CompletedTask;
    }

    //this part should not change
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<MonopolyCardGameShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
    }
}