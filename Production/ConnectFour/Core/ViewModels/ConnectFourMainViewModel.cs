namespace ConnectFour.Core.ViewModels;
[InstanceGame]
public partial class ConnectFourMainViewModel(CommandContainer commandContainer,
        ConnectFourMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        ConnectFourVMData data) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public ConnectFourVMData VMData { get; set; } = data;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer container);
    public ConnectFourCollection GetBoardList => mainGame.SaveRoot.GameBoard;
    public bool CanColumn(SpaceInfoCP space) => !mainGame.SaveRoot.GameBoard.IsFilled(space.Vector.Column);
    [Command(EnumCommandCategory.Game)]
    public async Task ColumnAsync(SpaceInfoCP space)
    {
        await mainGame.MakeMoveAsync(space.Vector.Column);
    }
}