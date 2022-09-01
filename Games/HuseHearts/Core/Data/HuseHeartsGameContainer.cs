namespace HuseHearts.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class HuseHeartsGameContainer : TrickGameContainer<HuseHeartsCardInformation, HuseHeartsPlayerItem, HuseHeartsSaveInfo, EnumSuitList>
{
    public HuseHeartsGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        HuseHeartsDelegates delegates,
        IListShuffler<HuseHeartsCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
        delegates.SetDummyList = ((value) => SaveRoot.DummyList = value);
        delegates.GetDummyList = (() => SaveRoot.DummyList);
    }
}