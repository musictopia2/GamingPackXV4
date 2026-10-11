namespace A8RoundRummy.Core.ViewModels;
[InstanceGame]
public partial class A8RoundRummyMainViewModel(CommandContainer commandContainer,
        A8RoundRummyMainGameClass mainGame,
        A8RoundRummyVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        A8RoundRummyGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<A8RoundRummyCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    protected override bool CanEnableDeck()
    {
        return !gameContainer!.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        if (mainGame!.CanProcessDiscard(out bool pickUp, out int deck, out string message) == false)
        {
            Toast.ShowUserErrorToast(message);
            return;
        }
        if (pickUp == true)
        {
            await mainGame.PickupFromDiscardAsync();
            return;
        }
        await gameContainer.SendDiscardMessageAsync(deck);
        await mainGame.DiscardAsync(deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public bool CanGoOut => viewModel.PlayerHand1.HandList.Count == 8; //if 8, you can go out since you have to draw first.
    [Command(EnumCommandCategory.Game)]
    public async Task GoOutAsync()
    {
        if (mainGame!.CanGoOut() == false)
        {
            Toast.ShowUserErrorToast("Sorry; you cannot go out");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            MultiplayerOut thisOut = new();
            thisOut.Deck = mainGame.CardForDiscard!.Deck;
            thisOut.WasGuaranteed = mainGame.WasGuarantee;
            await mainGame.Network!.SendAllAsync("goout", thisOut);
        }
        await mainGame.GoOutAsync();
    }
}