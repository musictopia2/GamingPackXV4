//i think this is the most common things i like to do
namespace Candyland.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode,
    string? globalTestOptionsUrl) : MultiplayerBasicBootstrapper<CandylandShellViewModel>(starts, mode, globalTestOptionsUrl)
{
    //protected override Task RegisterTestsAsync()
    //{
    //    ConfigureTestOptions(test =>
    //    {
    //        test.WhoStarts = 1;
    //    });
    //    return base.RegisterTestsAsync();
    //}
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        register.RegisterType<DrawShuffleClass<CandylandCardData, CandylandPlayerItem>>();
        register.RegisterType<GenericCardShuffler<CandylandCardData>>(); //this is iffy too.
        register.RegisterSingleton<IDeckCount, CandylandCount>();
        return Task.CompletedTask;
    }

    //this part should not change
    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<CandylandShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
        rr1.Register();
    }
}