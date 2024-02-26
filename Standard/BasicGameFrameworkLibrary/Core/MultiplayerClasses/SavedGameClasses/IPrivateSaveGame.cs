namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;
public interface IPrivateSaveGame
{
    Task SaveStateAsync<T>(T thisState)
        where T : IMappable, new();
    Task<string> SavedDataAsync<T>()
        where T : IMappable, new(); //blank means no data.

    //for now, okay.  could rethink for next major version of game package.
}