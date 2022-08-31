namespace Phase10.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class Phase10GameContainer : CardGameContainer<Phase10CardInformation, Phase10PlayerItem, Phase10SaveInfo>
{
    public Phase10GameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<Phase10CardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}