namespace Racko.Core.ViewModels;
[InstanceGame]
public partial class RackoMainViewModel(CommandContainer commandContainer,
        RackoMainGameClass mainGame,
        RackoVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        RackoGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<RackoCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
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
        return !gameContainer!.AlreadyDrew;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await mainGame!.PickupFromDiscardAsync();
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public bool CanPlayOnPile => gameContainer!.AlreadyDrew;
    public async Task PlayOnPileAsync(RackoCardInformation card)
    {
        await gameContainer!.SendDiscardMessageAsync(card.Deck);
        await mainGame.DiscardAsync(card);
    }
    public bool CanDiscardCurrent => gameContainer!.AlreadyDrew;
    [Command(EnumCommandCategory.Game)]
    public async Task DiscardCurrentAsync()
    {
        if (viewModel.OtherPile!.PileEmpty() == true)
        {
            Toast.ShowUserErrorToast("You must have a card to discard");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("discardcurrent");
        }
        await mainGame!.DiscardCurrentAsync();
    }
    public bool CanRacko => !gameContainer!.AlreadyDrew;
    [Command(EnumCommandCategory.Game)]
    public async Task RackoAsync()
    {
        if (mainGame!.HasRacko() == false)
        {
            Toast.ShowUserErrorToast("There is no Racko");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("racko");
        }
        await mainGame.EndRoundAsync();
    }
}