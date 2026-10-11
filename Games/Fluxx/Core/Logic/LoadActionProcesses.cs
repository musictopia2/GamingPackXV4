namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class LoadActionProcesses(ActionContainer actionContainer,
    FluxxVMData model,
    BasicActionLogic basicActionLogic,
    FluxxGameContainer gameContainer
        ) : ILoadActionProcesses
{
    void ILoadActionProcesses.LoadDirections()
    {
        actionContainer.ActionCategory = EnumActionCategory.Directions;
        actionContainer.Direction1!.UnselectAll();
        actionContainer.IndexDirection = -1;
    }
    void ILoadActionProcesses.LoadDoAgainCards()
    {
        actionContainer.ActionCategory = EnumActionCategory.DoAgain;
        var firstList = model.Pile1!.DiscardList();
        DeckRegularDict<FluxxCardInformation> secondList = [];
        firstList.ForEach(x =>
        {
            if (x.Deck == 0)
            {
                throw new CustomBasicException("Deck cannot be 0 for load action again.  Rethink");
            }
            var card = FluxxDetailClass.GetNewCard(x.Deck);
            card.Populate(x.Deck);
            secondList.Add(card);
        });
        var tempList = secondList.Where(items => items.CanDoCardAgain()).Select(items => items.Text()).ToBasicList();
        if (tempList.Count == 0)
        {
            throw new CustomBasicException("Must have at least one card to do again.  Otherwise; can't do again");
        }
        actionContainer.IndexCard = -1;
        actionContainer.CardList1!.LoadTextList(tempList);
    }
    void ILoadActionProcesses.LoadDrawUse()
    {
        actionContainer.ActionCategory = EnumActionCategory.DrawUse;
        basicActionLogic.LoadTempCards();
        actionContainer.ButtonChooseCardVisible = true;
    }
    void ILoadActionProcesses.LoadEverybodyGetsOne()
    {
        actionContainer.ActionCategory = EnumActionCategory.Everybody1;
        basicActionLogic.LoadTempCards();
        if (gameContainer.EverybodyGetsOneList.Count == 0)
        {
            basicActionLogic.LoadPlayers(true);
            return;
        }
        var tempLists = gameContainer.PlayersLeftForEverybodyGetsOne();
        if (tempLists.Count < 2)
        {
            throw new CustomBasicException("Cannot load everybody gets one because less than 2 players");
        }
        actionContainer.PlayerIndexList.Clear();
        if (gameContainer.IncreaseAmount() == 1)
        {
            actionContainer.TempHand!.AutoSelect = EnumHandAutoType.SelectAsMany;
        }
        actionContainer.IndexPlayer = -1;
        BasicList<string> firstList = [];
        tempLists.ForEach(thisPlayer =>
        {
            actionContainer.PlayerIndexList.Add(thisPlayer.Id);
            firstList.Add($"{thisPlayer.NickName}, {thisPlayer.MainHandList.Count} cards in hand, {thisPlayer.Id}");
        });
        actionContainer.Player1!.LoadTextList(firstList);
    }
    void ILoadActionProcesses.LoadFirstRandom()
    {
        actionContainer.ActionCategory = EnumActionCategory.FirstRandom;
        actionContainer.OtherHand.Visible = true;
        var turnPlayer = gameContainer.PlayerList!.GetWhoPlayer();
        basicActionLogic.LoadOtherCards(turnPlayer);
        actionContainer.ActionFrameText = $"{gameContainer.SingleInfo!.NickName} will choose a card for {turnPlayer.NickName}";
    }
    void ILoadActionProcesses.LoadRules()
    {
        actionContainer.ActionCategory = EnumActionCategory.Rules;
        var tempList = gameContainer.SaveRoot!.RuleList.Where(items => items.Category != EnumRuleCategory.Basic).Select(items => items.Text()).ToBasicList();
        if (tempList.Count == 0)
        {
            throw new CustomBasicException("Cannot load the rules because there are no rules");
        }
        if (gameContainer.CurrentAction!.Deck == EnumActionMain.TrashANewRule)
        {
            actionContainer.Rule1!.SelectionMode = ListViewPicker.EnumSelectionMode.SingleItem;
            actionContainer.RulesToDiscard = 1;
        }
        else
        {
            actionContainer.Rule1!.SelectionMode = ListViewPicker.EnumSelectionMode.MultipleItems;
            actionContainer.RulesToDiscard = tempList.RulesThatCanBeDiscarded;
        }
        actionContainer.Rule1.LoadTextList(tempList);
    }
    void ILoadActionProcesses.LoadTradeHands()
    {
        actionContainer.ActionCategory = EnumActionCategory.TradeHands;
        basicActionLogic.LoadPlayers(false);
        actionContainer.ButtonChoosePlayerVisible = true;
    }
    void ILoadActionProcesses.LoadUseTake()
    {
        actionContainer.ActionCategory = EnumActionCategory.UseTake;
        basicActionLogic.LoadPlayers(false);
        if (actionContainer.IndexPlayer > -1)
        {
            actionContainer.OtherHand!.Visible = true;
            int index = actionContainer.GetPlayerIndex(actionContainer.IndexPlayer);
            var thisPlayer = gameContainer.PlayerList![index];
            actionContainer.ActionFrameText = "Other Cards";
            basicActionLogic.LoadOtherCards(thisPlayer);
        }
        else
        {
            actionContainer.ButtonChoosePlayerVisible = true;
        }
    }
}
