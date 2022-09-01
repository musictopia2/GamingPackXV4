namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class BasicActionLogic
{
    private readonly FluxxGameContainer _gameContainer;
    private readonly ActionContainer _actionContainer;
    private readonly FluxxDelegates _delegates;
    public BasicActionLogic(FluxxGameContainer gameContainer, ActionContainer actionContainer, FluxxDelegates delegates)
    {
        _gameContainer = gameContainer;
        _actionContainer = actionContainer;
        _delegates = delegates;
    }
    public async Task ShowMainScreenAgainAsync()
    {
        if (_delegates.LoadMainScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the main screen.  Rethink");
        }
        await _delegates.LoadMainScreenAsync.Invoke();
    }
    public void LoadPlayers(bool includingSelf)
    {
        int oldSelectedIndex = _actionContainer.IndexPlayer;
        SimpleLoadPlayers(includingSelf);
        if (_gameContainer.CurrentAction!.Deck == EnumActionMain.UseWhatYouTake)
        {
            if (_actionContainer.Loads == 1)
            {
                _actionContainer.IndexPlayer = _gameContainer.SaveRoot!.SavedActionData.SelectedIndex;
            }
            else
            {
                _actionContainer.IndexPlayer = oldSelectedIndex;
            }
            if (_actionContainer.IndexPlayer > -1)
            {
                _actionContainer.Player1!.SelectSpecificItem(_actionContainer.IndexPlayer);
            }
        }
    }
    internal void SavedPlayers()
    {
        SimpleLoadPlayers(true);
    }
    private void SimpleLoadPlayers(bool includingSelf)
    {
        var tempLists = _gameContainer.PlayerList!.ToBasicList();
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
        if (includingSelf == false)
        {
            tempLists.RemoveSpecificItem(_gameContainer.SingleInfo);
        }
        _actionContainer.PlayerIndexList.Clear();
        BasicList<string> firstList = new();
        _actionContainer.IndexPlayer = -1;
        tempLists.ForEach(thisPlayer =>
        {
            _actionContainer.PlayerIndexList.Add(thisPlayer.Id);
            firstList.Add($"{thisPlayer.NickName}, {thisPlayer.MainHandList.Count} hand, # {_actionContainer.PlayerIndexList.Last()}");
        });
        _actionContainer.Player1!.LoadTextList(firstList);
    }
    public int GetPlayerIndex(int selectedIndex)
    {
        return _actionContainer.PlayerIndexList[selectedIndex];
    }
    public void LoadOtherCards(FluxxPlayerItem thisPlayer)
    {
        var thisList = thisPlayer.MainHandList.Select(items => items.Deck).ToBasicList();
        DeckRegularDict<FluxxCardInformation> newList = new();
        thisList.ForEach(thisItem =>
        {
            var thisCard = FluxxDetailClass.GetNewCard(thisItem);
            thisCard.Populate(thisItem);
            thisCard.IsUnknown = true;
            newList.Add(thisCard);
        });
        newList.ShuffleList();
        _actionContainer.OtherHand!.HandList.ReplaceRange(newList);
        _actionContainer.ButtonChooseCardVisible = true;
        _actionContainer.OtherHand.Visible = true;
    }
    public void LoadTempCards()
    {
        if (_gameContainer.TempActionHandList.Count == 0)
        {
            throw new CustomBasicException("There are no cards left for the temp cards");
        }
        _actionContainer.TempHand.AutoSelect = EnumHandAutoType.SelectOneOnly;
        var firstList = _gameContainer.TempActionHandList.GetNewObjectListFromDeckList(_gameContainer.DeckList!);
        firstList.ForEach(thisCard => thisCard.IsUnknown = false);
        _actionContainer.TempHand.HandList.ReplaceRange(firstList);
    }
}