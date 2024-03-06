namespace SorryDicedGame.Blazor;
public partial class CompletePlayerBoardComponent
{
    [Parameter]
    public string ImageHeight { get; set; } = "";
    //[Parameter]
    //[EditorRequired]
    //public BasicGameCommand? BoardCommand { get; set; }
    [Parameter]
    [EditorRequired]
    public BasicList<SorryDicedGamePlayerItem> Players { get; set; } = [];


    private static string Columns => gg1.RepeatAuto(2);
}