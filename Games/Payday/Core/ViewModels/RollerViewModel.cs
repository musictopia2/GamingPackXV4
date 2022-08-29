namespace Payday.Core.ViewModels;
[InstanceGame]
public partial class RollerViewModel : ScreenViewModel, IBlankGameVM
{
    private readonly PaydayMainGameClass _mainGame;
    private readonly StandardRollProcesses<SimpleDice, PaydayPlayerItem> _roller;
    public RollerViewModel(CommandContainer commandContainer, PaydayMainGameClass mainGame, StandardRollProcesses<SimpleDice, PaydayPlayerItem> roller, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _mainGame = mainGame;
        _roller = roller;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    public bool CanRollDice
    {
        get
        {
            EnumStatus gameStatus = _mainGame!.SaveRoot!.GameStatus;
            return gameStatus == EnumStatus.Starts || gameStatus == EnumStatus.RollCharity ||
                gameStatus == EnumStatus.RollLottery || gameStatus == EnumStatus.RollRadio;
        }
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task RollDiceAsync()
    {
        await _roller.RollDiceAsync();
    }
}