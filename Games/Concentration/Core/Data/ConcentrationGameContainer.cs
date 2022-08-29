namespace Concentration.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class ConcentrationGameContainer : CardGameContainer<RegularSimpleCard, ConcentrationPlayerItem, ConcentrationSaveInfo>
{
    public ConcentrationGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<RegularSimpleCard> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}