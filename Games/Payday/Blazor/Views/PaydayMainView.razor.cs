namespace Payday.Blazor.Views;
public partial class PaydayMainView
{
    [CascadingParameter]
    public int TargetHeight { get; set; }
    private string RealHeight => $"{TargetHeight}vh";
    private static string GetColumns => $"{bb1.RepeatMinimum(1)} {bb1.RepeatAuto(1)}";
    private static string RowData => "20vh 52vh 15vh";
    private readonly BasicList<ScoreColumnModel> _scores = new();
    private GameBoardGraphicsCP? _graphicsData;
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _graphicsData = aa1.Resolver!.Resolve<GameBoardGraphicsCP>();
        _labels.AddLabel("Main Turn", nameof(PaydayVMData.NormalTurn))
             .AddLabel("Other Turn", nameof(PaydayVMData.OtherLabel))
             .AddLabel("Progress", nameof(PaydayVMData.MonthLabel))
             .AddLabel("Status", nameof(PaydayVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Money", true, nameof(PaydayPlayerItem.MoneyHas), category: EnumScoreSpecialCategory.Currency)
            .AddColumn("Loans", true, nameof(PaydayPlayerItem.Loans), category: EnumScoreSpecialCategory.Currency);
        base.OnInitialized();
    }
    private string GetColor => _graphicsData!.GameContainer.SingleInfo!.Color.Color;
}