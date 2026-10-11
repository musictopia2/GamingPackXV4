namespace XPuzzle.Core.ViewModels;
[InstanceGame]
public partial class XPuzzleMainViewModel(CommandContainer commandContainer,
    XPuzzleGameBoardClass gameBoard,
    BasicData basicData, IEventAggregator aggregator
        ) : IBasicEnableProcess, 
    IMainGameVM, IHandleAsync<OpenMainEventModel>, IHandleAsync<CloseMainEventModel>
{
    partial void CreateCommands(CommandContainer command);
    private partial void Subscribe();
    private partial void Unsubscribe();
    public CommandContainer CommandContainer { get; set; } = commandContainer;
    public bool CanEnableBasics()
    {
        return true;
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task MakeMoveAsync(XPuzzleSpaceInfo space)
    {
        await gameBoard!.MakeMoveAsync(space); //hopefully good enough.   
    }
    void IMainGameVM.Initialize()
    {
        Subscribe();
        CreateCommands(CommandContainer);
        basicData.GameDataLoading = true; //has to be here because multiplayer games will be different.
    }

    async Task IHandleAsync<OpenMainEventModel>.HandleAsync(OpenMainEventModel message)
    {
        await gameBoard.NewGameAsync();
        basicData.GameDataLoading = false;
        //maybe don't need updateall anymore.
        //CommandContainer.UpdateAll(); //maybe needed for when its new game.
    }

    Task IHandleAsync<CloseMainEventModel>.HandleAsync(CloseMainEventModel message)
    {
        Unsubscribe();
        //anything i need to clean up is here.
        return Task.CompletedTask;
    }
}