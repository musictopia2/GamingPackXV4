namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class EverybodyOneProcesses : IEverybodyOneProcesses
{
    private readonly FluxxGameContainer _gameContainer;
    private readonly ActionContainer _actionContainer;
    private readonly IAnalyzeProcesses _processes;
    public EverybodyOneProcesses(FluxxGameContainer gameContainer, ActionContainer actionContainer, IAnalyzeProcesses processes)
    {
        _gameContainer = gameContainer;
        _actionContainer = actionContainer;
        _processes = processes;
    }
    async Task IEverybodyOneProcesses.EverybodyGetsOneAsync(BasicList<int> thisList, int selectedIndex)
    {
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
        {
            BasicList<int> sendList = new();
            sendList.AddRange(thisList);
            sendList.Add(selectedIndex);
            await _gameContainer.Network!.SendAllAsync("everybodygetsone", sendList);
        }
        int player = _actionContainer.GetPlayerIndex(selectedIndex);
        var newList = thisList.Select(items => new PreviousCard { Deck = items, Player = player }).ToBasicList();
        _gameContainer!.EverybodyGetsOneList.AddRange(newList);
        int oldCount = _gameContainer.TempActionHandList.Count;
        _gameContainer.TempActionHandList.RemoveGivenList(thisList);
        _actionContainer.SetUpGoals();
        if (_gameContainer.TempActionHandList.Count == oldCount)
        {
            throw new CustomBasicException("Did not remove from temphand");
        }
        if (_gameContainer.CanLoadEverybodyGetsOne() == false)
        {
            await LastCardsForEverybodyGetsOneAsync();
            return;
        }
        await _processes.AnalyzeQueAsync();
    }
    private async Task LastCardsForEverybodyGetsOneAsync()
    {
        int extras = _gameContainer!.IncreaseAmount();
        int maxs = extras + 1;
        foreach (var thisPlayer in _gameContainer.PlayerList!)
        {
            if (_gameContainer.EverybodyGetsOneList.Count(items => items.Player == thisPlayer.Id) < maxs)
            {
                var tempList = _gameContainer.TempActionHandList.Select(items => new PreviousCard { Deck = items, Player = thisPlayer.Id }).ToBasicList();
                _gameContainer.EverybodyGetsOneList.AddRange(tempList);
                _gameContainer.TempActionHandList.Clear();
                await FinalEverybodyGetsOneAsync();
                return;
            }
        }
        throw new CustomBasicException("Wrong");
    }
    private async Task FinalEverybodyGetsOneAsync()
    {
        _gameContainer!.EverybodyGetsOneList.ForEach(thisTemp =>
        {
            var thisPlayer = _gameContainer.PlayerList![thisTemp.Player];
            var thisCard = _gameContainer.DeckList!.GetSpecificItem(thisTemp.Deck);
            thisCard.Drew = false;
            thisCard.IsSelected = false;
            thisPlayer.MainHandList.Add(thisCard);
        });
        _gameContainer.EverybodyGetsOneList.Clear();
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetSelf();
        _gameContainer.SortCards!();
        _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
        _gameContainer.CurrentAction = null;
        await _processes.AnalyzeQueAsync();
    }
}
