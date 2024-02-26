namespace BasicGameFrameworkLibrary.Blazor.Bootstrappers;
public abstract class SinglePlayerBootstrapper<TViewModel>(IStartUp starts, EnumGamePackageMode mode) : BasicGameBootstrapper<TViewModel>(starts, mode)
    where TViewModel : IMainGPXShellVM
{
    protected override bool UseMultiplayerProcesses => false;
}