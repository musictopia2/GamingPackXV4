namespace HitTheDeck.Core.ViewModels;
[InstanceGame]
public partial class HitTheDeckMainViewModel(CommandContainer commandContainer,
        HitTheDeckMainGameClass mainGame,
        HitTheDeckVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        HitTheDeckGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<HitTheDeckCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
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
        if (NeedsSpecial == true)
        {
            return false;
        }
        if (mainGame!.SingleInfo!.MainHandList.Any(items => mainGame.CanPlay(items.Deck)) && mainGame.Test!.AllowAnyMove == false)
        {
            return false;
        }
        return !gameContainer.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return !NeedsSpecial;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int newDeck = viewModel.PlayerHand1!.ObjectSelected();
        if (newDeck == 0)
        {
            Toast.ShowUserErrorToast("Sorry, must select a card first");
            return;
        }
        if (mainGame!.CanPlay(newDeck) == false)
        {
            Toast.ShowUserErrorToast("Illegal move");
            return;
        }
        await mainGame.ProcessPlayAsync(newDeck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        var thisCard = viewModel.Pile1!.GetCardInfo();
        if (thisCard.Instructions == EnumInstructionList.Flip || thisCard.Instructions == EnumInstructionList.Cut)
        {
            return false;
        }
        if (mainGame!.SingleInfo!.MainHandList.Any(items => mainGame.CanPlay(items.Deck)))
        {
            return false;
        }
        return gameContainer.AlreadyDrew;
    }
    private bool NeedsSpecial
    {
        get
        {
            var thisCard = viewModel.Pile1!.GetCardInfo();
            return thisCard.Instructions == EnumInstructionList.Cut || thisCard.Instructions == EnumInstructionList.Flip;
        }
    }
    public bool CanCut()
    {
        var thisCard = viewModel.Pile1!.GetCardInfo();
        return thisCard.Instructions == EnumInstructionList.Cut;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task CutAsync()
    {
        await mainGame!.CutDeckAsync();
    }
    public bool CanFlip()
    {
        var thisCard = viewModel.Pile1!.GetCardInfo();
        return thisCard.Instructions == EnumInstructionList.Flip;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task FlipAsync()
    {
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("flipdeck");
        }
        await mainGame!.FlipDeckAsync();
    }
}