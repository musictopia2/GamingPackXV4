namespace FirstSample.Core.ViewModels;
[InstanceGame]
public partial class FirstSampleMainViewModel(IEventAggregator aggregator, IGamePackageResolver resolver) : IBasicEnableProcess, IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    private FirstSampleMainGameClass _mainGame = null!;
    public bool CanEnableBasics()
    {
        return true;
    }
    private partial void Subscribe();
    private partial void Unsubscribe();
    void IMainGameVM.Initialize()
    {
        _mainGame = resolver.ReplaceObject<FirstSampleMainGameClass>();
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