namespace BattleshipLite.Core.ViewModels;
[InstanceGame]
public partial class BattleshipLiteMainViewModel : BasicMultiplayerMainVM
{
    private readonly BattleshipLiteMainGameClass _mainGame;
    private readonly BattleshipBoardClass _board;
    public BattleshipLiteVMData VMData { get; set; }
    public BattleshipLiteMainViewModel(CommandContainer commandContainer,
        BattleshipLiteMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BattleshipBoardClass board,
        BattleshipLiteVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        _board = board;
        VMData = data;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanMakeMove(ShipInfo ship) //still picks this up though via source generators
    {
        return _board.CanMakeMove(ship);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeMoveAsync(ShipInfo ship)
    {
        await _board.MakeMoveAsync(ship);
    }
    public BasicList<ShipInfo> YourPlacedShips()
    {
        BattleshipLitePlayerItem player = _mainGame.PlayerList.GetSelf();
        return player.Ships.Where(x => x.WasShip).ToBasicList();
    }
    public BattleshipCollection YourCompleteShips => _mainGame.PlayerList.GetSelf().Ships;
    public ShipInfo OpponentShip(ShipInfo ship)
    {
        var player = _mainGame.PlayerList.GetOnlyOpponent();
        return player.Ships[ship.Vector];
    }
}