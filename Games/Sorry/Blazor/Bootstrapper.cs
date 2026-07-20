//i think this is the most common things i like to do
namespace Sorry.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : MultiplayerBasicBootstrapper<SorryShellViewModel>(starts, mode)
{
    //keep there for comments.  for now, if this goes to production, does not work.
    //protected override Task RegisterTestsAsync()
    //{
    //    ConfigureTestOptions(options =>
    //    {
    //        //here is where i do test options (if needed).


    //        //options.AdvancedTestOptions = true;
    //        //options.SaveOption = EnumTestSaveCategory.RestoreOnly;
    //        //options.DoubleCheck = true;
    //        // Do not reactivate advanced mode after Commit Test
    //        // changes the session to RestoreOnly.
    //        //if (options.SaveOption != EnumTestSaveCategory.RestoreOnly)
    //        //{
    //        //    options.AdvancedTestOptions = true;
    //        //}
    //    });
    //    return base.RegisterTestsAsync();
    //}
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        register!.RegisterType<DrawShuffleClass<CardInfo, SorryPlayerItem>>();
        register!.RegisterType<GenericCardShuffler<CardInfo>>();
        register!.RegisterSingleton<IDeckCount, DeckCount>(); return Task.CompletedTask;
    }

    //this part should not change
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<SorryShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
        rr1.Register();
    }
}