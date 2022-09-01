namespace Pinochle2Player.Core.ViewModels;
[InstanceGame]
public partial class Pinochle2PlayerMainViewModel : BasicCardGamesVM<Pinochle2PlayerCardInformation>
{
    private readonly Pinochle2PlayerMainGameClass _mainGame; //if we don't need, delete.
    private readonly Pinochle2PlayerVMData _model;
    private readonly IToast _toast;
    public Pinochle2PlayerMainViewModel(CommandContainer commandContainer,
        Pinochle2PlayerMainGameClass mainGame,
        Pinochle2PlayerVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        _model.OpponentMelds.SendEnableProcesses(this, () => false);
        commandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
        _model.YourMelds.SendEnableProcesses(this, () =>
        {
            if (_mainGame!.SaveRoot!.ChooseToMeld)
            {
                _model.YourMelds.AutoSelect = EnumHandAutoType.SelectAsMany;
                return true;
            }
            _model.YourMelds.AutoSelect = EnumHandAutoType.SelectOneOnly;
            return _model.Pile1!.PileEmpty() == false && _mainGame.SaveRoot.MeldList.Any(items => items.Player == _mainGame.WhoTurn && items.CardList.Count > 0);
        });
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer!.IsExecuting)
        {
            return; //for now, has to be this way.
        }
        if (_mainGame!.SaveRoot!.ChooseToMeld)
        {
            _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
        }
        else
        {
            _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectOneOnly;
        }
    }
    protected override Task TryCloseAsync()
    {
        CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
        return base.TryCloseAsync();
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        if (_mainGame!.SaveRoot!.ChooseToMeld == false)
        {
            return false;
        }
        return _model.Pile1!.GetCardInfo().Value != EnumRegularCardValueList.Nine && _mainGame.SingleInfo!.MainHandList.Any(items => items.Value == EnumRegularCardValueList.Nine && items.Suit == _mainGame.SaveRoot.TrumpSuit);
    }
    public override bool CanEndTurn()
    {
        if (_model.PlayerHand1!.HasSelectedObject() || _model.YourMelds!.HasSelectedObject())
        {
            return false;
        }
        return _mainGame!.SaveRoot!.ChooseToMeld;
    }

    protected override async Task ProcessDiscardClickedAsync()
    {
        int hands = _model.PlayerHand1!.HowManySelectedObjects;
        if (hands == 0)
        {
            _toast.ShowUserErrorToast("Must choose a card from hand in order to exchange for the top card");
            return;
        }
        if (hands > 1)
        {
            _toast.ShowUserErrorToast("Must choose only one card from hand to exchange");
            return;
        }
        Pinochle2PlayerCardInformation thisCard;
        thisCard = _mainGame!.SingleInfo!.MainHandList.GetSpecificItem(_model.PlayerHand1.ObjectSelected());
        if (thisCard.Value > EnumRegularCardValueList.Nine)
        {
            _toast.ShowUserErrorToast("Must choose a nine to exchange");
            return;
        }
        if (thisCard.Suit != _mainGame!.SaveRoot!.TrumpSuit)
        {
            _toast.ShowUserErrorToast("Must choose the nine of the trump suit in order to exchange");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("exchangediscard", thisCard.Deck);
        }
        await _mainGame.ExchangeDiscardAsync(thisCard.Deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public bool CanMeld => _mainGame!.SaveRoot!.ChooseToMeld;
    [Command(EnumCommandCategory.Game)]
    public async Task MeldAsync()
    {
        if (_model.PlayerHand1!.HasSelectedObject() == false)
        {
            _toast.ShowUserErrorToast("Must choose at least one card from hand in order to meld");
            return;
        }
        var completeList = _model.PlayerHand1.ListSelectedObjects();
        DeckRegularDict<Pinochle2PlayerCardInformation> otherList = new();
        if (_model.YourMelds!.HasSelectedObject())
        {
            otherList = _model.YourMelds.ListSelectedObjects();
            completeList.AddRange(otherList);
        }
        var thisMeld = _mainGame!.GetMeldFromList(completeList);
        if (thisMeld.ClassAValue == EnumClassA.None && thisMeld.ClassBValue == EnumClassB.None && thisMeld.ClassCValue == EnumClassC.None)
        {
            _toast.ShowUserErrorToast("There is no meld combinations here");
            return;
        }
        if (_model.YourMelds.HasSelectedObject())
        {
            foreach (var thisCard in otherList)
            {
                var tempMeld = _mainGame.GetMeldFromCard(thisCard);
                if (tempMeld.ClassAValue == EnumClassA.Dix)
                {
                    throw new CustomBasicException("Should have caught the problem with using dix earlier");
                }
                if (tempMeld.ClassBValue == thisMeld.ClassBValue && thisMeld.ClassBValue > EnumClassB.None)
                {
                    _toast.ShowUserErrorToast("Cannot reuse class b for class b");
                    return;
                }
                if (tempMeld.ClassCValue == thisMeld.ClassCValue && thisMeld.ClassCValue > EnumClassC.None)
                {
                    _toast.ShowUserErrorToast("Cannot reuse a pinochle for a pinochle");
                    return;
                }
                if (tempMeld.ClassAValue <= thisMeld.ClassAValue && thisMeld.ClassAValue > EnumClassA.None)
                {
                    _toast.ShowUserErrorToast("Cannot download class A to get more points or to create another of same value");
                    return;
                }
            }
        }
        var deckList = completeList.GetDeckListFromObjectList();
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("meld", deckList);
        }
        await _mainGame.MeldAsync(deckList);
    }
}