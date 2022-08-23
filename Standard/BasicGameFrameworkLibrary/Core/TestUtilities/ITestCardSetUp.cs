namespace BasicGameFrameworkLibrary.Core.TestUtilities;

public interface ITestCardSetUp<D, P>
    where D : IDeckObject, new()
    where P : class, IPlayerObject<D>, new()
{
    /// <summary>
    /// Generates test cards.  If none is specified, then none will be used.
    /// This ensures a player at least has the cards specified.  aids in testing.
    /// </summary>
    /// <param name="PlayerList"></param>
    /// <param name="DeckList"></param>
    Task SetUpTestHandsAsync(PlayerCollection<P> playerList, IListShuffler<D> deckList);
}