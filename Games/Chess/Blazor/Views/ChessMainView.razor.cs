namespace Chess.Blazor.Views;
public partial class ChessMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private GameBoardGraphicsCP? GameBoard { get; set; }
    private ChessGameContainer? GameContainer { get; set; }
    protected override void OnInitialized()
    {
        GameBoard = aa1.Resolver!.Resolve<GameBoardGraphicsCP>(); //hopefully this simple this time.
        GameContainer = aa1.Resolver.Resolve<ChessGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(ChessVMData.NormalTurn))
                .AddLabel("Instructions", nameof(ChessVMData.Instructions))
                .AddLabel("Status", nameof(ChessVMData.Status));
        base.OnInitialized();
    }
    private static int LongestSize => CheckersChessBaseBoard<EnumColorChoice, SpaceCP>.LongestSize;
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand TieCommand => DataContext!.TieCommand!;
    private ICustomCommand UndoCommand => DataContext!.UndoMovesCommand!;
    private int HighlightedIndex(SpaceCP space)
    {
        int tempIndex;
        if (GameContainer!.BasicData.MultiPlayer == false)
        {
            tempIndex = space.ReversedIndex;
        }
        else
        {
            tempIndex = space.MainIndex;
        }
        return tempIndex;
    }
    private string GetPreviousColor => GameContainer!.SaveRoot.PreviousMove.PlayerColor.ToWebColor();
    private static EnumPieceType GetCategory(CheckerChessPieceCP<EnumColorChoice> piece)
    {
        PieceCP output = (PieceCP)piece;
        return output.WhichPiece;
    }
}