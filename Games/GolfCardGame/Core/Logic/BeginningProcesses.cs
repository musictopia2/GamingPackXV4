namespace GolfCardGame.Core.Logic;
[SingletonGame]
[AutoReset]
public class BeginningProcesses : IBeginningProcesses
{
    private readonly GolfCardGameGameContainer _gameContainer;
    private readonly GolfDelegates _delegates;
    private readonly GolfCardGameVMData _model;
    public BeginningProcesses(GolfCardGameGameContainer gameContainer, GolfDelegates delegates, GolfCardGameVMData model)
    {
        _gameContainer = gameContainer;
        _delegates = delegates;
        _model = model;
    }
    async Task IBeginningProcesses.SelectBeginningAsync(int player, DeckRegularDict<RegularSimpleCard> selectList, DeckRegularDict<RegularSimpleCard> unselectList)
    {
        //at this point, run the delegate plus lots of other things.
        if (_delegates.LoadMainScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is handling the load main screen.  Rethink");
        }
        if (selectList.Count != 2 || unselectList.Count != 2)
        {
            throw new CustomBasicException("The select and unselect list must contain 2 cards");
        }
        await _delegates.LoadMainScreenAsync();
        _gameContainer.SingleInfo = _gameContainer.PlayerList![player];
        _gameContainer.SingleInfo.MainHandList.ReplaceRange(selectList);
        _gameContainer.SingleInfo.TempSets.ReplaceRange(unselectList);
        _gameContainer.SingleInfo.FinishedChoosing = true;
        _gameContainer.Command.ManuelFinish = true;
        if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.HiddenCards1!.ClearBoard();
            _model.GolfHand1!.ClearBoard();
        }
        if (!_gameContainer.BasicData!.MultiPlayer)
        {
            _gameContainer.SaveRoot!.GameStatus = EnumStatusType.Normal;
            _gameContainer.WhoStarts = _gameContainer.WhoTurn;
            await _gameContainer.StartNewTurnAsync!.Invoke();
            return;
        }
        if (_gameContainer.PlayerList.Any(items => !items.FinishedChoosing))
        {
            _model.Instructions = "Waiting for the other players to finish choosing the 2 cards";
            _gameContainer.Network!.IsEnabled = true;
            return;
        }
        _gameContainer.SaveRoot!.GameStatus = EnumStatusType.Normal;
        await _gameContainer.StartNewTurnAsync!.Invoke();
    }
}