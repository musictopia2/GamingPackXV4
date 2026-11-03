namespace TicTacToe.Core.ViewModels;
[InstanceGame]
public partial class TicTacToeMainViewModel(CommandContainer commandContainer,
        TicTacToeMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        TicTacToeVMData data) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public TicTacToeVMData VMData { get; set; } = data;
    partial void CreateCommands(CommandContainer container);
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    public bool CanMakeMove(SpaceInfoCP space)
    {
        if (VMData is null)
        {
            return false; //otherwise, forces me to make static.  for now, can't do.
        }
        return space.Status == EnumSpaceType.Blank;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task MakeMoveAsync(SpaceInfoCP space)
    {
        await mainGame.MakeMoveAsync(space);
    }

    protected override async Task CloseAsync()
    {
        
    }

    protected override async Task OpenAsync()
    {
        
    }
}