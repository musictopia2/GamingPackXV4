namespace DealCardGame.Core.ViewModels;
[InstanceGame]
public partial class DealCardGameMainViewModel(CommandContainer commandContainer,
        DealCardGameMainGameClass mainGame,
        DealCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast,
        DealCardGameGameContainer gameContainer,
        PrivateAutoResumeProcesses privateAutoResume) : BasicCardGamesVM<DealCardGameCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    public DealCardGameVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    //anything else needed is here.
    //if i need something extra, will add to template as well.
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        if (IsConfirming())
        {
            return false;
        }
        if (mainGame.SaveRoot.PlaysRemaining > 0)
        {
            return false;
        }
        var player = gameContainer.PlayerList!.GetSelf();
        if (player.MainHandList.Count <= 7)
        {
            return false;
        }
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int deck = VMData.PlayerHand1!.ObjectSelected();
        if (deck == 0)
        {
            Toast.ShowUserErrorToast("You must select a card first");
            return;
        }
        await gameContainer.SendDiscardMessageAsync(deck);
        await mainGame.DiscardAsync(deck);
    }
    protected override bool AlwaysEnableHand()
    {
        return false;
    }
    protected override bool CanEnableHand()
    {
        if (IsConfirming())
        {
            return false;
        }
        return mainGame.SaveRoot.GameStatus != EnumGameStatus.NeedsPayment; //i think.
    }
    //public bool IsSelf => mainGame.SingleInfo!.PlayerCategory == EnumPlayerCategory.Self;
    private bool IsConfirming()
    {
        if (mainGame.SaveRoot.GameStatus == EnumGameStatus.ConfirmPayment)
        {
            return true;
        }
        if (mainGame.SaveRoot.GameStatus == EnumGameStatus.ConsiderJustSayNo)
        {
            return true;
        }
        if (mainGame.SaveRoot.GameStatus == EnumGameStatus.StartDebtCollector)
        {
            return true; //because you have to choose a player to pay the debt.
        }
        if (gameContainer.PersonalInformation.RentInfo.RentCategory != EnumRentCategory.NA)
        {
            return true;
        }
        if (gameContainer.PersonalInformation.StealInfo.StartStealing)
        {
            return true;
        }
        return false;
    }
    public override bool CanEndTurn()
    {
        if (IsConfirming())
        {
            return false;
        }
        var player = mainGame.PlayerList.GetSelf();
        if (player.MainHandList.Count > 7)
        {
            return false;
        }
        return true;
        //return IsConfirming() == false;
    }
    private bool HasPlays => mainGame.SaveRoot.PlaysRemaining > 0;
    public bool CanBank => IsConfirming() == false && HasPlays;
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
            Toast.ShowUserErrorToast("Properties cannot be put into the bank");
            return;
        }
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("bank", card.Deck);
        }
        await mainGame.BankAsync(card.Deck);
    }
    private DealCardGameCardInformation? GetSelectedCard()
    {
        var list = VMData.PlayerHand1.ListSelectedObjects();
        if (list.Count == 0)
        {
            Toast.ShowUserErrorToast("There was no card selected");
            return null;
        }
        if (list.Count > 1)
        {
            Toast.ShowUserErrorToast("Should have only had the possibility of selecting one card");
            return null;
        }
        var card = list.Single();
        return card;
    }
    public bool CanSetChosen => IsConfirming() == false && HasPlays;
    [Command(EnumCommandCategory.Game)]
    public async Task SetChosenAsync(SetPlayerModel model)
    {
        var player = mainGame.PlayerList[model.PlayerId];
        DealCardGameCardInformation? card;
        if (player.PlayerCategory == EnumPlayerCategory.Self)
        {
            //this means you clicked on your own.
            card = GetSelectedCard();
            if (card is null)
            {
                return;
            }
            if (card.CardType == EnumCardType.PropertyRegular || card.CardType == EnumCardType.PropertyWild)
            {
                await PlayPropertyAsync(card, model.Color);
                return;
            }
            if (card.ActionCategory == EnumActionCategory.House || card.ActionCategory == EnumActionCategory.Hotel)
            {
                await PlayHouseOrHotelAsync(card, model.Color);
                return;
            }
            if (card.CardType == EnumCardType.ActionRent)
            {
                await StartRentAsync(model, card);
                return;
            }
            Toast.ShowUserErrorToast("Must choose another player for this one");
            return;
        }
        card = GetSelectedCard();
        if (card is null)
        {
            return;
        }
        if (card.ActionCategory == EnumActionCategory.DealBreaker)
        {
            await DealBreakerProcessesAsync(card, model);
            return;
        }
        if (card.ActionCategory == EnumActionCategory.SlyDeal)
        {
            await StealPropertyAsync(model, card);
            return;
        }
        if (card.ActionCategory == EnumActionCategory.ForcedDeal)
        {
            await TradePropertyAsync(model, card);
            return;
        }
        if (card.ActionCategory == EnumActionCategory.JustSayNo)
        {
            Toast.ShowUserErrorToast("Just say no can never be played on other players");
            return;
        }
        Toast.ShowUserErrorToast("Cannot click another player for this card");
        //Toast.ShowUserErrorToast("Cannot choose another player for anything yet");
    }
    private bool CanStealProperty(SetPlayerModel model)
    {
        var player = mainGame.PlayerList[model.PlayerId];
        SetPropertiesModel property = player.SetData.Single(x => x.Color == model.Color);
        if (property.HasRequiredSet == true)
        {
            Toast.ShowUserErrorToast("Cannot steal because this is already a complete set");
            return false;
        }
        if (property.Cards.Count == 0)
        {
            Toast.ShowUserErrorToast("Cannot steal because this group has no properties");
            return false;
        }
        return true;
    }
    private bool CanTradeProperty(SetPlayerModel model)
    {
        var player = mainGame.PlayerList[model.PlayerId];
        SetPropertiesModel property = player.SetData.Single(x => x.Color == model.Color);
        if (property.HasRequiredSet == true)
        {
            Toast.ShowUserErrorToast("Cannot trade because this is already a complete set");
            return false;
        }
        if (property.Cards.Count == 0)
        {
            Toast.ShowUserErrorToast("Cannot trade because this group has no properties");
            return false;
        }
        return true;
    }
    public async Task TradePropertyAsync(SetPlayerModel model, DealCardGameCardInformation card)
    {
        if (CanTradeProperty(model) == false)
        {
            return;
        }
        await mainGame.StartTradingPropertyAsync(model, card);
    }
    private async Task StealPropertyAsync(SetPlayerModel model, DealCardGameCardInformation card)
    {
        if (CanStealProperty(model) == false)
        {
            return;
        }
        await mainGame.StartStealingPropertyAsync(model, card);
    }
    private bool CanChargeRent(SetPlayerModel model, DealCardGameCardInformation card)
    {
        if (card.AnyColor)
        {
            return true;
        }
        if (card.FirstColorChoice == model.Color || card.SecondColorChoice == model.Color)
        {
            return true;
        }
        Toast.ShowUserErrorToast("Cannot charge rent because you chose a property group different than the card supports");
        return false;
    }
    private async Task StartRentAsync(SetPlayerModel model, DealCardGameCardInformation card)
    {
        if (CanChargeRent(model, card) == false)
        {
            return;
        }
        //don't communicate with other players yet.
        await mainGame.StartRentAsync(model, card);
    }
    private bool CanStealSet(SetPlayerModel model)
    {
        var player = mainGame.PlayerList[model.PlayerId];
        SetPropertiesModel property = player.SetData.Single(x => x.Color == model.Color);
        if (property.HasRequiredSet == false)
        {
            Toast.ShowUserErrorToast("Cannot steal the set because the player did not even have a complete set");
            return false;
        }
        return true;
    }
    private async Task DealBreakerProcessesAsync(DealCardGameCardInformation card, SetPlayerModel model)
    {
        if (CanStealSet(model) == false)
        {
            return;
        }
        if (mainGame.BasicData.MultiPlayer)
        {
            StealSetModel steal = new()
            {
                Color = model.Color,
                PlayerId = model.PlayerId,
                Deck = card.Deck
            };
            await mainGame.Network!.SendAllAsync("stealset", steal);
        }
        await mainGame.StartToStealSetAsync(card.Deck, model.PlayerId, model.Color);
    }
    private bool CanPlayHouseOrHotel(DealCardGameCardInformation card, EnumColor color)
    {
        var player = mainGame.PlayerList.GetWhoPlayer();
        SetPropertiesModel property = player.SetData.Single(x => x.Color == color);
        if (property.Cards.Any(x => x.ActionCategory == card.ActionCategory))
        {
            if (card.ActionCategory == EnumActionCategory.House)
            {
                Toast.ShowUserErrorToast("You can only have one house for a property set");
                return false;
            }
            if (card.ActionCategory == EnumActionCategory.Hotel)
            {
                Toast.ShowUserErrorToast("You can only have one hotel for a property set");
                return false;
            }
        }
        if (property.Color == EnumColor.Black || property.Color == EnumColor.Lime)
        {
            Toast.ShowUserErrorToast("You cannot play a house or hotel for railroads or utilities type of properties which are black and lime");
            return false;
        }
        if (property.HasRequiredSet == false)
        {
            Toast.ShowUserErrorToast("You cannot play a house or hotel if you do not have the required sets");
            return false;
        }
        if (card.ActionCategory == EnumActionCategory.Hotel)
        {
            if (property.HasRequiredHouse == false)
            {
                Toast.ShowUserErrorToast("Must have a house before playing a hotel");
                return false;
            }
        }
        return true;
    }
    private async Task PlayHouseOrHotelAsync(DealCardGameCardInformation card, EnumColor color)
    {
        if (CanPlayHouseOrHotel(card, color) == false)
        {
            return;
        }
        await FinishPlayPropertyHouseHotel(card, color);
    }
    private bool CanPlayProperty(DealCardGameCardInformation card, EnumColor color)
    {
        if (card.CardType == EnumCardType.PropertyRegular)
        {
            if (card.MainColor != color)
            {
                Toast.ShowUserErrorToast("Cannot play property to this set");
                return false;
            }
            return true;
        }
        if (card.AnyColor)
        {
            return true;
        }
        if (card.FirstColorChoice == color || card.SecondColorChoice == color)
        {
            return true;
        }
        Toast.ShowUserErrorToast("Wrong color for this wild property");
        return false;
    }
    private async Task PlayPropertyAsync(DealCardGameCardInformation card, EnumColor color)
    {
        if (CanPlayProperty(card, color) == false)
        {
            return;
        }
        await FinishPlayPropertyHouseHotel(card, color);
    }
    private async Task FinishPlayPropertyHouseHotel(DealCardGameCardInformation card, EnumColor color)
    {
        if (mainGame.BasicData.MultiPlayer)
        {
            SetCardModel set = new(card.Deck, color);
            await mainGame.Network!.SendAllAsync("playproperty", set);
        }
        await mainGame.PlayPropertyAsync(card.Deck, color);
    }
    private bool IsProperAction(DealCardGameCardInformation card)
    {
        if (card.ActionCategory == EnumActionCategory.None)
        {
            Toast.ShowUserErrorToast("This is not an action card");
            return false;
        }
        if (card.ActionCategory == EnumActionCategory.Gos)
        {
            if (mainGame.PlayerList.GetSelf().MainHandList.Count > 6)
            {
                Toast.ShowUserErrorToast("You cannot play pass go because you have over 6 cards in hand which could require wasting cards which hurt the game.");
                return false;
            }
        }
        if (card.ActionCategory == EnumActionCategory.DebtCollector
            || card.ActionCategory == EnumActionCategory.Birthday ||
            card.ActionCategory == EnumActionCategory.Gos)
        {
            return true;
        }
        if (card.ActionCategory == EnumActionCategory.House || card.ActionCategory == EnumActionCategory.Hotel)
        {
            Toast.ShowUserErrorToast("You play houses and hotels by choosing your property group to expand it");
            return false;
        }
        if (card.ActionCategory == EnumActionCategory.DoubleRent)
        {
            Toast.ShowUserErrorToast("This gets played automatically if you choose to double the rent and you have it");
            return false;
        }
        if (card.ActionCategory == EnumActionCategory.JustSayNo)
        {
            Toast.ShowUserErrorToast("Just say no is a special action you take to remove the action against you or reenable the action against the other player");
            return false;
        }
        if (card.ActionCategory == EnumActionCategory.DealBreaker)
        {
            Toast.ShowUserErrorToast("Deal breaker gets played by choosing a property group of your opponent");
            return false;
        }
        if (card.ActionCategory == EnumActionCategory.ForcedDeal || card.ActionCategory == EnumActionCategory.SlyDeal)
        {
            Toast.ShowUserErrorToast("Forced deals and sly deals get played by choosing a property group and choosing a card from that property");
            return false;
        }
        Toast.ShowUserErrorToast("Cannot play action card for unknown reason.  If this should be possible, rethink");
        return false;
    }
    public bool CanPlay => IsConfirming() == false && HasPlays;
    [Command(EnumCommandCategory.Game)]
    public async Task PlayAsync()
    {
        var card = GetSelectedCard();
        if (card is null)
        {
            return;
        }
        if (IsProperAction(card) == false)
        {
            return;
        }
        if (card.ActionCategory != EnumActionCategory.SlyDeal && card.ActionCategory != EnumActionCategory.ForcedDeal)
        {
            if (mainGame.BasicData.MultiPlayer)
            {
                await mainGame.Network!.SendAllAsync("playaction", card.Deck);
            }

            await mainGame.PlayActionAsync(card.Deck);
            return;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ResumeAsync()
    {
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("resume");
        }
        await mainGame.ResumeAsync();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ChoosePlayerAsync()
    {
        if (VMData.PlayerPicker.SelectedIndex == 0)
        {
            Toast.ShowUserErrorToast("Must choose a player to use debt collector against");
            return;
        }
        var player = mainGame.PlayerList.Single(x => x.NickName == VMData.PlayerPicker.GetText(VMData.PlayerPicker.SelectedIndex));
        if (mainGame.BasicData.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("playerchosenfordebt", player.Id);
        }
        VMData.PlayerPicker.SelectedIndex = 0; //has to reset again.   hopefully doing here is fine.
        await mainGame.ChosePlayerForDebtAsync(player.Id);
    }
    public bool CanStartOrganizing => IsConfirming() == false;
    [Command(EnumCommandCategory.Game)]
    public async Task StartOrganizing() //a person can always organize even though they don't have any plays left
    {
        gameContainer.PersonalInformation.Organizing = true;
        //needs to populate the stuff as well.
        gameContainer.PersonalInformation.TemporaryCards.Clear();
        var player = gameContainer.PlayerList!.GetSelf();
        player.ClonePlayerProperties(gameContainer.PersonalInformation);
        await privateAutoResume.SaveStateAsync(gameContainer);
        gameContainer.Command.UpdateSpecificAction(DealCardGameActions.Main); //risk this one.
    }
}