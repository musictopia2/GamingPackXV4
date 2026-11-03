namespace Xactika.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class XactikaGameContainer(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<XactikaCardInformation> deckList,
    IRandomGenerator random) : TrickGameContainer<XactikaCardInformation, XactikaPlayerItem, XactikaSaveInfo, EnumShapes>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
{
    internal Action? LoadBidding { get; set; }
    internal Action? CloseBidding { get; set; }
    internal Action? LoadShapeButton { get; set; }
    internal Action? CloseShapeButton { get; set; }
    internal bool ShowedOnce { get; set; }
    internal Func<Task>? StartNewTrickAsync { get; set; }
    internal Func<Task>? ShowHumanCanPlayAsync { get; set; }
    internal Action? ShowTurn { get; set; }
}