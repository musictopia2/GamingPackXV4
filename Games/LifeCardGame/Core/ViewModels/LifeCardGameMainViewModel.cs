namespace LifeCardGame.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class LifeCardGameMainViewModel(CommandContainer commandContainer,
        LifeCardGameMainGameClass mainGame,
        LifeCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        LifeCardGameGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<LifeCardGameCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.PlayerHand1.Maximum = 5;
        viewModel.CurrentPile.SendEnableProcesses(this, () => false);
        CommandContainer!.ExecutingChanged = CommandContainer_ExecutingChanged;
        gameContainer.LoadOtherScreen = LoadOtherScreen;
        gameContainer.CloseOtherScreen = CloseOtherScreen;
        CreateCommands();
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands();
    partial void CreateCommands(CommandContainer command);
    private void LoadOtherScreen()
    {
        if (OtherScreen != null)
        {
            return;
        }
        OtherScreen = Resolver.Resolve<OtherViewModel>();
    }
    private void CloseOtherScreen()
    {
        if (OtherScreen == null)
        {
            return;
        }
        OtherScreen = null;
    }
    [StateProperty]
    public partial OtherViewModel? OtherScreen { get; set; }
    private void CommandContainer_ExecutingChanged()
    {
        mainGame!.PlayerList!.EnableLifeStories(mainGame, viewModel, !CommandContainer!.IsExecuting); //i think
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return viewModel.CurrentPile!.PileEmpty();
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int newDeck = viewModel.PlayerHand1!.ObjectSelected();
        if (newDeck == 0)
        {
            Toast.ShowUserErrorToast("Sorry, must select a card first");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("discard", newDeck);
        }
        await mainGame!.DiscardAsync(newDeck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    [Command(EnumCommandCategory.Old)]
    public void YearsPassed()
    {
        Toast.ShowInfoToast($"{mainGame.SaveRoot!.YearsPassed()} passed.  Once it reaches 60; the game is over");
    }
    public bool CanPlayCard => viewModel.CurrentPile.PileEmpty();
    [Command(EnumCommandCategory.Game)]
    public async Task PlayCardAsync()
    {
        int decks = viewModel.PlayerHand1.ObjectSelected();
        if (decks == 0)
        {
            Toast.ShowUserErrorToast("Must choose a card to play");
            return;
        }
        var thisCard = mainGame.SingleInfo!.MainHandList.GetSpecificItem(decks);
        if (mainGame.CanPlayCard(thisCard) == false)
        {
            Toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("playcard", decks);
        }
        CommandContainer!.ManuelFinish = true;
        await mainGame.PlayCardAsync(thisCard);
    }
}