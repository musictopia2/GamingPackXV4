namespace ListChooser.Core.ViewModels;
[InstanceGame]
public partial class ListChooserMainViewModel(IEventAggregator aggregator, IGamePackageResolver resolver, LevelClass level) : IBasicEnableProcess, IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    private ListChooserMainGameClass _mainGame = null!;
    public bool CanEnableBasics()
    {
        return true;
    }

    public EnumLevel LevelChosen { get; set; }

    private partial void Subscribe();
    private partial void Unsubscribe();
    void IMainGameVM.Initialize()
    {
        _mainGame = resolver.ReplaceObject<ListChooserMainGameClass>();
        LevelChosen = level.Level;
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