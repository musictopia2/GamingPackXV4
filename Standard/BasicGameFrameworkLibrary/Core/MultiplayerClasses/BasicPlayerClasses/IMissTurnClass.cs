namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.BasicPlayerClasses;
public interface IMissTurnClass<P> where P : IPlayerItem
{
    Task PlayerMissTurnAsync(P player);
}