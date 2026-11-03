namespace SorryDicedGame.Blazor;
public partial class PlayerBoard
{
    [Parameter]
    [EditorRequired]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    [EditorRequired]
    public SorryDicedGamePlayerItem? Player { get; set; }
    [Parameter]
    [EditorRequired]
    public EventCallback<SorryDicedGamePlayerItem> OnHomeClicked { get; set; }
    [Parameter]
    [EditorRequired]
    public BasicList<BoardModel> BoardList { get; set; } = [];
    [Parameter]
    [EditorRequired]
    public EventCallback<WaitingModel> OnWaitingClicked { get; set; }
    private static string Columns => gg1.RepeatAuto(2);
    private string GetWaitingText => $"{Player!.NickName} Waiting";
    private static EnumColorChoice GetColor(int index) => EnumColorChoice.FromValue(index + 1);
    private string GetHomeText => $"{Player!.NickName} Home";
    private void PrivateHomeClicked()
    {
        OnHomeClicked.InvokeAsync(Player);
    }
    private void WaitingClicked(EnumColorChoice color)
    {
        WaitingModel waiting = new()
        {
            Player = Player!.Id,
            ColorUsed = color
        };
        OnWaitingClicked.InvokeAsync(waiting);
    }
    private int HowManyWaiting(int index)
    {
        int x = index + 1; //since it sends 0 based when its a grid.
        EnumColorChoice color = EnumColorChoice.FromValue(x);
        return BoardList.Count(x => x.PlayerOwned == Player!.Id && x.At == EnumBoardCategory.Waiting && x.Color == color);
    }
    private bool HasPieceAtHome(int index)
    {
        int count = BoardList.Count(x => x.PlayerOwned == Player!.Id && x.At == EnumBoardCategory.Home);
        if (count >= index)
        {
            return true;
        }
        return false;
    }
}