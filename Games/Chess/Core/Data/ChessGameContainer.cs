namespace Chess.Core.Data;
[SingletonGame]
[AutoReset]
public class ChessGameContainer : BasicGameContainer<ChessPlayerItem, ChessSaveInfo>
{
    public ChessGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
        Animates = new(aggregator);
        Animates.LongestTravelTime = 200;
        CurrentPiece = EnumPieceType.None;
    }
    public AnimateBasicGameBoard Animates;
    internal BasicList<MoveInfo> CompleteMoveList { get; set; } = new();
    public BasicList<MoveInfo> CurrentMoveList { get; set; } = new(); //needs to be public so blazor ui can use it.
    public BasicList<SpaceCP>? SpaceList;
    public EnumPieceType CurrentPiece { get; set; } //has to be public so blazor can use it.
    public bool CanUpdate { get; set; } = true;
}