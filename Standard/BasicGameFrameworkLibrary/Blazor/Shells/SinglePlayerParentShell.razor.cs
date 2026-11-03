namespace BasicGameFrameworkLibrary.Blazor.Shells;
public partial class SinglePlayerParentShell
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    public BasicData BasicData { get; set; }
    public IGameInfo GameInfo { get; set; }
    public SinglePlayerParentShell()
    {
        BasicData = Resolver!.Resolve<BasicData>();
        GameInfo = Resolver!.Resolve<IGameInfo>();
    }
    protected override void OnInitialized()
    {
        IStartUp starts = Resolver!.Resolve<IStartUp>();
        starts.StartVariables(BasicData);
        BasicData.ChangeState = ShowChange;
        base.OnInitialized();
    }
    private async void ShowChange()
    {
        await InvokeAsync(StateHasChanged);
    }
}