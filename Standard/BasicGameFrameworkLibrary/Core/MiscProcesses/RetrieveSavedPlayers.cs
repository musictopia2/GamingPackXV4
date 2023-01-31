namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public class RetrieveSavedPlayers<P, S> : IRetrieveSavedPlayers<P>
    where P : class, IPlayerItem, new()

    where S : BasicSavedGameClass<P>, new()
{
    async Task<PlayerCollection<P>> IRetrieveSavedPlayers<P>.GetPlayerListAsync(string payLoad)
    {
        S saveroot;
        try
        {
            saveroot = await js1.DeserializeObjectAsync<S>(payLoad);
        }
        catch
        {
            return new PlayerCollection<P>(); //try this way.  just means no players which would mean no autoresume this tiem.
            throw;
        }
        return saveroot.PlayerList;
    }
}