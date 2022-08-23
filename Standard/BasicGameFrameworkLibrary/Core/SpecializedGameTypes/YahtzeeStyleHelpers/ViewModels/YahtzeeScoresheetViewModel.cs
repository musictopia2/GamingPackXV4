namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.ViewModels;

public partial class YahtzeeScoresheetViewModel<D> : ScreenViewModel, IBlankGameVM, IHandleAsync<SelectionChosenEventModel>, IScoresheetAction
    where D : SimpleDice, new()
{
    private readonly ScoreContainer _scoreContainer;
    private readonly YahtzeeGameContainer<D> _gameContainer;
    private readonly IYahtzeeMove _yahtzeeMove;
    private RowInfo? _privateChosen;
    public YahtzeeScoresheetViewModel(
        CommandContainer commandContainer,
        ScoreContainer scoreContainer,
        IEventAggregator aggregator,
        YahtzeeGameContainer<D> gameContainer,
        IYahtzeeMove yahtzeeMove
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _scoreContainer = scoreContainer;
        _gameContainer = gameContainer;
        _yahtzeeMove = yahtzeeMove;
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    public async Task HandleAsync(SelectionChosenEventModel message)
    {
        switch (message.OptionChosen)
        {
            case EnumOptionChosen.Yes:
                await ProcessMoveAsync(_privateChosen!);
                break;
            case EnumOptionChosen.No:
                _gameContainer.Command.ManuelFinish = false;
                _gameContainer.Command.IsExecuting = false;
                break;
            default:
                throw new CustomBasicException("Should have chosen yes or no");
        }
    }
    private static bool HasWarning(RowInfo currentRow)
    {
        return !currentRow.Possible.HasValue;
    }
    public bool CanRow(RowInfo row)
    {
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            return false;
        }
        if (_gameContainer.SaveRoot.RollNumber == 1)
        {
            return false; //hopefully this simple this time.
        }
        return !row.HasFilledIn(); //hopefully this simple now.
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task RowAsync(RowInfo row)
    {
        _privateChosen = row;
        _gameContainer.Command.ManuelFinish = true;
        if (HasWarning(row))
        {
            WarningEventModel warn = new();
            warn.Message = "Are you sure you want to mark off " + row.Description;
            await Aggregator.PublishAsync(warn);
            return;
        }
        await ProcessMoveAsync(row);
    }
    private async Task ProcessMoveAsync(RowInfo row)
    {
        if (_privateChosen == null)
        {
            //return; //try to ignore.  if this works, then won't matter if one is remaining (because host rejoined game).
            throw new CustomBasicException("Did not save selection");
        }
        _privateChosen = null;
        if (_gameContainer.CanSendMessage())
        {
            await _gameContainer.Network!.SendMoveAsync(_scoreContainer.RowList.IndexOf(row));
        }
        await _yahtzeeMove.MakeMoveAsync(row);
    }
}