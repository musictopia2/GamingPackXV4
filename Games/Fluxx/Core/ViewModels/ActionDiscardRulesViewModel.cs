namespace Fluxx.Core.ViewModels;
[InstanceGame]
[UseLabelGrid]
public partial class ActionDiscardRulesViewModel : BasicActionScreen
{
    [LabelColumn]
    public int RulesToDiscard { get; set; }
    public ActionDiscardRulesViewModel(FluxxGameContainer gameContainer,
        ActionContainer actionContainer,
        KeeperContainer keeperContainer,
        FluxxDelegates delegates,
        IFluxxEvent fluxxEvent,
        BasicActionLogic basicActionLogic,
        IEventAggregator aggregator) : base(gameContainer, actionContainer, keeperContainer, delegates, fluxxEvent, basicActionLogic, aggregator)
    {
        RulesToDiscard = actionContainer.RulesToDiscard;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanViewRuleCard()
    {
        if (ActionContainer.Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
        {
            return ActionContainer.IndexRule > -1;
        }
        return ActionContainer.TempRuleList!.Count == 1;
    }
    [Command(EnumCommandCategory.Plain)]
    public void ViewRuleCard()
    {
        if (ActionContainer.Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
        {
            ActionContainer.CurrentDetail!.ShowCard(GameContainer.SaveRoot!.RuleList[ActionContainer.IndexRule + 1]);
            return;
        }
        ActionContainer.CurrentDetail!.ShowCard(GameContainer.SaveRoot!.RuleList[ActionContainer.TempRuleList!.Single() + 1]);
    }
    public bool CanDiscardRules()
    {
        if (ActionContainer.Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
        {
            return ActionContainer.IndexRule > -1;
        }
        return ActionContainer.TempRuleList!.Count <= ActionContainer.RulesToDiscard;
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task DiscardRulesAsync()
    {
        await BasicActionLogic.ShowMainScreenAgainAsync();
        if (ActionContainer.Rule1!.SelectionMode == ListViewPicker.EnumSelectionMode.SingleItem)
        {
            await FluxxEvent.RuleTrashedAsync(ActionContainer.IndexRule);
            return;
        }
        await FluxxEvent.RulesSimplifiedAsync(ActionContainer.TempRuleList!);
    }
}
