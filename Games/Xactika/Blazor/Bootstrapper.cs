namespace Xactika.Blazor;
public class Bootstrapper(IStartUp starts, EnumGamePackageMode mode) : MultiplayerBasicBootstrapper<XactikaShellViewModel>(starts, mode)
{
    protected override Task ConfigureAsync(IGamePackageRegister register)
    {
        register.RegisterType<SeveralPlayersTrickObservable<EnumShapes, XactikaCardInformation, XactikaPlayerItem, XactikaSaveInfo>>();
        register.RegisterSingleton<IDeckCount, XactikaDeckCount>();
        Core.DIFinishProcesses.GlobalDIAutoRegisterClass.RegisterNonSavedClasses(GetDIContainer);
        Core.DIFinishProcesses.SpecializedRegistrationHelpers.RegisterCommonMultplayerClasses(GetDIContainer);
        //if using misc deck, then remove this line of code.
        Core.DIFinishProcesses.AutoResetClass.RegisterAutoResets();
        //anything that needs to be registered will be here.
        return Task.CompletedTask;
    }

    protected override void FinishRegistrations(IGamePackageRegister register)
    {
        register.RegisterType<XactikaShellViewModel>(); //has to use interface part to make it work with source generators.
        Core.DIFinishProcesses.GlobalDIFinishClass.FinishDIRegistrations(GetDIContainer);
        Core.AutoResumeContexts.GlobalRegistrations.Register();
        rr1.Register();
    }
}