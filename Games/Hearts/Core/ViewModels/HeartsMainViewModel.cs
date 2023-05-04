namespace Hearts.Core.ViewModels;
[InstanceGame]
public class HeartsMainViewModel : BasicCardGamesVM<HeartsCardInformation>
{
    private readonly HeartsMainGameClass _mainGame; //if we don't need, delete.
    private readonly HeartsVMData _model;
    private readonly HeartsGameContainer _gameContainer; //if not needed, delete.
    public HeartsMainViewModel(CommandContainer commandContainer,
        HeartsMainGameClass mainGame,
        HeartsVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        HeartsGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _gameContainer = gameContainer;
        _model.Deck1.NeverAutoDisable = true;
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