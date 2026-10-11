namespace Millebournes.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class MillebournesGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<MillebournesCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<MillebournesCardInformation, MillebournesPlayerItem, MillebournesSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    public CoupeInfo CurrentCoupe = new();
    public TeamCP? CurrentCP;
    public BasicList<TeamCP> TeamList = [];
    public TeamCP FindTeam(int teamNumber)
    {
        return TeamList.Single(items => items.TeamNumber == teamNumber);
    }
    internal Func<EnumPileType, int, Task>? TeamClickAsync { get; set; }
    internal Action? CloseCoupe { get; set; }
    internal Action? LoadCoupe { get; set; }
}