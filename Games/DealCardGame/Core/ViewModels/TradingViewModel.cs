namespace DealCardGame.Core.ViewModels;
[InstanceGame]
public partial class TradingViewModel : IBasicEnableProcess
{
    private readonly DealCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    public readonly DealCardGameVMData VMData;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    private readonly DealCardGameMainGameClass _mainGame;
    public TradingViewModel(DealCardGameGameContainer gameContainer,
        IToast toast,
        DealCardGameVMData model,
        PrivateAutoResumeProcesses privateAutoResume,
        DealCardGameMainGameClass mainGame)
    {
        CreateCommands(gameContainer.Command);
        _gameContainer = gameContainer;
        _toast = toast;
        VMData = model;
        _privateAutoResume = privateAutoResume;
        _mainGame = mainGame;
    }
    public CommandContainer GetCommandContainer => _gameContainer.Command;
    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.Game)]
    public async Task CancelAsync()
    {
        _gameContainer.PersonalInformation.TradeInfo.StartTrading = false;
        _gameContainer.PersonalInformation.TradeInfo.PlayerId = 0;
        _gameContainer.PersonalInformation.TradeInfo.OpponentColor = EnumColor.None;
        _gameContainer.PersonalInformation.TradeInfo.CardPlayed = 0;
        _gameContainer.PersonalInformation.TradeInfo.OpponentCard = 0;
        _gameContainer.PersonalInformation.TradeInfo.YourCard = 0;
        _gameContainer.PersonalInformation.TradeInfo.YourColor = EnumColor.None;
        _gameContainer.Command.ResetCustomStates();
        await _privateAutoResume.SaveStateAsync(_gameContainer);
        _gameContainer.Command.UpdateAll(); //i think.
    }
    public void AddAction(Action action)
    {
        _gameContainer.Command.CustomStateHasChanged += action;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task TradeAsync()
    {
        if (_gameContainer.PersonalInformation.TradeInfo.OpponentCard == 0)
        {
            _toast.ShowUserErrorToast("Needs to choose a card from the opponent for trading");
            return;
        }
        if (_gameContainer.PersonalInformation.TradeInfo.YourCard == 0)
        {
            _toast.ShowUserErrorToast("Needs to choose your card to trade with your opponent");
            return;
        }
        if (_gameContainer.PersonalInformation.TradeInfo.YourColor == EnumColor.None)
        {
            _toast.ShowUserErrorToast("Needs to know the color so it knows what color the opponent receives from yours");
            return;
        }
        if (_gameContainer.BasicData.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("trade", _gameContainer.PersonalInformation.TradeInfo);
        }
        _gameContainer.Command.ResetCustomStates();
        await _mainGame.FinishTradingPropertyAsync(_gameContainer.PersonalInformation.TradeInfo);
    }
    public bool CanEnableBasics()
    {
        return true;
    }
    public TradePropertyModel TradeInfo => _gameContainer.PersonalInformation.TradeInfo;
    public DealCardGamePlayerItem GetYourPlayer => _gameContainer.SingleInfo!;
    public DealCardGamePlayerItem GetChosenPlayer => _gameContainer.PlayerList!.Single(x => x.Id == _gameContainer.PersonalInformation.TradeInfo.PlayerId);
}