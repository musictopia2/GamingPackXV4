namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public interface IRetrieveSavedPlayers<P>
     where P : class, IPlayerItem, new()
{
    Task<PlayerCollection<P>> GetPlayerListAsync(string payLoad);
}