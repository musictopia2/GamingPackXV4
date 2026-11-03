namespace Savannah.Blazor;
public partial class OpponentDiscardBlazor : IDisposable
{

    private CommandContainer _commandContainer = null!;

    [Parameter]
    public SavannahPlayerItem? Player { get; set; }
    [Parameter]
    public Func<bool>? IsEnabled { get; set; }
    [Parameter]
    public RenderFragment<RegularSimpleCard>? ChildContent { get; set; }
    private RegularSimpleCard? LastCard => Player!.DiscardList.Count == 0 ? null : Player.DiscardList.Last();

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _commandContainer = Resolver!.Resolve<CommandContainer>();
        _commandContainer.ExecutingChanged = Refresh;
    }
    private void Refresh()
    {
        InvokeAsync(StateHasChanged);
    }
    public void Dispose()
    {
        _commandContainer.ExecutingChanged = null;
        GC.SuppressFinalize(this);
    }
}