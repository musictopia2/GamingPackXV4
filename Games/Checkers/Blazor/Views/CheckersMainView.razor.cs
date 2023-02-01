namespace Checkers.Blazor.Views;
public partial class CheckersMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private GameBoardGraphicsCP? GameBoard { get; set; }
    private CheckersGameContainer? GameContainer { get; set; }
    //private bool CanRenderSpace => GameContainer!.PlayerList!.First().Color != EnumColorChoice.None;
    protected override void OnInitialized()
    {
        GameBoard = aa1.Resolver!.Resolve<GameBoardGraphicsCP>();
        GameContainer = aa1.Resolver.Resolve<CheckersGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(CheckersVMData.NormalTurn))
                .AddLabel("Instructions", nameof(CheckersVMData.Instructions))
                .AddLabel("Status", nameof(CheckersVMData.Status));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand TieCommand => DataContext!.TieCommand!;
    private static string PieceColor(CheckerChessPieceCP<EnumColorChoice> piece)
    {
        if (piece.Highlighted)
        {
            return cs1.Yellow;
        }
        return piece.EnumValue.Color;
    }
    private static string PieceColor(EnumColorChoice color) => color.Color;
    private static EnumCheckerPieceCategory CheckerCategory(CheckerChessPieceCP<EnumColorChoice> piece)
    {
        CheckerPieceCP output = (CheckerPieceCP)piece;
        if (output.IsCrowned)
        {
            return EnumCheckerPieceCategory.CrownedPiece;
        }
        return EnumCheckerPieceCategory.SinglePiece;
    }
    private static int LongestSize => CheckersChessBaseBoard<EnumColorChoice, SpaceCP>.LongestSize;
    private EnumCheckerPieceCategory AnimationCategory
    {
        get
        {
            if (GameContainer!.CurrentCrowned)
            {
                return EnumCheckerPieceCategory.CrownedPiece;
            }
            return EnumCheckerPieceCategory.SinglePiece;
        }
    }
}