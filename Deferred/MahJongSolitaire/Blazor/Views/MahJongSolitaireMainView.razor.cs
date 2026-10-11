namespace MahJongSolitaire.Blazor.Views;
public partial class MahJongSolitaireMainView
{
    private BasicList<BoardInfo>? BoardList { get; set; }
    protected override void OnParametersSet()
    {
        BoardList = DataContext!.MainGame.GameBoard1.GetPriorityBoards();
        base.OnParametersSet();
    }
    //try to not have undomove since it had too many problems for now.
    //private ICustomCommand UndoCommand => DataContext!.UndoMoveCommand!;
}