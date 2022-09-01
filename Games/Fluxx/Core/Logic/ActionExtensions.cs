namespace Fluxx.Core.Logic;
internal static class ActionExtensions
{
    public static void LoadSavedGame(this ActionContainer actionContainer, FluxxGameContainer gameContainer)
    {
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetSelf();
        actionContainer.YourCards!.HandList = gameContainer.SingleInfo.MainHandList;
        actionContainer.YourKeepers!.HandList = gameContainer.SingleInfo.KeeperList;
        gameContainer.SingleInfo = gameContainer.PlayerList.GetWhoPlayer();
        if (gameContainer.CurrentAction == null)
        {
            return;
        }
        if (gameContainer.CurrentAction.Deck == EnumActionMain.UseWhatYouTake && gameContainer.SaveRoot!.SavedActionData.SelectedIndex > -1)
        {
            BasicActionLogic logic = gameContainer.Resolver.Resolve<BasicActionLogic>();
            logic.SavedPlayers();
            actionContainer.IndexPlayer = gameContainer.SaveRoot.SavedActionData.SelectedIndex;
            if (actionContainer.IndexPlayer == -1)
            {
                throw new CustomBasicException("Rethink for reloading game");
            }
            actionContainer.Player1!.SelectSpecificItem(actionContainer.IndexPlayer);
        }
    }
    public static async Task PrepActionScreenAsync(this ActionContainer actionContainer, FluxxGameContainer gameContainer, FluxxDelegates delegates, ILoadActionProcesses loadAction)
    {
        actionContainer.Loads++;
        actionContainer.ActionFrameText = "Action Card Information";
        actionContainer.ButtonChoosePlayerVisible = false;
        actionContainer.OtherHand.Visible = false; //has to be proven true here too.
        actionContainer.ButtonChooseCardVisible = false;
        if (delegates.LoadProperActionScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading action screen.  Rethink");
        }
        actionContainer.CurrentDetail!.ResetCard();
        if (gameContainer.IsFirstPlayRandom() || gameContainer.CurrentAction == null)
        {
            actionContainer.ActionCategory = EnumActionCategory.FirstRandom;
            loadAction.LoadFirstRandom();
            await delegates.LoadProperActionScreenAsync(actionContainer);
            return;
        }
        switch (gameContainer.CurrentAction!.Deck)
        {
            case EnumActionMain.TrashANewRule:
            case EnumActionMain.LetsSimplify:
                loadAction.LoadRules();
                break;
            case EnumActionMain.LetsDoThatAgain:
                loadAction.LoadDoAgainCards();
                break;
            case EnumActionMain.RotateHands:
                loadAction.LoadDirections();
                break;
            case EnumActionMain.TradeHands:
                loadAction.LoadTradeHands();
                break;
            case EnumActionMain.UseWhatYouTake:
                loadAction.LoadUseTake();
                break;
            case EnumActionMain.EverybodyGets1:
                loadAction.LoadEverybodyGetsOne();
                break;
            case EnumActionMain.Draw3Play2OfThem:
            case EnumActionMain.Draw2AndUseEm:
                loadAction.LoadDrawUse();
                break;
            default:
                throw new CustomBasicException("Don't know what to do for status");
        }
        actionContainer.ActionDetail.ShowCard(gameContainer.CurrentAction!);
        await delegates.LoadProperActionScreenAsync(actionContainer);
    }
    public static BasicList<int> GetTempPlayerList(this ActionContainer actionContainer)
    {
        return Enumerable.Range(0, actionContainer.Player1!.Count()).ToBasicList();
    }
    public static BasicList<int> GetTempRuleList(this ActionContainer actionContainer)
    {
        return Enumerable.Range(0, actionContainer.Rule1!.Count()).ToBasicList();
    }
    public static BasicList<int> GetTempCardList(this ActionContainer actionContainer)
    {
        return Enumerable.Range(0, actionContainer.CardList1!.Count()).ToBasicList();
    }
    public static BasicList<FluxxPlayerItem> PlayersLeftForEverybodyGetsOne(this FluxxGameContainer gameContainer)
    {
        int extras = gameContainer.IncreaseAmount();
        int mosts = extras + 1;
        var tempLists = gameContainer.PlayerList!.ToBasicList();
        tempLists.RemoveAllOnly(thisPlayer =>
        {
            return gameContainer.EverybodyGetsOneList.Count(items => items.Player == thisPlayer.Id) == mosts;
        });
        return tempLists;
    }
    public static bool CanLoadEverybodyGetsOne(this FluxxGameContainer gameContainer)
    {
        var temps = gameContainer.PlayersLeftForEverybodyGetsOne();
        if (temps.Count == 0)
        {
            throw new CustomBasicException("Needs to have at least one player in order to figure out if everybody gets one");
        }
        return temps.Count > 1;
    }
}
