namespace BasicGameFrameworkLibrary.Blazor.Bootstrappers;
public abstract class SinglePlayerBootstrapper<TViewModel>(IStartUp starts, EnumGamePackageMode mode,
    string? globalTestOptionsUrl) : BasicGameBootstrapper<TViewModel>(starts, mode, globalTestOptionsUrl)
    where TViewModel : IMainGPXShellVM
{
    protected override bool UseMultiplayerProcesses => false;
}