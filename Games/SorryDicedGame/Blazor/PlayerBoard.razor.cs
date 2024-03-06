namespace SorryDicedGame.Blazor;
public partial class PlayerBoard
{
    [Parameter]
    [EditorRequired]
    public string ImageHeight { get; set; } = "";

    [Parameter]
    [EditorRequired]
    public SorryDicedGamePlayerItem? Player { get; set; }

    private static string Columns => gg1.RepeatAuto(2);
    //for now pretend like you have all 4 in waiting and at home.
    //no clicking for now.
    private string GetWaitingText => $"{Player!.NickName} Waiting";
    private static EnumColorChoice GetColor(int index) => EnumColorChoice.FromValue(index + 1);
    private string GetHomeText => $"{Player!.NickName} Home";

    //for now, has all 4 at home too.
    private static SizeF TargetSize => new(40, 40);
}