namespace BasicMultiplayerRegularCardGames.Core.ViewModels;
[InstanceGame]
public class BasicMultiplayerRegularCardGamesMainViewModel(CommandContainer commandContainer,
    BasicMultiplayerRegularCardGamesMainGameClass mainGame,
    BasicMultiplayerRegularCardGamesVMData viewModel,
    BasicData basicData,
    IGamePackageResolver resolver,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicCardGamesVM<RegularSimpleCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    private readonly BasicMultiplayerRegularCardGamesMainGameClass _mainGame = mainGame; //if we don't need, delete.
    public BasicMultiplayerRegularCardGamesVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        //any additional initialize is here.
        base.AdditionalInitialize();
    }
    //anything else needed is here.
    //if i need something extra, will add to template as well.
    protected override bool CanEnableDeck()
    {
        //todo:  decide whether to enable deck.
        return false; //otherwise, can't compile.
    }
    protected override bool CanEnablePile1()
    {
        //todo:  decide whether to enable deck.
        return false; //otherwise, can't compile.
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        //if we have anything, will be here.
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}