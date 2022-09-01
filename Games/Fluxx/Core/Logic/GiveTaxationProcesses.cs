namespace Fluxx.Core.Logic;
[SingletonGame]
[AutoReset]
public class GiveTaxationProcesses : IGiveTaxationProcesses
{
    private readonly FluxxGameContainer _gameContainer;
    private readonly IAnalyzeProcesses _processes;
    public GiveTaxationProcesses(FluxxGameContainer gameContainer, IAnalyzeProcesses processes)
    {
        _gameContainer = gameContainer;
        _processes = processes;
    }
    async Task IGiveTaxationProcesses.GiveCardsForTaxationAsync(IDeckDict<FluxxCardInformation> list)
    {
        if (_gameContainer!.CurrentAction == null)
        {
            throw new CustomBasicException("Must have a current action in order to give cards for taxation");
        }
        if (_gameContainer.CurrentAction.Deck != EnumActionMain.Taxation)
        {
            throw new CustomBasicException("The current action must be taxation");
        }
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetOtherPlayer();
        if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
        {
            await _gameContainer.Network!.SendAllAsync("taxation", list.GetDeckListFromObjectList());
        }
        _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
        list.ForEach(thisCard => thisCard.Drew = true);
        _gameContainer.SingleInfo.MainHandList.AddRange(list);
        if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _gameContainer.SortCards!();
        }
        if (_gameContainer.LoadPlayAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the play button on main screen for taxation.  Rethink");
        }
        await _gameContainer.LoadPlayAsync();
        do
        {
            _gameContainer.OtherTurn = await _gameContainer.PlayerList.CalculateOtherTurnAsync();
            if (_gameContainer.OtherTurn == 0)
            {
                _gameContainer.CurrentAction = null;
                break;
            }
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetOtherPlayer();
            if (_gameContainer.SingleInfo.MainHandList.Count > 0)
            {
                break;
            }
        } while (true);
        _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
        await _processes.AnalyzeQueAsync();
    }
}