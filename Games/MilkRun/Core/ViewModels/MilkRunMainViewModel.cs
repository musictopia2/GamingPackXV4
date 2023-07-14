namespace MilkRun.Core.ViewModels;
[InstanceGame]
public class MilkRunMainViewModel : BasicCardGamesVM<MilkRunCardInformation>
{
    private readonly MilkRunMainGameClass _mainGame;
    private readonly MilkRunVMData _model;
    private readonly MilkRunGameContainer _gameContainer;
    private readonly IToast _toast;
    public MilkRunMainViewModel(CommandContainer commandContainer,
        MilkRunMainGameClass mainGame,
        MilkRunVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        MilkRunGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _gameContainer = gameContainer;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        _model.PlayerHand1.Maximum = 8;
    }
    protected override bool CanEnableDeck()
    {
        if (_model.Deck1.IsEndOfDeck())
        {
            return false; //because its the end of the deck period now.
        }
        return _mainGame!.SaveRoot!.CardsDrawn != 2;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int newDeck = _model.PlayerHand1!.ObjectSelected();
        if (newDeck > 0)
        {
            if (_mainGame!.SaveRoot!.CardsDrawn < 2 && _model.Deck1.IsEndOfDeck() == false)
            {
                _toast.ShowUserErrorToast("Sorry, must draw the 2 cards first before discarding");
                return;
            }
            if (newDeck == _gameContainer.PreviousCard)
            {
                _toast.ShowUserErrorToast("Cannot discard the same card you picked up");
                return;
            }
            await _gameContainer.SendDiscardMessageAsync(newDeck);
            await _mainGame.DiscardAsync(newDeck);
            return;
        }
        if (_mainGame!.SaveRoot!.DrawnFromDiscard == true)
        {
            _toast.ShowUserErrorToast("Sorry, you already picked up one card from discard.  Cannot pickup another one.  If you want to discard, then choose a card to discard");
            return;
        }
        if (_model.Pile1!.PileEmpty())
        {
            _toast.ShowUserErrorToast("Sorry, there is no card to pickup from the discard.");
            return;
        }
        await _mainGame.PickupFromDiscardAsync();
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}