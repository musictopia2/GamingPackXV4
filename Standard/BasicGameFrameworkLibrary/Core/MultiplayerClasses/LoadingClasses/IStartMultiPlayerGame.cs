namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.LoadingClasses;
public interface IStartMultiPlayerGame<P>
    where P : class, IPlayerItem, new()
{
    Task LoadNewGameAsync(PlayerCollection<P> startList);
    Task LoadSavedGameAsync();
}