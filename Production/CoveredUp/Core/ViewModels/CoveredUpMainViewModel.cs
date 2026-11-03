namespace CoveredUp.Core.ViewModels;
[InstanceGame]
public class CoveredUpMainViewModel(CommandContainer commandContainer,
        CoveredUpMainGameClass mainGame,
        CoveredUpVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        CoveredUpGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<RegularSimpleCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    public CoveredUpVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        foreach (var player in mainGame.PlayerList)
        {
            player.LoadPlayerBoard(gameContainer, VMData);
            if (player.PlayerCategory == EnumPlayerCategory.Self)
            {
                SelfBoard = player.PlayerBoard!; //all others has to have their boards (even though displayed differently).
                SelfBoard.SendEnableProcesses(this, () =>
                {
                    return VMData.OtherPile!.PileEmpty() == false;
                });
            }
        }
        base.AdditionalInitialize();
    }
    public IEnumerable<CoveredUpPlayerItem> Players => gameContainer.PlayerList!;
    public PlayerBoardCP? SelfBoard { get; private set; }

    //anything else needed is here.
    //if i need something extra, will add to template as well.
    protected override bool CanEnableDeck()
    {
        return VMData.OtherPile!.PileEmpty();
    }
    protected override bool CanEnablePile1()
    {
        return true;
        //if (VMData.OtherPile!.PileEmpty() == false)
        //{
        //    return false;
        //}

        //return VMData.OtherPile!.PileEmpty();
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int oldDeck;
        if (VMData.OtherPile!.PileEmpty() == true)
        {
            oldDeck = 0;
        }
        else
        {
            oldDeck = VMData.OtherPile.GetCardInfo().Deck;
        }
        if (gameContainer.SaveRoot.WentOut && oldDeck == 0)
        {
            Toast.ShowUserErrorToast("Cannot pick up from discard anymore because somebody already went out");
            return;
        }
        if (oldDeck == 0)
        {
            await mainGame!.PickupFromDiscardAsync();
            return;
        }
        if (oldDeck == gameContainer!.PreviousCard)
        {
            Toast.ShowUserErrorToast("Cannot discard the same card that was picked up");
            return;
        }
        if (mainGame.BasicData.MultiPlayer == true)
        {
            await gameContainer.SendDiscardMessageAsync(oldDeck);
        }
        await mainGame.DiscardAsync(oldDeck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}