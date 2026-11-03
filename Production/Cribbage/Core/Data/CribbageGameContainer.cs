namespace Cribbage.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class CribbageGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<CribbageCard> deckList,
    IRandomGenerator random) : CardGameContainer<CribbageCard, CribbagePlayerItem, CribbageSaveInfo>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Func<Task>? NextStepAsync { get; set; }
}