namespace SorryDicedGame.Blazor;
public partial class CompletePlayerBoardComponent
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
}