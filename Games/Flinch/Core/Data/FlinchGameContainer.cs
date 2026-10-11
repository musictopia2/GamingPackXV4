namespace Flinch.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class FlinchGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<FlinchCardInformation> deckList,
    IRandomGenerator random) : CardGameContainer<FlinchCardInformation, FlinchPlayerItem, FlinchSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Action? LoadPlayerPiles { get; set; }
    internal Func<int, int, bool>? IsValidMove { get; set; }
}