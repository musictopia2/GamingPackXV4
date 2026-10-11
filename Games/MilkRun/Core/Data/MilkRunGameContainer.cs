namespace MilkRun.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class MilkRunGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<MilkRunCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<MilkRunCardInformation, MilkRunPlayerItem, MilkRunSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Func<int, int, EnumPileType, EnumMilkType, bool>? CanMakeMove { get; set; }
}