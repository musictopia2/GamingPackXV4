namespace BasicGameFrameworkLibrary.Core.CommonInterfaces;
public interface ICardPreDealSetup<D, P>
        where D : IDeckObject, new()
        where P : class, IPlayerObject<D>, new()
{
    void SetUpPreDealCards(PlayerCollection<P> playerList, IListShuffler<D> deckList);
}