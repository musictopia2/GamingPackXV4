namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainGameInterfaces;
public interface ICardGameMainProcesses<D> : IEndTurn where D : IDeckObject, new()
{
    Task ContinueTurnAsync();
    Task DrawAsync();
    Task PickupFromDiscardAsync();
    Task DiscardAsync(int deck);
    Task DiscardAsync(D card);
    int PlayerDraws { get; set; }
    int LeftToDraw { get; set; }
    IGameNetwork? Network { get; }
}