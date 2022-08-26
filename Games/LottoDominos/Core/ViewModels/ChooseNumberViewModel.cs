namespace LottoDominos.Core.ViewModels;
[InstanceGame]
public partial class ChooseNumberViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly LottoDominosMainGameClass _mainGame;

    public ChooseNumberViewModel(CommandContainer commandContainer,
        LottoDominosMainGameClass mainGame,
        IGamePackageResolver resolver,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _mainGame = mainGame;
        _mainGame.ProcessNumberAsync = ChooseNumberAsync;
        _mainGame.ComputerChooseNumberAsync = ComputerChooseNumberAsync;
        _mainGame.ReloadNumberLists = ReloadNumberLists;
        if (_mainGame.SaveRoot.GameStatus != Data.EnumStatus.ChooseNumber)
        {
            throw new CustomBasicException("Can't load the choose number view model because the status is not even choose number");
        }
        Number1 = new NumberPicker(commandContainer, resolver);
        Number1.ChangedNumberValueAsync += Number1_ChangedNumberValueAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public async Task ChooseNumberAsync(int numberChosen)
    {
        _mainGame.SingleInfo!.NumberChosen = numberChosen;
        Number1!.SelectNumberValue(numberChosen);
        if (_mainGame.SingleInfo.CanSendMessage(_mainGame.BasicData) == true)
        {
            await _mainGame.Network!.SendAllAsync("numberchosen", numberChosen);
        }
        CommandContainer.UpdateAll();
        if (_mainGame.SingleInfo.PlayerCategory != EnumPlayerCategory.Self && _mainGame.Test!.NoAnimations == false)
        {
            await _mainGame.Delay!.DelaySeconds(1);
        }
        await _mainGame.EndTurnAsync();
    }
    public async Task ComputerChooseNumberAsync()
    {
        await ChooseNumberAsync(Number1.NumberToChoose());
    }
    public void ReloadNumberLists()
    {
        Number1!.UnselectAll();
        NumberToChoose = -1;
        Number1.LoadNumberList(_mainGame.GetNumberList());
    }
    public int NumberToChoose { get; set; }
    public bool CanChooseNumber => NumberToChoose > -1;

    [Command(EnumCommandCategory.Plain)]
    public async Task ChooseNumberAsync()
    {
        await ChooseNumberAsync(NumberToChoose);
    }
    public NumberPicker Number1;
    private Task Number1_ChangedNumberValueAsync(int chosen)
    {
        NumberToChoose = chosen;
        return Task.CompletedTask;
    }
    public CommandContainer CommandContainer { get; set; }
}