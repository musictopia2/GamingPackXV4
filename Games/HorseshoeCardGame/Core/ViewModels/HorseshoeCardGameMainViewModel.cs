namespace HorseshoeCardGame.Core.ViewModels;
[InstanceGame]
public class HorseshoeCardGameMainViewModel : BasicCardGamesVM<HorseshoeCardGameCardInformation>
{
    private readonly HorseshoeCardGameMainGameClass _mainGame; //if we don't need, delete.
    private readonly HorseshoeCardGameVMData _model;
    public HorseshoeCardGameMainViewModel(CommandContainer commandContainer,
        HorseshoeCardGameMainGameClass mainGame,
        HorseshoeCardGameVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _model.Deck1.NeverAutoDisable = true;
        LoadPlayerControls();
    }
    private void LoadPlayerControls()
    {
        _mainGame!.PlayerList!.ForEach(thisPlayer =>
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
        _model.PlayerHand1!.UnselectAllObjects();
    }
    protected override Task OnAutoSelectedHandAsync()
    {
        HorseshoeCardGamePlayerItem thisPlayer = _mainGame!.PlayerList!.GetSelf();
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