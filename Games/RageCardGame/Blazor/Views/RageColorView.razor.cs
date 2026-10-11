namespace RageCardGame.Blazor.Views;
public partial class RageColorView
{
    private BasicList<ScoreColumnModel> _scores = [];
    private readonly BasicList<LabelGridModel> _labels = [];
    private RageCardGameVMData? _vmData;
    protected override void OnParametersSet()
    {
        _vmData = Resolver!.Resolve<RageCardGameVMData>();
        _scores = ScoreModule.GetScores();
        _labels.Clear();
        _labels.AddLabel("Trump", nameof(RageCardGameVMData.TrumpSuit))
            .AddLabel("Lead", nameof(RageCardGameVMData.Lead));
        base.OnParametersSet();
    }
    private static string GetColor(BasicPickerData<EnumColor> piece) => piece.EnumValue.Color;
}