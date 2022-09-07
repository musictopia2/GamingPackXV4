namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.MainGameInterfaces;
public interface IGameSetUp<P, S> : IBasicGameProcesses<P>
    where P : class, IPlayerItem, new()
    where S : BasicSavedGameClass<P>
{
    Task SetUpGameAsync(bool isBeginning);
    Task PopulateSaveRootAsync();
    S SaveRoot { get; set; }
    Func<bool, Task>? FinishUpAsync { get; set; }
    Task StartNewTurnAsync();
    bool ComputerEndsTurn { set; }
    Task ContinueTurnAsync();
    Task FinishGetSavedAsync();
}