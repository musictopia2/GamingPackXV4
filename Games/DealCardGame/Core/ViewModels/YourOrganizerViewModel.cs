namespace DealCardGame.Core.ViewModels;
[InstanceGame]
public partial class YourOrganizerViewModel : IBasicEnableProcess
{
    private readonly DealCardGameGameContainer _gameContainer;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
    private readonly DealCardGameMainGameClass _mainGame;
    private readonly IToast _toast;
    public YourOrganizerViewModel(DealCardGameGameContainer gameContainer, 
        DealCardGameVMData model,
        PrivateAutoResumeProcesses privateAutoResume,
        DealCardGameMainGameClass mainGame,
        IToast toast
        )
    {
        _gameContainer = gameContainer;
        VMData = model;
        _privateAutoResume = privateAutoResume;
        _mainGame = mainGame;
        _toast = toast;
        CreateCommands(_gameContainer.Command);
        VMData.YourCompleteSets.SetClickedAsync = SetClickedAsync;
    }
    private async Task SetClickedAsync(EnumColor color)
    {
        await VMData.YourCompleteSets.AddCardToSetAsync(color);
        //await VMData.YourCompleteSets.AddCardToSetAsync()
    }
    public void AddAction(Action action)
    {
        _gameContainer.Command.CustomStateHasChanged += action;
    }
    partial void CreateCommands(CommandContainer command);
    public DealCardGameVMData VMData { get; set; }
    [Command(EnumCommandCategory.Game)]
    public async Task CancelAsync()
    {
        await VMData.YourCompleteSets.ResetAsync(false); //you have to reset first.
        _gameContainer.PersonalInformation.Organizing = false; //no longer organizing
        await _privateAutoResume.SaveStateAsync(_gameContainer);
        _gameContainer.Command.ResetCustomStates();
        _gameContainer.Command.UpdateAll();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ResetAsync()
    {
        await VMData.YourCompleteSets.ResetAsync(true);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task FinishAsync()
    {
        if (VMData.YourCompleteSets.IsAcceptable() == false)
        {
            return; //because not acceptable.  should give a toast.
        }
        _gameContainer.Command.ResetCustomStates();
        await _mainGame.FinishOrganizingSetsAsync(_gameContainer.PersonalInformation.SetData);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PutToTemporaryHandAsync()
    {
        if (VMData.YourCompleteSets.HasSelectedCard == false)
        {
            _toast.ShowUserErrorToast("You never chose a card to put into temporary hand");
            return;
        }
        await VMData.YourCompleteSets.AddCardToTemporaryHandAsync();
    }
    public bool CanEnableBasics()
    {
        return true;
    }
}