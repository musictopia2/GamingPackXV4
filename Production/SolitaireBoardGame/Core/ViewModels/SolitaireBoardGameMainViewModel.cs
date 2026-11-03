namespace SolitaireBoardGame.Core.ViewModels;
[InstanceGame]
public partial class SolitaireBoardGameMainViewModel(IEventAggregator aggregator,
    CommandContainer commandContainer, 
    IGamePackageResolver resolver,
    BasicData basicData
    ) : IBasicEnableProcess, IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    private SolitaireBoardGameMainGameClass _mainGame = null !;
    public CommandContainer CommandContainer { get; set; } = commandContainer;

    public bool CanEnableBasics()
    {
        return true;
    }

    private partial void Subscribe();
    private partial void Unsubscribe();
    void IMainGameVM.Initialize()
    {
        _mainGame = resolver.ReplaceObject<SolitaireBoardGameMainGameClass>();
        Subscribe();
        CreateCommands(CommandContainer);
    //if i have to do the processes for commands, do here.
    //also any other setup would be here too.
    }

    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        basicData.GameDataLoading = true;
        await _mainGame.NewGameAsync();
        basicData.GameDataLoading = false;
    }

    Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
        return Task.CompletedTask;
    }

    partial void CreateCommands(CommandContainer command);
    [Command(EnumCommandCategory.Plain)]
    public async Task MakeMoveAsync(GameSpace space)
    {
        await _mainGame.ProcessCommandAsync(space);
    }
}