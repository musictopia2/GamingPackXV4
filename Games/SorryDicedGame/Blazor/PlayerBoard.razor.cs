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
    public EventCallback<WaitingModel> OnWaitingClicked { get; set; }
    private static string Columns => gg1.RepeatAuto(2);
    private string GetWaitingText => $"{Player!.NickName} Waiting";
    private static EnumColorChoice GetColor(int index) => EnumColorChoice.FromValue(index + 1);
    private string GetHomeText => $"{Player!.NickName} Home";
    private void PrivateHomeClicked()
    {
        //for now, can always click.  later rethink.
        OnHomeClicked.InvokeAsync(Player);
    }
    private void WaitingClicked(EnumColorChoice color)
    {
        WaitingModel waiting = new()
        {
            Player = Player!,
            ColorUsed = color
        };
        OnWaitingClicked.InvokeAsync(waiting);
    }
}