namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfacesForHelpers;
public interface IDominoDrawProcesses<D> : IEndTurn
    where D : IDominoInfo, new()
{
    Task DrawDominoAsync(D domino);
}