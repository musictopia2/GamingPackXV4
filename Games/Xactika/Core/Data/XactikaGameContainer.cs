namespace Xactika.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class XactikaGameContainer : TrickGameContainer<XactikaCardInformation, XactikaPlayerItem, XactikaSaveInfo, EnumShapes>
{
    public XactikaGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<XactikaCardInformation> deckList,
        IRandomGenerator random) : base(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    {
    }
    internal Func<Task>? LoadBiddingAsync { get; set; }
    internal Func<Task>? CloseBiddingAsync { get; set; }
    internal Func<Task>? LoadShapeButtonAsync { get; set; }
    internal Func<Task>? CloseShapeButtonAsync { get; set; }
    internal bool ShowedOnce { get; set; }
    internal Func<Task>? StartNewTrickAsync { get; set; }
    internal Func<Task>? ShowHumanCanPlayAsync { get; set; }
    internal Action? ShowTurn { get; set; }
}