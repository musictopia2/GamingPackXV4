namespace BattleshipLite.Core.ViewModels;
[InstanceGame]
public partial class BattleshipLiteMainViewModel(CommandContainer commandContainer,
        BattleshipLiteMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BattleshipBoardClass board,
        BattleshipLiteVMData data) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public BattleshipLiteVMData VMData { get; set; } = data;
    public BattleshipLiteSaveInfo SaveInfo => mainGame.SaveRoot;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanMakeMove(ShipInfo ship) //still picks this up though via source generators
    {
        return board.CanMakeMove(ship);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeMoveAsync(ShipInfo ship)
    {
        await board.MakeMoveAsync(ship);
    }
    public BasicList<ShipInfo> YourPlacedShips()
    {
        BattleshipLitePlayerItem player = mainGame.PlayerList.GetSelf();
        return player.Ships.Where(x => x.WasShip).ToBasicList();
    }
    public BattleshipLitePlayerItem GetSelf => mainGame.PlayerList.GetSelf();
    public BattleshipCollection YourCompleteShips => mainGame.PlayerList.GetSelf().Ships;
    public ShipInfo OpponentShip(ShipInfo ship)
    {
        var player = mainGame.PlayerList.GetOnlyOpponent();
        return player.Ships[ship.Vector];
    }
}