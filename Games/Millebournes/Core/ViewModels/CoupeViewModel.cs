namespace Millebournes.Core.ViewModels;
[InstanceGame]
public partial class CoupeViewModel
{
    private readonly CommandContainer _commandContainer;
    private readonly MillebournesVMData _model;
    private readonly MillebournesGameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly MillebournesMainGameClass _mainGame;
    public CoupeViewModel(CommandContainer commandContainer, 
        MillebournesVMData model, 
        MillebournesGameContainer gameContainer, 
        IToast toast,
        MillebournesMainGameClass mainGame)
    {
        _commandContainer = commandContainer;
        _model = model;
        _gameContainer = gameContainer;
        _toast = toast;
        _mainGame = mainGame;
        _model.Stops.TimeUp += Stops_TimeUp;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public void Close()
    {
        _model.Stops.TimeUp -= Stops_TimeUp;
    }
    private void CloseCoupe()
    {
        if (_gameContainer.CloseCoupe == null)
        {
            throw new CustomBasicException("Nobody is handling closing coupe.  Rethink");
        }
        _gameContainer.CloseCoupe.Invoke();
    }
    private async void Stops_TimeUp()
    {
        CloseCoupe();
        _commandContainer!.IsExecuting = true;
        _commandContainer.ManuelFinish = true;
        if (_gameContainer.BasicData!.MultiPlayer == false)
        {
            await _mainGame.EndPartAsync(false);
            return;
        }
        MillebournesPlayerItem thisPlayer = _gameContainer!.PlayerList!.GetSelf();
        await _gameContainer.Network!.SendAllAsync("timeup", thisPlayer.Id);
        await _mainGame.EndCoupeAsync(thisPlayer.Id); //iffy.
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task CoupeAsync()
    {
        _model.Stops!.PauseTimer();
        CloseCoupe();
        _gameContainer!.SingleInfo = _gameContainer.PlayerList!.GetSelf();
        _gameContainer.CurrentCP = _gameContainer.FindTeam(_gameContainer.SingleInfo.Team);
        bool rets = _mainGame.HasCoupe(out int newDeck);
        SendPlay thisSend;
        if (rets == false)
        {
            _toast.ShowWarningToast("No Coup Foures Here");
            if (_gameContainer.BasicData!.MultiPlayer == true)
            {
                thisSend = new ();
                thisSend.Player = _gameContainer.SingleInfo.Id;
                thisSend.Team = _gameContainer.SingleInfo.Team;
                await _gameContainer.Network!.SendAllAsync("nocoupe", thisSend); //looks like multiplayer has a bug with no coupe.  has to be fixed.
            }
            _gameContainer.CurrentCP.IncreaseWrongs();
            _mainGame.UpdateGrid(_gameContainer.SingleInfo.Team);
            if (_mainGame.BasicData.MultiPlayer == false)
            {
                await _mainGame.EndPartAsync(false);
                return;
            }
            await _mainGame.EndCoupeAsync(_mainGame.SingleInfo!.Id);
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer == false)
        {
            await _mainGame.ProcessCoupeAsync(newDeck, _mainGame.SingleInfo!.Id);
            return;
        }
        thisSend = new ();
        thisSend.Player = _mainGame.SingleInfo!.Id;
        thisSend.Deck = newDeck;
        await _mainGame.Network!.SendAllAsync("hascoupe", thisSend);
        _gameContainer.CurrentCoupe.Player = _gameContainer.SingleInfo.Id;
        _gameContainer.CurrentCoupe.Card = newDeck;
        await _mainGame.EndCoupeAsync(_mainGame.SingleInfo.Id);
    }
}