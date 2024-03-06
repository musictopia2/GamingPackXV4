namespace SorryDicedGame.Blazor;
public partial class CompletePlayerBoardComponent : IDisposable
{
    [Parameter]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    [EditorRequired]
    public ICustomCommand? HomeCommand { get; set; }
    [Parameter]
    [EditorRequired]
    public ICustomCommand? WaitingCommand { get; set; }
    [Parameter]
    [EditorRequired]
    public BasicList<SorryDicedGamePlayerItem> Players { get; set; } = [];
    [Parameter]
    [EditorRequired]
    public BasicList<BoardModel> BoardList { get; set; } = [];
    [CascadingParameter]
    public SorryDicedGameMainViewModel? DataContext { get; set; }
    private static string Columns => gg1.RepeatAuto(2);
    private async Task OnHomeClicked(SorryDicedGamePlayerItem player)
    {
        if (HomeCommand!.CanExecute(player) == false)
        {
            return;
        }
        await HomeCommand.ExecuteAsync(player);
    }
    private async Task OnWaitingClicked(WaitingModel wait)
    {
        if (WaitingCommand!.CanExecute(wait) == false)
        {
            return;
        }
        await WaitingCommand.ExecuteAsync(wait);
    }
    protected override void OnInitialized()
    {
        DataContext!.CommandContainer.AddAction(ShowChange, "sorryplayers");
        base.OnInitialized();
    }
    private bool _canRender;
    private void ShowChange()
    {
        _canRender = true;
        InvokeAsync(StateHasChanged);
    }
    protected override void OnAfterRender(bool firstRender)
    {
        _canRender = false;
        base.OnAfterRender(firstRender);
    }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
        DataContext!.CommandContainer.RemoveAction("sorryplayers");
    }
}