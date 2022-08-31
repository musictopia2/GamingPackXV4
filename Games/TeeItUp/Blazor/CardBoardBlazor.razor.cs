namespace TeeItUp.Blazor;
public partial class CardBoardBlazor
{
    [Parameter]
    public GameBoardObservable<TeeItUpCardInformation>? DataContext { get; set; }
}