namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.SavedGameClasses;

public interface IMultiplayerSaveState
{
    Task DeleteGameAsync();
    Task DeleteMultiplayerGameAsync(); //this means it will have to delete multiplayer game.  because when transferring to desktop, says multiplayer is false but this is exception.
    Task SaveStateAsync<T>(T thisState)
        where T : IMappable, new();
    Task<string> SavedDataAsync<T>()
        where T : IMappable, new(); //blank means no data.
    Task<string> TempMultiSavedAsync();
    Task<EnumRestoreCategory> SinglePlayerRestoreCategoryAsync();
    Task<EnumRestoreCategory> MultiplayerRestoreCategoryAsync();
}