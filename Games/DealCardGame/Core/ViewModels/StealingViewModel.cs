namespace DealCardGame.Core.ViewModels;
[InstanceGame]
public partial class StealingViewModel : IBasicEnableProcess
{
    private readonly DealCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    public readonly DealCardGameVMData VMData;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    private readonly DealCardGameMainGameClass _mainGame;
    public StealingViewModel(DealCardGameGameContainer gameContainer,
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
        _gameContainer.PersonalInformation.StealInfo.StartStealing = false;
        _gameContainer.PersonalInformation.StealInfo.Color = EnumColor.None;
        _gameContainer.PersonalInformation.StealInfo.PlayerId = 0;
        _gameContainer.PersonalInformation.StealInfo.CardChosen = 0;
        _gameContainer.Command.ResetCustomStates();
        await _privateAutoResume.SaveStateAsync(_gameContainer);
        _gameContainer.Command.UpdateAll(); //i think.
    }
    public void AddAction(Action action)
    {
        _gameContainer.Command.CustomStateHasChanged += action;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task StealAsync()
    { 
        if (_gameContainer.PersonalInformation.StealInfo.CardChosen == 0)
        {
            _toast.ShowUserErrorToast("Just choose a card to steal");
            return;
        }
        if (_gameContainer.BasicData.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("stealproperty", _gameContainer.PersonalInformation.StealInfo);
        }
        _gameContainer.Command.ResetCustomStates();
        await _mainGame.PossibleStealingPropertyAsync(_gameContainer.PersonalInformation.StealInfo);
    }
    public bool CanEnableBasics()
    {
        return true;
    }
    public StealPropertyModel StealInfo => _gameContainer.PersonalInformation.StealInfo;
    public DealCardGamePlayerItem GetChosenPlayer => _gameContainer.PlayerList!.Single(x => x.Id == _gameContainer.PersonalInformation.StealInfo.PlayerId);
}