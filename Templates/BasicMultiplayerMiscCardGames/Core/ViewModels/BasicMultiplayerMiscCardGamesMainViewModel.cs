namespace BasicMultiplayerMiscCardGames.Core.ViewModels;
[InstanceGame]
public class BasicMultiplayerMiscCardGamesMainViewModel(CommandContainer commandContainer,
    BasicMultiplayerMiscCardGamesMainGameClass mainGame,
    BasicMultiplayerMiscCardGamesVMData viewModel,
    BasicData basicData,
    IGamePackageResolver resolver,
    IEventAggregator aggregator,
    IToast toast
        ) : BasicCardGamesVM<BasicMultiplayerMiscCardGamesCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    private readonly BasicMultiplayerMiscCardGamesMainGameClass _mainGame = mainGame; //if we don't need, delete.
    public BasicMultiplayerMiscCardGamesVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        //anything you need upon loading here (so i can still use primary constructors).
        base.AdditionalInitialize();
    }
    protected override Task OpenAsync()
    {
        //if i need this to run upon open, here
        return base.OpenAsync();
    }
    protected override Task CloseAsync()
    {
        //anything needed to close is here.
        return base.CloseAsync();
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