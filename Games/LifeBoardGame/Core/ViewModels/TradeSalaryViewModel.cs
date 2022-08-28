namespace LifeBoardGame.Core.ViewModels;
public partial class TradeSalaryViewModel : BasicSubmitViewModel
{
    private readonly LifeBoardGameGameContainer _gameContainer;
    private readonly LifeBoardGameVMData _model;
    private readonly ITradeSalaryProcesses _processes;
    public TradeSalaryViewModel(
        CommandContainer commandContainer,
        LifeBoardGameGameContainer gameContainer,
        LifeBoardGameVMData model,
        ITradeSalaryProcesses processes,
        IEventAggregator aggregator
        ) : base(commandContainer, aggregator)
    {
        if (gameContainer.SaveRoot.GameStatus != EnumWhatStatus.NeedTradeSalary)
        {
            throw new CustomBasicException("Was not trading salary.  Therefore, should not have loaded the trade salary view model.  Rethink");
        }

        _gameContainer = gameContainer;
        _model = model;
        _processes = processes;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public override bool CanSubmit => _model.PlayerChosen != "";
    public override Task SubmitAsync()
    {
        return _processes.TradedSalaryAsync(_model.PlayerChosen);
    }
    [Command(EnumCommandCategory.Plain)]
    public Task EndTurnAsync()
    {
        return _gameContainer.EndTurnAsync!.Invoke();
    }
}