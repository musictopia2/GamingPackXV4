namespace Fluxx.Blazor.Views;
public partial class ActionDiscardRulesView : SimpleActionView
{
    [CascadingParameter]
    public ActionDiscardRulesViewModel? DataContext { get; set; }
    private ICustomCommand ViewCommand => DataContext!.ViewRuleCardCommand!;
    private ICustomCommand DiscardCommand => DataContext!.DiscardRulesCommand!;
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Rules To Discard", nameof(ActionDiscardRulesViewModel.RulesToDiscard));
        base.OnInitialized();
    }
}