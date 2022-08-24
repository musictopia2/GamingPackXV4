namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.CardBoards;
public partial class DominosGridBoardBlazor<D>
    where D : class, IDominoInfo, new()
{
    [Parameter]
    public GameBoardObservable<D>? DataContext { get; set; } //only iffy part is new game.
}