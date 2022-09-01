namespace RoundsCardGame.Core.ViewModels;
[InstanceGame]
public class RoundsCardGameMainViewModel : BasicCardGamesVM<RoundsCardGameCardInformation>
{
    private readonly RoundsCardGameMainGameClass _mainGame; //if we don't need, delete.
    private readonly RoundsCardGameVMData _model;
    private readonly RoundsCardGameGameContainer _gameContainer; //if not needed, delete.
    public RoundsCardGameMainViewModel(CommandContainer commandContainer,
        RoundsCardGameMainGameClass mainGame,
        RoundsCardGameVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        RoundsCardGameGameContainer gameContainer,
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