namespace Savannah.Core.Data;
[SingletonGame]
[AutoReset] //usually needs reset
public class SavannahGameContainer : CardGameContainer<RegularSimpleCard, SavannahPlayerItem, SavannahSaveInfo>
{
    public SavannahGameContainer(BasicData basicData,
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
    //the deckcount will be different once i make the necessary changes.
    //public int DeckCount => 52 * PlayerList!.Count;
    public int DeckCount
    {
        get
        {
            int firsts = 54 * PlayerList!.Count;
            int seconds = PlayerList.Count * 2;
            //return firsts;
            return firsts + seconds;
        }
    }
    public Func<Task>? UnselectAllPilesAsync { get; set; } //the main game will do this.
    public Func<Task>? DiscardAsync { get; set; }
    public Action? WrongDiscardProcess { get; set; }
}