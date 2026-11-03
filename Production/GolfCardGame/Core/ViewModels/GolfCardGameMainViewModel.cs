namespace GolfCardGame.Core.ViewModels;
[InstanceGame]
public partial class GolfCardGameMainViewModel(CommandContainer commandContainer,
        GolfCardGameMainGameClass mainGame,
        GolfCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        GolfCardGameGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<RegularSimpleCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.OtherPile!.PileClickedAsync = OtherPile_PileClickedAsync;
        viewModel.OtherPile.SendEnableProcesses(this, () => gameContainer.AlreadyDrew);
        viewModel.GolfHand1.SendEnableProcesses(this, () => gameContainer.AlreadyDrew);
        viewModel.HiddenCards1.SendEnableProcesses(this, () => gameContainer.AlreadyDrew);
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer commandContainer);
    private async Task OtherPile_PileClickedAsync()
    {
        var tempCard = viewModel.OtherPile!.GetCardInfo();
        if (gameContainer!.PreviousCard == tempCard.Deck)
        {
            Toast.ShowUserErrorToast("Sorry; you cannot throwaway the same card you picked up from the discard pile");
            return;
        }
        await gameContainer.SendDiscardMessageAsync(tempCard.Deck);
        await mainGame.DiscardAsync(tempCard.Deck);
    }
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
    public bool CanKnock => !mainGame.PlayerList.Any(items => items.Knocked == true) && !gameContainer.AlreadyDrew;
    [Command(EnumCommandCategory.Game)]
    public async Task KnockAsync()
    {
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("knock");
        }
        await mainGame.KnockAsync();
    }
}