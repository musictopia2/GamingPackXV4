namespace Backgammon.Blazor.Views;
public partial class BackgammonMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private GameBoardGraphicsCP? _graphicsData;
    private static string GetColumns => $"{bb.RepeatMinimum(1)} {bb.RepeatAuto(1)}";
    protected override void OnInitialized()
    {
        _graphicsData = aa.Resolver!.Resolve<GameBoardGraphicsCP>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(BackgammonVMData.NormalTurn))
           .AddLabel("Game Status", nameof(BackgammonVMData.Status))
           .AddLabel("Moves Made", nameof(BackgammonVMData.MovesMade))
           .AddLabel("Last Status", nameof(BackgammonVMData.LastStatus))
           .AddLabel("Instructions", nameof(BackgammonVMData.Instructions));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand UndoCommand => DataContext!.UndoMoveCommand!;
}