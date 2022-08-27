namespace ConnectFour.Core.ViewModels;
[InstanceGame]
public partial class ConnectFourMainViewModel : BasicMultiplayerMainVM
{
    private readonly ConnectFourMainGameClass _mainGame; //if we don't need, delete.
    public ConnectFourVMData VMData { get; set; }
    public ConnectFourMainViewModel(CommandContainer commandContainer,
        ConnectFourMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        ConnectFourVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer container);
    public ConnectFourCollection GetBoardList => _mainGame.SaveRoot.GameBoard;
    public bool CanColumn(SpaceInfoCP space) => !_mainGame.SaveRoot.GameBoard.IsFilled(space.Vector.Column);
    [Command(EnumCommandCategory.Game)]
    public async Task ColumnAsync(SpaceInfoCP space)
    {
        await _mainGame.MakeMoveAsync(space.Vector.Column);
    }
}