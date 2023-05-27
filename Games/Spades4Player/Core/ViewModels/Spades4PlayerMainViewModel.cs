namespace Spades4Player.Core.ViewModels;
[InstanceGame]
public class Spades4PlayerMainViewModel : BasicCardGamesVM<Spades4PlayerCardInformation>
{
    private readonly Spades4PlayerMainGameClass _mainGame; //if we don't need, delete.
    private readonly Spades4PlayerVMData _model;
    private readonly Spades4PlayerGameContainer _gameContainer; //if not needed, delete.
    public Spades4PlayerMainViewModel(CommandContainer commandContainer,
        Spades4PlayerMainGameClass mainGame,
        Spades4PlayerVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        Spades4PlayerGameContainer gameContainer,
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