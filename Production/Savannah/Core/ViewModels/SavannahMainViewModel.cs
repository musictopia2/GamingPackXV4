namespace Savannah.Core.ViewModels;
[InstanceGame]
public partial class SavannahMainViewModel(CommandContainer commandContainer,
        SavannahMainGameClass mainGame,
        SavannahVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        SavannahGameContainer gameContainer,
        IToast toast) : BasicCardGamesVM<RegularSimpleCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    public SavannahVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        gameContainer.WrongDiscardProcess = () =>
        {
            Toast.ShowUserErrorToast("You are not allowed to discard jokers");
        };
        VMData.Deck1.NeverAutoDisable = true;
        VMData.PlayerHand1.Maximum = 4;
        CreateCommands(CommandContainer);
        VMData.PlayerHand1.BeforeAutoSelectObjectAsync = PlayerHand1_BeforeAutoSelectObjectAsync;
        VMData.SelfStock.StockClickedAsync = SelfStock_StockClickedAsync;
        VMData.PublicPiles.PileClickedAsync = PublicPiles_PileClickedAsync;
        base.AdditionalInitialize();
    }
    private async Task PublicPiles_PileClickedAsync(int index, BasicPileInfo<RegularSimpleCard> thisPile)
    {
        SendPlay plays = mainGame.CardSelected(Toast.ShowUserErrorToast);
        await Task.Delay(0);
        if (plays.Deck == 0)
        {
            return;
        }
        var card = gameContainer.DeckList.GetSpecificItem(plays.Deck);
        int rolled = VMData.Cup!.DiceList.Sum(x => x.Value);
        if (gameContainer.Test.AllowAnyMove == false)
        {
            if (VMData.PublicPiles.CanPlayOnPile(index, rolled, card) == false)
            {
                Toast.ShowUserErrorToast("Illegal Move");
                return;
            }
        }
        plays.Pile = index;
        if (gameContainer.BasicData.MultiPlayer)
        {
            await gameContainer.Network!.SendAllAsync(nameof(IMultiplayerModel.Play), plays);
        }
        await mainGame.PlayOnPileAsync(plays);
    }
    private async Task SelfStock_StockClickedAsync()
    {
        int nums = VMData.SelfStock.CardSelected();
        if (nums > 0)
        {
            VMData.SelfStock.UnselectCard();
            return;
        }
        await UnselectAllAsync();
        VMData.SelfStock.SelectCard();
    }
    private async Task PlayerHand1_BeforeAutoSelectObjectAsync()
    {
        if (VMData.PlayerHand1.HasSelectedObject())
        {
            return;
        }
        await UnselectAllAsync();
    }
    private async Task UnselectAllAsync()
    {
        if (gameContainer.UnselectAllPilesAsync is null)
        {
            throw new CustomBasicException("Nobody is handling the UnselectAllPilesAsync");
        }
        await gameContainer.UnselectAllPilesAsync.Invoke(); //hopefully nothing else because the proper one should be done automatically here.
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanClickPlayerDiscard => CanEnablePlayer;
    [Command(EnumCommandCategory.Game)]
    public async Task ClickPlayerDiscardAsync(SavannahPlayerItem player)
    {
        if (player is null)
        {
            throw new CustomBasicException("There was no player");
        }
        if (player.DiscardList.Count == 0)
        {
            return; //because they have no cards left.  just in case.
        }
        if (player.DiscardList.Last().IsSelected)
        {
            player.DiscardList.Last().IsSelected = false;
            return;
        }
        await UnselectAllAsync();
        player.DiscardList.Last().IsSelected = true; //i think.  this is all that is needed.  because you can choose the card.  but still has to choose what public pile to play on.
    }
    protected override bool CanEnableDeck()
    {
        return false; //otherwise, can't compile.
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    protected override Task ProcessDiscardClickedAsync()
    {
        throw new CustomBasicException("You cannot click on discard anymore");
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public bool CanEnablePlayer
    {
        get
        {
            if (mainGame.SaveRoot.ChoseOtherPlayer)
            {
                return false;
            }
            if (VMData.Cup is null)
            {
                return false;
            }
            if (VMData.Cup.DiceList.Count == 0)
            {
                return false;
            }
            if (mainGame.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
            {
                return false; //because only self can play obviously.
            }
            return VMData.Cup!.DiceList.First().Value == VMData.Cup.DiceList.Last().Value;
        }
    }
}