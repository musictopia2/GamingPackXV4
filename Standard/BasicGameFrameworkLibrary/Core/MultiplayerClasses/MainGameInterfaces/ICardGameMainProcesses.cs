namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainGameInterfaces;
public interface ICardGameMainProcesses<D> : IEndTurn where D : IDeckObject, new()
{
    Task ContinueTurnAsync(); //do need to keep the option open to be async.
    Task DrawAsync(); //this for sure is needed.
    Task PickupFromDiscardAsync(); //we no longer have single.
    Task DiscardAsync(int deck);
    Task DiscardAsync(D card);
    int PlayerDraws { get; set; }
    int LeftToDraw { get; set; }
    IGameNetwork? Network { get; }
}