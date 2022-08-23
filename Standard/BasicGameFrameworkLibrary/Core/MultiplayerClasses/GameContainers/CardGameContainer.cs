namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.GameContainers;
public class CardGameContainer<D, P, S> : BasicGameContainer<P, S>
    where D : class, IDeckObject, new()
    where P : class, IPlayerSingleHand<D>, new()
    where S : BasicSavedCardClass<P, D>, new()
{
    public CardGameContainer(BasicData basicData,
        TestOptions test,
        IGameInfo gameInfo,
        IAsyncDelayer delay,
        IEventAggregator aggregator,
        CommandContainer command,
        IGamePackageResolver resolver,
        IListShuffler<D> deckList,
        IRandomGenerator random)
        : base(basicData, test, gameInfo, delay, aggregator, command, resolver, random)
    {
        DeckList = deckList;
    }
    public int PreviousCard
    {
        get
        {
            return SaveRoot!.PreviousCard;
        }
        set
        {
            SaveRoot!.PreviousCard = value;
        }
    }
    public bool AlreadyDrew
    {
        get
        {
            return SaveRoot!.AlreadyDrew;
        }
        set
        {
            SaveRoot!.AlreadyDrew = value;
        }
    }
    public int PlayerDraws { get; set; }
    public int LeftToDraw { get; set; }
    public IListShuffler<D> DeckList; //i think this could be okay.
    public int PlayerWentOut()
    {
        P player = PlayerList!.Where(x => x.MainHandList.Count == 0).SingleOrDefault()!;
        if (player == null)
        {
            return -1;
        }
        return player.Id;
    }
    public DeckRegularDict<D> GetPlayerCards()
    {
        var firstList = PlayerList!.Select(Items => Items.MainHandList).ToBasicList();
        DeckRegularDict<D> output = new();
        firstList.ForEach(items => output.AddRange(items));
        return output;
    }
    public async Task SendDrawMessageAsync()
    {
        await Network!.SendAllAsync("drawcard");
    }
    public async Task SendDiscardMessageAsync(int deck)
    {
        if (SingleInfo!.CanSendMessage(BasicData!) == false)
        {
            return;
        }
        await Network!.SendAllAsync("discard", deck);
    }
    public Action? SortAfterDrawing { get; set; }
    public Func<Task>? DrawAsync { get; set; }
    public Action? SortCards { get; set; } //needed so other controls can invoke this like monopoly card game.
    public Func<D, Task>? AnimatePlayAsync { get; set; }
}