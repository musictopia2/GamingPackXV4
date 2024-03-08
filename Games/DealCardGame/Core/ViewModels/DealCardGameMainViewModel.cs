namespace DealCardGame.Core.ViewModels;
[InstanceGame]
public partial class DealCardGameMainViewModel : BasicCardGamesVM<DealCardGameCardInformation>
{
    //private readonly DealCardGameMainGameClass MainGame;
    private readonly DealCardGameMainGameClass _mainGame; //if we don't need, delete.
    private readonly IToast _toast;
    public DealCardGameVMData VMData { get; set; }
    public DealCardGameMainViewModel(CommandContainer commandContainer,
        DealCardGameMainGameClass mainGame,
        DealCardGameVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
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


    [Command(EnumCommandCategory.Game)]
    public async Task BankAsync()
    {
        var card = GetSelectedCard();
        if (card is null)
        {
            return;
        }
        if (card.CardType == EnumCardType.PropertyWild || card.CardType == EnumCardType.PropertyRegular)
        {
            _toast.ShowUserErrorToast("Properties cannot be put into the bank");
            return;
        }
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("bank", card.Deck);
        }
        await _mainGame.BankAsync(card.Deck);
    }
    private DealCardGameCardInformation? GetSelectedCard()
    {
        var list = VMData.PlayerHand1.ListSelectedObjects();
        if (list.Count == 0)
        {
            _toast.ShowUserErrorToast("There was no card selected");
            return null;
        }
        if (list.Count > 1)
        {
            _toast.ShowUserErrorToast("Should have only had the possibility of selecting one card");
            return null;
        }
        var card = list.Single();
        return card;
    }


    [Command(EnumCommandCategory.Game)]
    public async Task SetChosenAsync(SetPlayerModel model)
    {
        var player = _mainGame.PlayerList[model.PlayerId];
        if (player.PlayerCategory == EnumPlayerCategory.Self)
        {
            //this means you clicked on your own.
            var card = GetSelectedCard();
            if (card is null)
            {
                return;
            }
            if (card.CardType == EnumCardType.PropertyRegular || card.CardType == EnumCardType.PropertyWild)
            {
                await PlayPropertyAsync(card, model.Color);
                return;
            }
            if (card.CardType == EnumCardType.ActionRent)
            {
                _toast.ShowUserErrorToast("Charging rent is not supported yet");
                //await RentAsync(card, model.Color);
                return;
            }
            _toast.ShowUserErrorToast("For now, cannot click on your cards because only rent and property cards are supported");
            return;
        }
        _toast.ShowUserErrorToast("Cannot choose another player for anything yet");
    }
    private bool CanPlayProperty(DealCardGameCardInformation card, EnumColor color)
    {
        if (card.CardType == EnumCardType.PropertyRegular)
        {
            if (card.MainColor != color)
            {
                _toast.ShowUserErrorToast("Cannot play property to this set");
                return false;
            }
            return true;
        }
        if (card.AnyColor)
        {
            return true;
        }
        if (card.FirstColorChoice == color ||  card.SecondColorChoice == color)
        {
            return true;
        }
        _toast.ShowUserErrorToast("Wrong color for this wild property");
        return false;
    }
    private async Task PlayPropertyAsync(DealCardGameCardInformation card, EnumColor color)
    {
        if (CanPlayProperty(card, color) == false)
        {
            return;
        }
        if (_mainGame.BasicData.MultiPlayer)
        {
            SetCardModel set = new(card.Deck, color);
            await _mainGame.Network!.SendAllAsync("playproperty", set);
        }
        await _mainGame.PlayPropertyAsync(card.Deck, color);
    }
    //private async Task RentAsync(DealCardGameCardInformation card, EnumColor color)
    //{

    //}

    [Command(EnumCommandCategory.Game)]
    public async Task PlayAsync()
    {
        var card = GetSelectedCard();
        if (card is null)
        {
            return;
        }
        if (card.CardType == EnumCardType.Money)
        {
            _toast.ShowUserErrorToast("Cannot play money.  Either discard or put into your bank");
            return;
        }
        if (card.ActionCategory != EnumActionCategory.Gos)
        {
            _toast.ShowUserErrorToast("For now, only gos can be played");
            return;
        }
        if (_mainGame.BasicData.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("playaction", card.Deck);
        }
        await _mainGame.PlayActionAsync(card.Deck);
    }
}