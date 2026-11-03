using System.ComponentModel.Design;

namespace Payday.Blazor.Views;
public partial class PaydayMainView
{
    [CascadingParameter]
    public int TargetHeight { get; set; }
    private static string GetColumns => $"{bb1.RepeatMinimum(1)} {bb1.RepeatAuto(1)}";
    private static string RowData => $"{22.HeightPixels}px {54.HeightPixels}px {18.HeightPixels}px";
    private string RealHeight => $"{TargetHeight.HeightPixels}px";
    private readonly BasicList<ScoreColumnModel> _scores = [];
    private GameBoardGraphicsCP? _graphicsData;
    private readonly BasicList<LabelGridModel> _labels = [];
    protected override void OnInitialized()
    {
        _graphicsData = Resolver!.Resolve<GameBoardGraphicsCP>();
        _labels.AddLabel("Main Turn", nameof(PaydayVMData.NormalTurn))
             .AddLabel("Other Turn", nameof(PaydayVMData.OtherLabel))
             .AddLabel("Progress", nameof(PaydayVMData.MonthLabel))
             .AddLabel("Status", nameof(PaydayVMData.Status));
        _scores.Clear();
        _scores.AddColumn("Money", true, nameof(PaydayPlayerItem.MoneyHas), numberOfDigits: 10, category: EnumScoreSpecialCategory.Currency)
            .AddColumn("Loans", true, nameof(PaydayPlayerItem.Loans), numberOfDigits: 10, category: EnumScoreSpecialCategory.Currency);
        base.OnInitialized();
    }
    private string GetColor => _graphicsData!.GameContainer.SingleInfo!.Color.Color;
    private static string InstructionsHeight => $"{15.HeightPixels}px";
    private static string DealListHeight => $"{45.HeightPixels}px";
}