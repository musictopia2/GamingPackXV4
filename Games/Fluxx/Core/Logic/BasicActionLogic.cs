namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class BasicActionLogic(FluxxGameContainer gameContainer, ActionContainer actionContainer, FluxxDelegates delegates)
{
    public async Task ShowMainScreenAgainAsync()
    {
        if (delegates.LoadMainScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the main screen.  Rethink");
        }
        await delegates.LoadMainScreenAsync.Invoke();
    }
    public void LoadPlayers(bool includingSelf)
    {
        int oldSelectedIndex = actionContainer.IndexPlayer;
        SimpleLoadPlayers(includingSelf);
        if (gameContainer.CurrentAction!.Deck == EnumActionMain.UseWhatYouTake)
        {
            if (actionContainer.Loads == 1)
            {
                actionContainer.IndexPlayer = gameContainer.SaveRoot!.SavedActionData.SelectedIndex;
            }
            else
            {
                actionContainer.IndexPlayer = oldSelectedIndex;
            }
            if (actionContainer.IndexPlayer > -1)
            {
                actionContainer.Player1!.SelectSpecificItem(actionContainer.IndexPlayer);
            }
        }
    }
    internal void SavedPlayers()
    {
        SimpleLoadPlayers(true);
    }
    private void SimpleLoadPlayers(bool includingSelf)
    {
        var tempLists = gameContainer.PlayerList!.ToBasicList();
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        if (includingSelf == false)
        {
            tempLists.RemoveSpecificItem(gameContainer.SingleInfo);
        }
        actionContainer.PlayerIndexList.Clear();
        BasicList<string> firstList = [];
        actionContainer.IndexPlayer = -1;
        tempLists.ForEach(thisPlayer =>
        {
            actionContainer.PlayerIndexList.Add(thisPlayer.Id);
            firstList.Add($"{thisPlayer.NickName}, {thisPlayer.MainHandList.Count} hand, # {actionContainer.PlayerIndexList.Last()}");
        });
        actionContainer.Player1!.LoadTextList(firstList);
    }
    public int GetPlayerIndex(int selectedIndex)
    {
        return actionContainer.PlayerIndexList[selectedIndex];
    }
    public void LoadOtherCards(FluxxPlayerItem thisPlayer)
    {
        var thisList = thisPlayer.MainHandList.Select(items => items.Deck).ToBasicList();
        DeckRegularDict<FluxxCardInformation> newList = [];
        thisList.ForEach(thisItem =>
        {
            var thisCard = FluxxDetailClass.GetNewCard(thisItem);
            thisCard.Populate(thisItem);
            thisCard.IsUnknown = true;
            newList.Add(thisCard);
        });
        newList.ShuffleList();
        actionContainer.OtherHand!.HandList.ReplaceRange(newList);
        actionContainer.ButtonChooseCardVisible = true;
        actionContainer.OtherHand.Visible = true;
    }
    public void LoadTempCards()
    {
        if (gameContainer.TempActionHandList.Count == 0)
        {
            throw new CustomBasicException("There are no cards left for the temp cards");
        }
        actionContainer.TempHand.AutoSelect = EnumHandAutoType.SelectOneOnly;
        var firstList = gameContainer.TempActionHandList.GetNewObjectListFromDeckList(gameContainer.DeckList!);
        firstList.ForEach(thisCard => thisCard.IsUnknown = false);
        actionContainer.TempHand.HandList.ReplaceRange(firstList);
    }
}