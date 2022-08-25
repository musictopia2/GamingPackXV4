namespace XPuzzle.Core.ViewModels;
[InstanceGame]
public partial class XPuzzleMainViewModel : ScreenViewModel, IBasicEnableProcess, IBlankGameVM, IAggregatorContainer
{
    private readonly XPuzzleGameBoardClass _gameBoard;
    private readonly BasicData _basicData;
    private readonly ISystemError _error;
    private readonly IToast _toast;
    public XPuzzleMainViewModel(IEventAggregator aggregator,
        CommandContainer commandContainer,
        XPuzzleGameBoardClass gameBoard,
        BasicData basicData,
        ISystemError error,
        IToast toast
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        _gameBoard = gameBoard;
        _basicData = basicData;
        _error = error;
        _toast = toast;
        _basicData.GameDataLoading = true; //has to be here because multiplayer games will be different.
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    IEventAggregator IAggregatorContainer.Aggregator => Aggregator;
    public bool CanEnableBasics()
    {
        return true;
    }
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        await _gameBoard.NewGameAsync();
        _basicData.GameDataLoading = false;
        CommandContainer.UpdateAll(); //maybe needed for when its new game.
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task MakeMoveAsync(XPuzzleSpaceInfo space)
    {
        await _gameBoard!.MakeMoveAsync(space);
        EnumMoveList NextMove = _gameBoard.Results();
        if (NextMove == EnumMoveList.TurnOver)
        {
            return; //will automatically enable it again.
        }
        if (NextMove == EnumMoveList.Won)
        {
            _toast.ShowSuccessToast("Congratulations, you won");
            await this.SendGameOverAsync(_error); //only if you won obviously.
        }
    }
}