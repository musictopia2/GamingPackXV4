namespace SixtySix2Player.Core.ViewModels;
[InstanceGame]
public partial class SixtySix2PlayerMainViewModel : BasicCardGamesVM<SixtySix2PlayerCardInformation>
{
    private readonly SixtySix2PlayerMainGameClass _mainGame; //if we don't need, delete.
    private readonly SixtySix2PlayerVMData _model;
    private readonly IToast _toast;
    public SixtySix2PlayerMainViewModel(CommandContainer commandContainer,
        SixtySix2PlayerMainGameClass mainGame,
        SixtySix2PlayerVMData viewModel,
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
        commandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override Task TryCloseAsync()
    {
        CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
        return base.TryCloseAsync();
    }
    private void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer!.IsExecuting)
        {
            return;
        }
        if (CanAnnounceMarriage)
        {
            _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
        }
        else
        {
            _model.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectOneOnly;
        }
    }
    public bool CanAnnounceMarriage => _model!.TrickArea1!.IsLead && _mainGame.SaveRoot!.CardsForMarriage.Count == 0;
    [Command(EnumCommandCategory.Game)]
    public async Task AnnouceMarriageAsync()
    {
        int howMany = _model.PlayerHand1.HowManySelectedObjects;
        if (howMany != 2)
        {
            _toast.ShowUserErrorToast("Must choose 2 cards");
            return;
        }
        var thisList = _model.PlayerHand1.ListSelectedObjects();
        var thisMarriage = _mainGame!.WhichMarriage(thisList);
        if (thisMarriage == EnumMarriage.None)
        {
            _toast.ShowUserErrorToast("This is not a valid marrige");
            return;
        }
        if (_mainGame.CanShowMarriage(thisMarriage) == false)
        {
            _toast.ShowUserErrorToast("Cannot show marriage because the points will put you over 66 points.");
            return;
        }
        var tempList = thisList.GetDeckListFromObjectList();
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("announcemarriage", tempList);
        }
        await _mainGame.AnnounceMarriageAsync(tempList);
    }
    public bool CanGoOut()
    {
        if (_mainGame!.CanAnnounceMarriageAtBeginning == true || _model.TrickArea1!.IsLead && _mainGame.SaveRoot!.CardsForMarriage.Count == 0)
        {
            return _model.TrickArea1!.IsLead;
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task GoOutAsync()
    {
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("goout");
        }
        await _mainGame!.GoOutAsync();
    }
    public string DeckCount => _model.Deck1.TextToAppear;
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return _mainGame!.CanExchangeForDiscard();
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        if (_mainGame!.CanExchangeForDiscard() == false)
        {
            throw new CustomBasicException("Should have been disabled because cannot exchange for discard");
        }
        int howMany = _model.PlayerHand1!.HowManySelectedObjects;
        if (howMany == 0)
        {
            _toast.ShowUserErrorToast("Must choose a card to exchange");
            return;
        }
        if (howMany > 1)
        {
            _toast.ShowUserErrorToast("Cannot choose more than one card to exchange");
            return;
        }
        int decks = _model.PlayerHand1!.ObjectSelected();
        var thisCard = _mainGame!.SingleInfo!.MainHandList.GetSpecificItem(decks);
        if (thisCard.Value > EnumRegularCardValueList.Nine)
        {
            _toast.ShowUserErrorToast("Must choose a nine to exchange");
            return;
        }
        if (thisCard.Suit != _mainGame.SaveRoot!.TrumpSuit)
        {
            _toast.ShowUserErrorToast("Must choose the nine of the trump suit in order to exchange");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("exchangediscard", thisCard.Deck);
        }
        await _mainGame!.ExchangeDiscardAsync(thisCard.Deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}