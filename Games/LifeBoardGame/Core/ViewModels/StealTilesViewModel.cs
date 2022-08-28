namespace LifeBoardGame.Core.ViewModels;
public partial class StealTilesViewModel : BasicSubmitViewModel
{
    private readonly LifeBoardGameGameContainer _gameContainer;
    private readonly LifeBoardGameVMData _model;
    private readonly IStolenTileProcesses _processes;
    public StealTilesViewModel(
        CommandContainer commandContainer,
        LifeBoardGameGameContainer gameContainer,
        LifeBoardGameVMData model,
        IStolenTileProcesses processes,
        IEventAggregator aggregator
        ) : base(commandContainer, aggregator)
    {
        if (gameContainer.SaveRoot.GameStatus != EnumWhatStatus.NeedStealTile)
        {
            throw new CustomBasicException("Was not stealing tiles");
        }
        _gameContainer = gameContainer;
        _model = model;
        _processes = processes;
        _model.PlayerChosen = "";
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public override bool CanSubmit => _model.PlayerChosen != "";
    public override Task SubmitAsync()
    {
        return _processes.TilesStolenAsync(_model.PlayerChosen);
    }
    [Command(EnumCommandCategory.Plain)]
    public Task EndTurnAsync()
    {
        return _gameContainer.EndTurnAsync!.Invoke();
    }
}