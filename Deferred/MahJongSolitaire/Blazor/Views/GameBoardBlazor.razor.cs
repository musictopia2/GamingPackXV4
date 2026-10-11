namespace MahJongSolitaire.Blazor.Views;
internal struct TileGame
{
    public int Deck { get; set; }
    public int GameNumber { get; set; }
}
public partial class GameBoardBlazor
{
    [Parameter]
    public BasicList<BoardInfo> BoardList { get; set; } = new();
    private static string GetGameKey => $"MahjongGame{MahJongSolitaireMainViewModel.GameDrawing}";
}