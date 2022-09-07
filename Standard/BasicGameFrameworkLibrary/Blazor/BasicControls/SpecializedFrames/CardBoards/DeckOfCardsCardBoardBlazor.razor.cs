namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.CardBoards;
public partial class DeckOfCardsCardBoardBlazor<R>
    where R : class, IRegularCard, new()
{
    [Parameter]
    public GameBoardObservable<R>? DataContext { get; set; }
}