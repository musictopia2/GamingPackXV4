namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.GameContainers;
public class TrickGameContainer<D, P, SA, TS>(BasicData basicData,
    TestOptions test,
    IGameInfo gameInfo,
    IAsyncDelayer delay,
    IEventAggregator aggregator,
    CommandContainer command,
    IGamePackageResolver resolver,
    IListShuffler<D> deckList,
    IRandomGenerator random) : CardGameContainer<D, P, SA>(basicData, test, gameInfo, delay, aggregator, command, resolver, deckList, random)
    where TS : IFastEnumSimple
    where D : class, ITrickCard<TS>, new()
    where P : class, IPlayerTrick<TS, D>, new()
    where SA : BasicSavedTrickGamesClass<TS, D, P>, new()
{
    public int SelfPlayer => PlayerList!.Single(x => x.PlayerCategory == EnumPlayerCategory.Self).Id;
    public D GetBrandNewCard(int deck)
    {
        D thisCard = DeckList!.GetSpecificItem(deck);
        return (D)thisCard.CloneCard();
    }
    public D GetSpecificCardFromDeck(int deck)
    {
        return DeckList!.GetSpecificItem(deck);
    }
    public Func<Task>? CardClickedAsync { get; set; }
    public Func<Task>? ContinueTrickAsync { get; set; }
    public Func<Task>? EndTrickAsync { get; set; }
}