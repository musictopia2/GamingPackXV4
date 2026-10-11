namespace Fluxx.Blazor.Views;
public partial class ActionDiscardRulesView : SimpleActionView<ActionDiscardRulesViewModel>
{
    private ICustomCommand ViewCommand => DataContext!.ViewRuleCardCommand!;
    private ICustomCommand DiscardCommand => DataContext!.DiscardRulesCommand!;
    private readonly BasicList<LabelGridModel> _labels = [];
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Rules To Discard", nameof(ActionDiscardRulesViewModel.RulesToDiscard));
        base.OnInitialized();
    }
}