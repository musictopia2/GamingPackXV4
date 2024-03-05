namespace SorryDicedGame.Blazor;
public partial class CompleteStartComponent
{
    [Parameter]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    public PlayerCollection<SorryDicedGamePlayerItem> Players { get; set; } = [];
    private string Columns => gg1.RepeatAuto(2);
}