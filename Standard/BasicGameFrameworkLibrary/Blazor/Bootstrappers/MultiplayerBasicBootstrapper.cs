namespace BasicGameFrameworkLibrary.Blazor.Bootstrappers;
public abstract class MultiplayerBasicBootstrapper<TViewModel> : BasicGameBootstrapper<TViewModel>
    where TViewModel : IMainGPXShellVM
{
    protected MultiplayerBasicBootstrapper(IStartUp starts, EnumGamePackageMode mode) : base(starts, mode) { }
    protected override void MiscRegisterFirst(IGamePackageRegister register)
    {
        register.RegisterType<NewRoundViewModel>(false);
        register.RegisterType<RestoreViewModel>(false);
    }
    protected override bool UseMultiplayerProcesses => true;
}