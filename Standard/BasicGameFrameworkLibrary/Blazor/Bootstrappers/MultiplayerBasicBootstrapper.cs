namespace BasicGameFrameworkLibrary.Blazor.Bootstrappers;
public abstract class MultiplayerBasicBootstrapper<TViewModel>(IStartUp starts, EnumGamePackageMode mode,
    string? globalTestOptionsUrl) : BasicGameBootstrapper<TViewModel>(starts, mode, globalTestOptionsUrl)
    where TViewModel : IMainGPXShellVM
{
    protected override void MiscRegisterFirst(IGamePackageRegister register)
    {
        register.RegisterType<NewRoundViewModel>(false);
        register.RegisterType<RestoreViewModel>(false);
    }
    protected override bool UseMultiplayerProcesses => true;
}