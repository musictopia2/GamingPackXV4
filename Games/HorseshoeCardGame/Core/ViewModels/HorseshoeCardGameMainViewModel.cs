namespace HorseshoeCardGame.Core.ViewModels;
[InstanceGame]
public class HorseshoeCardGameMainViewModel(CommandContainer commandContainer,
        HorseshoeCardGameMainGameClass mainGame,
        HorseshoeCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<HorseshoeCardGameCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        LoadPlayerControls();
        base.AdditionalInitialize();
    }
    private void LoadPlayerControls()
    {
        mainGame!.PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.TempHand == null)
            {
                throw new CustomBasicException("TempHand was never created.  Rethink");
            }
            thisPlayer.TempHand.SendEnableProcesses(this, () =>
            {
                if (thisPlayer.PlayerCategory != EnumPlayerCategory.Self)
                {
                    return false;
                }
                return true;
            });
            if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisPlayer.TempHand.SelectedCard = TempHand_SelectedCard;
            }
        });
    }
    private void TempHand_SelectedCard()
    {
        viewModel.PlayerHand1!.UnselectAllObjects();
    }
    protected override Task OnAutoSelectedHandAsync()
    {
        HorseshoeCardGamePlayerItem thisPlayer = mainGame!.PlayerList!.GetSelf();
        thisPlayer.TempHand!.UnselectAllCards();
        return base.OnAutoSelectedHandAsync();
    }
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
}