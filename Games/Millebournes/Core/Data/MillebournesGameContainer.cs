namespace Millebournes.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class MillebournesGameContainer : CardGameContainer<MillebournesCardInformation, MillebournesPlayerItem, MillebournesSaveInfo>
{
    public MillebournesGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<MillebournesCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    public CoupeInfo CurrentCoupe = new();
    public TeamCP? CurrentCP;
    public BasicList<TeamCP> TeamList = new();
    public TeamCP FindTeam(int teamNumber)
    {
        return TeamList.Single(items => items.TeamNumber == teamNumber);
    }
    internal Func<EnumPileType, int, Task>? TeamClickAsync { get; set; }
    internal Func<Task>? CloseCoupeAsync { get; set; }
    internal Func<Task>? LoadCoupeAsync { get; set; }
}