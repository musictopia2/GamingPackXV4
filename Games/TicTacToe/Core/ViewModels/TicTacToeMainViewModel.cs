namespace TicTacToe.Core.ViewModels;
[InstanceGame]
public partial class TicTacToeMainViewModel : BasicMultiplayerMainVM
{
    private readonly TicTacToeMainGameClass _mainGame; //if we don't need, delete.
    public TicTacToeVMData VMData { get; set; }
    public TicTacToeMainViewModel(CommandContainer commandContainer,
        TicTacToeMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        TicTacToeVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    public bool CanMakeMove(SpaceInfoCP space)
    {
        if (VMData is null)
        {
            return false; //otherwise, forces me to make static.  for now, can't do.
        }
        return space.Status == EnumSpaceType.Blank;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeMoveAsync(SpaceInfoCP space)
    {
        await _mainGame.MakeMoveAsync(space);
    }
}