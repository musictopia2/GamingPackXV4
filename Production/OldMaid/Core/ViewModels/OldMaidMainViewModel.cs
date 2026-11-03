namespace OldMaid.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class OldMaidMainViewModel(CommandContainer commandContainer,
        OldMaidMainGameClass mainGame,
        OldMaidVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        OldMaidGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<RegularSimpleCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        viewModel.Deck1.NeverAutoDisable = false;
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        gameContainer.ShowOtherCardsAsync = LoadOpponentScreenAsync;
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer container);
    protected override async Task OpenAsync()
    {
        if (mainGame.SaveRoot.RemovePairs == false)
        {
            await LoadOpponentScreenAsync();
        }
    }
    private async Task LoadOpponentScreenAsync()
    {
        if (OpponentScreen != null)
        {
            return;
        }
        OpponentScreen = Resolver.Resolve<OpponentCardsViewModel>();
    }
    protected override Task CloseAsync()
    {
        OpponentScreen = null;
        return base.CloseAsync();
    }
    [StateProperty]
    public partial OpponentCardsViewModel? OpponentScreen { get; set; }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        var thisCol = viewModel.PlayerHand1!.ListSelectedObjects();
        if (thisCol.Count != 2)
        {
            Toast.ShowUserErrorToast("Must select 2 cards to throw away");
            return;
        }
        if (OldMaidMainGameClass.IsValidMove(thisCol) == false)
        {
            Toast.ShowUserErrorToast("Illegal move");
            return;
        }
        await mainGame.ProcessPlayAsync(thisCol.First().Deck, thisCol.Last().Deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public bool CanRemoveAllPairs => mainGame.SaveRoot.RemovePairs;
    [Command(EnumCommandCategory.Game)]
    public async Task RemoveAllPairsAsync()
    {
        var pairs = new List<(int Card1, int Card2)>();

        foreach (var group in viewModel.PlayerHand1.HandList
            .GroupBy(card => card.Value))
        {
            var cards = group.ToList();

            for (int i = 0; i + 1 < cards.Count; i += 2)
            {
                pairs.Add((cards[i].Deck, cards[i + 1].Deck));
            }
        }

        viewModel.PlayerHand1.UnselectAllObjects();

        foreach (var (Card1, Card2) in pairs)
        {
            await mainGame.ProcessPlayAsync(Card1, Card2, false);
        }
        await mainGame.ContinueTurnAsync();
    }
}