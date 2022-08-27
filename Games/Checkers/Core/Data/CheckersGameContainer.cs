namespace Checkers.Core.Data;
[SingletonGame]
[AutoReset]
public class CheckersGameContainer : BasicGameContainer<CheckersPlayerItem, CheckersSaveInfo>
{
    public CheckersGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
        Animates = new AnimateBasicGameBoard(aggregator);
        Animates.LongestTravelTime = 200;
        CurrentCrowned = false;
    }
    public AnimateBasicGameBoard Animates;
    public bool CurrentCrowned;
    public BasicList<MoveInfo> CompleteMoveList { get; set; } = new();
    public BasicList<MoveInfo> CurrentMoveList { get; set; } = new();
    public BasicList<SpaceCP>? SpaceList;
    public bool CanUpdate { get; set; } = true;
}