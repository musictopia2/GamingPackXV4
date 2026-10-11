namespace SnagCardGame.Core.ViewModels;
[InstanceGame]
public class SnagCardGameMainViewModel(CommandContainer commandContainer,
        SnagCardGameMainGameClass mainGame,
        SnagCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<SnagCardGameCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast),
    ITrickDummyHand<EnumSuitList, SnagCardGameCardInformation>
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.Bar1.SendEnableProcesses(this, () =>
        {
            if (mainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
            {
                return false;
            }
            return !mainGame.SaveRoot.FirstCardPlayed;
        });
        base.AdditionalInitialize();
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
    protected override bool AlwaysEnableHand()
    {
        return false;
    }
    protected override bool CanEnableHand()
    {
        if (mainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
        {
            return false;
        }
        return mainGame.SaveRoot.FirstCardPlayed;
    }
    public DeckRegularDict<SnagCardGameCardInformation> GetCurrentHandList()
    {
        if (mainGame!.SaveRoot!.FirstCardPlayed == false)
        {
            return viewModel.Bar1!.PossibleList.ToRegularDeckDict();
        }
        return mainGame.SingleInfo!.MainHandList.ToRegularDeckDict();
    }
    public int CardSelected()
    {
        if (mainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Only self should know what is selected");
        }
        if (mainGame.SaveRoot!.FirstCardPlayed == false)
        {
            return viewModel.Bar1!.ObjectSelected();
        }
        return viewModel.PlayerHand1!.ObjectSelected();
    }
    public void RemoveCard(int deck)
    {
        if (mainGame!.SaveRoot!.FirstCardPlayed == false)
        {
            viewModel.Bar1!.HandList.RemoveObjectByDeck(deck);
        }
        else
        {
            mainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
        }
    }
}