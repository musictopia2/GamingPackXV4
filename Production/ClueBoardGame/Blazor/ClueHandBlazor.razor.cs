namespace ClueBoardGame.Blazor;
public partial class ClueHandBlazor
{
    [Parameter]
    public HandObservable<CardInfo>? Hand { get; set; }
}