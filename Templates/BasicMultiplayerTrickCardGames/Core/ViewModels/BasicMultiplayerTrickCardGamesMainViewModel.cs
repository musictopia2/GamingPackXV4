namespace BasicMultiplayerTrickCardGames.Core.ViewModels;
[InstanceGame]
public class BasicMultiplayerTrickCardGamesMainViewModel(CommandContainer commandContainer,
        BasicMultiplayerTrickCardGamesMainGameClass mainGame,
        BasicMultiplayerTrickCardGamesVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        BasicMultiplayerTrickCardGamesGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<BasicMultiplayerTrickCardGamesCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    private readonly BasicMultiplayerTrickCardGamesVMData _viewModel = viewModel;
    protected override void AdditionalInitialize()
    {
        _viewModel.Deck1.NeverAutoDisable = true;
        base.AdditionalInitialize();
    }
    //anything else needed is here.
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