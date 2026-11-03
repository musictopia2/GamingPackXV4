namespace DealCardGame.Blazor;
public partial class OriginalActionComponent
{
    [CascadingParameter]
    public int ImageHeight { get; set; }
    [Parameter]
    [EditorRequired]
    public DealCardGameSaveInfo? SaveRoot { get; set; }
    [Parameter]
    [EditorRequired]
    public DealCardGameCardInformation? ActionCard { get; set; }
    [Parameter]
    [EditorRequired]
    public DealCardGamePlayerItem? SelfPlayer { get; set; }
    [Parameter]
    [EditorRequired]
    public DealCardGamePlayerItem? OpponentPlayer { get; set; }
    [Parameter]
    [EditorRequired]
    public CommandContainer? Command { get; set; }
    [Parameter]
    [EditorRequired]
    public BasicList<DealCardGamePlayerItem> Players { get; set; } = [];
}