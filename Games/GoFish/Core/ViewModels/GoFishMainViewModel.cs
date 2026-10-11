namespace GoFish.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class GoFishMainViewModel(CommandContainer commandContainer,
        GoFishMainGameClass mainGame,
        GoFishVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        GoFishGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<RegularSimpleCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = false;
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        gameContainer.LoadAskScreenAsync = LoadAskScreen;
        base.AdditionalInitialize();
    }
    protected override Task OpenAsync()
    {
        if (mainGame.SaveRoot.RemovePairs == false)
        {
            LoadAskScreen();
        }
        return base.OpenAsync();
    }
    protected override Task CloseAsync()
    {
        gameContainer.LoadAskScreenAsync = null;
        return base.CloseAsync();
    }
    
    private void LoadAskScreen()
    {
        if (AskScreen != null)
        {
            return;
        }
        AskScreen = Resolver.Resolve<AskViewModel>();
    }
    [StateProperty]
    public partial AskViewModel? AskScreen { get; set; }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        return mainGame!.SaveRoot!.RemovePairs == true || mainGame.SaveRoot.NumberAsked == true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        var thisList = viewModel.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count != 2)
        {
            Toast.ShowUserErrorToast("Must select 2 cards to throw away");
            return;
        }
        if (mainGame!.IsValidMove(thisList) == false)
        {
            Toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            SendPair thisPair = new();
            thisPair.Card1 = thisList.First().Deck;
            thisPair.Card2 = thisList.Last().Deck;
            await mainGame.Network!.SendAllAsync("processplay", thisPair);
        }
        await mainGame.ProcessPlayAsync(thisList.First().Deck, thisList.Last().Deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}