namespace Millebournes.Core.ViewModels;
[InstanceGame]
public partial class CoupeViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly MillebournesVMData _model;
    private readonly MillebournesGameContainer _gameContainer;
    private readonly IToast _toast;
    private readonly MillebournesMainGameClass _mainGame;
    public CoupeViewModel(CommandContainer commandContainer, 
        MillebournesVMData model, 
        MillebournesGameContainer gameContainer, 
        IToast toast,
        MillebournesMainGameClass mainGame, 
        IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _model = model;
        _gameContainer = gameContainer;
        _toast = toast;
        _mainGame = mainGame;
        _model.Stops.TimeUp += Stops_TimeUp;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override Task TryCloseAsync()
    {
        _model.Stops.TimeUp -= Stops_TimeUp;
        return base.TryCloseAsync();
    }
    public CommandContainer CommandContainer { get; set; }
    private async Task CloseCoupeAsync()
    {
        if (_gameContainer.CloseCoupeAsync == null)
        {
            throw new CustomBasicException("Nobody is handling closing coupe.  Rethink");
        }
        await _gameContainer.CloseCoupeAsync.Invoke();
    }
    private async void Stops_TimeUp()
    {
        await CloseCoupeAsync();
        CommandContainer!.IsExecuting = true;
        CommandContainer.ManuelFinish = true;
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
        await CloseCoupeAsync();
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
