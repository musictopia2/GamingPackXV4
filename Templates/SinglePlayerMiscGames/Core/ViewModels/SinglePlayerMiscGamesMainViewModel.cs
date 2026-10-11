namespace SinglePlayerMiscGames.Core.ViewModels;
[InstanceGame]
public partial class SinglePlayerMiscGamesMainViewModel(IEventAggregator aggregator, IGamePackageResolver resolver) : IBasicEnableProcess, IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    private SinglePlayerMiscGamesMainGameClass _mainGame = null!;
    public bool CanEnableBasics()
    {
        return true;
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IMainGameVM.Initialize()
    {
        _mainGame = resolver.ReplaceObject<SinglePlayerMiscGamesMainGameClass>();
        Subscribe();
        //if i have to do the processes for commands, do here.
        //also any other setup would be here too.
    }
    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        await _mainGame.NewGameAsync();
    }
    Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
        return Task.CompletedTask;
    }
}