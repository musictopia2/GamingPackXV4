namespace Backgammon.Core.Data;
[SingletonGame]
[AutoReset]
public class BackgammonGameContainer : BasicGameContainer<BackgammonPlayerItem, BackgammonSaveInfo>
{
    public BackgammonGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
        Animates = new(aggregator);
        Animates.LongestTravelTime = 100;
    }
    public bool RefreshPieces { get; set; }
    public Dictionary<int, TriangleClass> TriangleList { get; set; } = new Dictionary<int, TriangleClass>();
    public AnimateBasicGameBoard Animates { get; set; }
    public bool MoveInProgress { get; set; }
    public BasicList<MoveInfo> MoveList { get; set; } = new();
    public int FirstDiceValue { get; set; }
    public int SecondDiceValue { get; set; }
    public bool HadDoubles()
    {
        if (FirstDiceValue == 0)
        {
            throw new CustomBasicException("The dice can never roll a 0.  Must populate the dice value first");
        }
        return FirstDiceValue == SecondDiceValue;
    }
    public Func<int, Task>? MakeMoveAsync { get; set; }
    public Action? DiceVisibleProcesses { get; set; }
}