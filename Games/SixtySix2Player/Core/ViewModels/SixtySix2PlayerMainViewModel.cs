namespace SixtySix2Player.Core.ViewModels;
[InstanceGame]
public partial class SixtySix2PlayerMainViewModel(CommandContainer commandContainer,
        SixtySix2PlayerMainGameClass mainGame,
        SixtySix2PlayerVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<SixtySix2PlayerCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        CommandContainer.ExecutingChanged = null;
        return base.CloseAsync();
    }
    partial void CreateCommands(CommandContainer command);
    private void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer!.IsExecuting)
        {
            return;
        }
        if (CanAnnounceMarriage)
        {
            viewModel.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectAsMany;
        }
        else
        {
            viewModel.PlayerHand1!.AutoSelect = EnumHandAutoType.SelectOneOnly;
        }
    }
    public bool CanAnnounceMarriage => viewModel!.TrickArea1!.IsLead && mainGame.SaveRoot!.CardsForMarriage.Count == 0;
    [Command(EnumCommandCategory.Game)]
    public async Task AnnouceMarriageAsync()
    {
        int howMany = viewModel.PlayerHand1.HowManySelectedObjects;
        if (howMany != 2)
        {
            Toast.ShowUserErrorToast("Must choose 2 cards");
            return;
        }
        var thisList = viewModel.PlayerHand1.ListSelectedObjects();
        var thisMarriage = mainGame!.WhichMarriage(thisList);
        if (thisMarriage == EnumMarriage.None)
        {
            Toast.ShowUserErrorToast("This is not a valid marrige");
            return;
        }
        if (mainGame.CanShowMarriage(thisMarriage) == false)
        {
            Toast.ShowUserErrorToast("Cannot show marriage because the points will put you over 66 points.");
            return;
        }
        var tempList = thisList.GetDeckListFromObjectList();
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("announcemarriage", tempList);
        }
        await mainGame.AnnounceMarriageAsync(tempList);
    }
    public bool CanGoOut()
    {
        if (mainGame!.CanAnnounceMarriageAtBeginning == true || viewModel.TrickArea1!.IsLead && mainGame.SaveRoot!.CardsForMarriage.Count == 0)
        {
            return viewModel.TrickArea1!.IsLead;
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task GoOutAsync()
    {
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("goout");
        }
        await mainGame!.GoOutAsync();
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return mainGame!.CanExchangeForDiscard();
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        if (mainGame!.CanExchangeForDiscard() == false)
        {
            throw new CustomBasicException("Should have been disabled because cannot exchange for discard");
        }
        int howMany = viewModel.PlayerHand1!.HowManySelectedObjects;
        if (howMany == 0)
        {
            Toast.ShowUserErrorToast("Must choose a card to exchange");
            return;
        }
        if (howMany > 1)
        {
            Toast.ShowUserErrorToast("Cannot choose more than one card to exchange");
            return;
        }
        int decks = viewModel.PlayerHand1!.ObjectSelected();
        var thisCard = mainGame!.SingleInfo!.MainHandList.GetSpecificItem(decks);
        if (thisCard.Value > EnumRegularCardValueList.Nine)
        {
            Toast.ShowUserErrorToast("Must choose a nine to exchange");
            return;
        }
        if (thisCard.Suit != mainGame.SaveRoot!.TrumpSuit)
        {
            Toast.ShowUserErrorToast("Must choose the nine of the trump suit in order to exchange");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("exchangediscard", thisCard.Deck);
        }
        await mainGame!.ExchangeDiscardAsync(thisCard.Deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}