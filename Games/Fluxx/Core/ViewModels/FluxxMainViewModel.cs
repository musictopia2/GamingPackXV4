namespace Fluxx.Core.ViewModels;
[InstanceGame]
public partial class FluxxMainViewModel(CommandContainer commandContainer,
        FluxxMainGameClass mainGame,
        FluxxVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        FluxxGameContainer gameContainer,
        IDiscardProcesses discardProcesses,
        IAnalyzeProcesses analyzeQueProcesses,
        KeeperContainer keeperContainer,
        FluxxDelegates delegates,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<FluxxCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast), IShowKeeperVM
{

    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        gameContainer.LoadGive = LoadGive;
        gameContainer.LoadPlay = LoadPlay;
        viewModel.Keeper1.ConsiderSelectOneAsync = OnConsiderSelectOneCardAsync;
        viewModel.Goal1.ConsiderSelectOneAsync = OnConsiderSelectOneCardAsync;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    protected override async Task CloseAsync()
    {
        ClosePlayGive();
        await base.CloseAsync();
    }
    protected override async Task OpenAsync()
    {

        await base.OpenAsync();
        if (gameContainer!.CurrentAction != null && gameContainer.CurrentAction.Deck == EnumActionMain.Taxation)
        {
            LoadGive();
        }
        else
        {
            LoadPlay();
        }
    }
    
    private void ClosePlayGive()
    {
        if (PlayGiveScreen == null)
        {
            return;
        }
        PlayGiveScreen = null;
    }
    private void LoadPlay()
    {
        ClosePlayGive();
        PlayGiveScreen = Resolver.Resolve<PlayViewModel>();
    }
    private void LoadGive()
    {
        ClosePlayGive();
        PlayGiveScreen = Resolver.Resolve<GiveViewModel>();
    }
    public BasicSubmitViewModel? PlayGiveScreen { get; set; }
    ICustomCommand IShowKeeperVM.ShowKeepersCommand => ShowKeepersCommand!;

    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        return mainGame.OtherTurn == 0;
    }
    public override async Task EndTurnAsync()
    {
        var ends = mainGame!.StatusEndRegularTurn();
        if (ends == EnumEndTurnStatus.Goal)
        {
            Toast.ShowUserErrorToast("Sorry; you must get rid of excess goals");
            return;
        }
        if (ends == EnumEndTurnStatus.Play)
        {
            Toast.ShowUserErrorToast("Sorry; you have plays remaining");
            return;
        }
        if (ends == EnumEndTurnStatus.Hand)
        {
            Toast.ShowUserErrorToast("Sorry; you have too many cards in your hand");
            return;
        }
        if (ends == EnumEndTurnStatus.Keeper)
        {
            Toast.ShowUserErrorToast("Sorry; you have too many keepers");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendEndTurnAsync();
        }
        await mainGame.EndTurnAsync();
    }
    protected override Task OnConsiderSelectOneCardAsync(FluxxCardInformation thisObject)
    {
        if (thisObject.Deck == viewModel.CardDetail!.CurrentCard.Deck)
        {
            viewModel.CardDetail.ResetCard();
        }
        else
        {
            viewModel.CardDetail.ShowCard(thisObject);
        }
        return Task.CompletedTask;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task DiscardAsync()
    {
        int goalDiscarded = viewModel.Goal1!.ObjectSelected();
        int keepers = viewModel.Keeper1!.HowManySelectedObjects;
        int yours = viewModel.PlayerHand1!.HowManySelectedObjects;
        if (goalDiscarded == 0 && keepers == 0 && yours == 0)
        {
            Toast.ShowUserErrorToast("There is no cards selected to discard");
            return;
        }
        BasicList<int> tempList = [keepers, goalDiscarded, yours];
        if (tempList.Count(item => item > 0) > 1)
        {
            Toast.ShowUserErrorToast("Can choose a goal, keepers, or from your hand; not from more than one");
            return;
        }
        if (goalDiscarded > 0)
        {
            if (gameContainer!.NeedsToRemoveGoal() == false)
            {
                Toast.ShowUserErrorToast("Cannot discard any goals");
                viewModel.UnselectAllCards();
                return;
            }
            if (goalDiscarded == (int)mainGame.SaveRoot!.GoalList.Last().Deck && mainGame.SaveRoot.GoalList.Count == 3)
            {
                Toast.ShowUserErrorToast("Cannot discard the third goal on the list.  Must choose one of the 2 that was there before");
                viewModel.UnselectAllCards();
                return;
            }
            await discardProcesses.DiscardGoalAsync(goalDiscarded);
            await analyzeQueProcesses.AnalyzeQueAsync();
            return;
        }
        if (gameContainer!.NeedsToRemoveGoal())
        {
            Toast.ShowUserErrorToast("Must discard a goal before discarding anything else");
            viewModel.UnselectAllCards();
            return;
        }
        if (yours > 0)
        {
            if (mainGame.SaveRoot!.HandLimit == -1)
            {
                Toast.ShowUserErrorToast("There is no hand limit.  Therefore cannot discard any cards from your hand");
                viewModel.UnselectAllCards();
                return;
            }
            int newCount = mainGame.SingleInfo!.MainHandList.Count - yours;
            if (newCount < mainGame.SaveRoot.HandLimit)
            {
                Toast.ShowUserErrorToast($"Cannot discard from hand {yours} cards because that will cause you to discard too many cards");
                viewModel.UnselectAllCards();
                return;
            }
            var firstList = viewModel.PlayerHand1.ListSelectedObjects();
            await discardProcesses.DiscardFromHandAsync(firstList);
            return;
        }
        if (keepers > 0)
        {
            if (mainGame.SaveRoot!.KeeperLimit == -1)
            {
                Toast.ShowUserErrorToast("There is no keeper limit.  Therefore; cannot discard any keepers");
                viewModel.UnselectAllCards();
                return;
            }
            int newCount = mainGame.SingleInfo!.KeeperList.Count - keepers;
            if (newCount < mainGame.SaveRoot.KeeperLimit)
            {
                Toast.ShowUserErrorToast($"Cannot discard from keepers {keepers} cards because that will cause you to discard too many keepers");
                viewModel.UnselectAllCards();
                return;
            }
            var firstList = viewModel.Keeper1.ListSelectedObjects();
            DeckRegularDict<FluxxCardInformation> finList = [.. firstList];
            await discardProcesses.DiscardKeepersAsync(finList);
            return;
        }
        throw new CustomBasicException("Don't know how to discard from here");
    }
    [Command(EnumCommandCategory.Game)]
    public void SelectHandCards()
    {
        viewModel.PlayerHand1.SelectAllObjects();
    }
    [Command(EnumCommandCategory.Game)]
    public void UnselectHandCards()
    {
        viewModel.PlayerHand1.UnselectAllObjects();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ShowKeepersAsync()
    {
        keeperContainer.ShowKeepers();
        if (delegates.LoadKeeperScreenAsync == null)
        {
            throw new CustomBasicException("Nobody is loading the keeper screen when main needs to load it");
        }
        await delegates.LoadKeeperScreenAsync.Invoke();
    }
}