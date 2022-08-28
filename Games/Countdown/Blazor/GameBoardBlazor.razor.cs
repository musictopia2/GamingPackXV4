namespace Countdown.Blazor;
public partial class GameBoardBlazor
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public CountdownGameContainer? GameContainer { get; set; }

    private BasicList<CountdownPlayerItem> _players = new();
    protected override void OnInitialized()
    {
        _players = GameContainer!.GetPlayerList();
        base.OnInitialized();
    }
}