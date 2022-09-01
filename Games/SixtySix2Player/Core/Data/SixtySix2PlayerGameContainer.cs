namespace SixtySix2Player.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class SixtySix2PlayerGameContainer : TrickGameContainer<SixtySix2PlayerCardInformation, SixtySix2PlayerPlayerItem, SixtySix2PlayerSaveInfo, EnumSuitList>
{
    public SixtySix2PlayerGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<SixtySix2PlayerCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
}