namespace RageCardGame.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class RageCardGameGameContainer : TrickGameContainer<RageCardGameCardInformation, RageCardGamePlayerItem, RageCardGameSaveInfo, EnumColor>
{
    public RageCardGameGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<RageCardGameCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal Func<Task>? ColorChosenAsync { get; set; }
    internal Action? ShowLeadColor { get; set; }
    internal Func<Task>? ChooseColorAsync { get; set; }
}