namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.EventModels;
public sealed class RefreshScoreBoardEventModel<P>(
    PlayerCollection<P> players)
    where P : class, IPlayerItem, new()
{
    public PlayerCollection<P> Players { get; } = players;
}