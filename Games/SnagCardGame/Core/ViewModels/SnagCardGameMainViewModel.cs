namespace SnagCardGame.Core.ViewModels;
[InstanceGame]
public class SnagCardGameMainViewModel : BasicCardGamesVM<SnagCardGameCardInformation>,
    ITrickDummyHand<EnumSuitList, SnagCardGameCardInformation>
{
    private readonly SnagCardGameMainGameClass _mainGame;
    private readonly SnagCardGameVMData _model;
    public SnagCardGameMainViewModel(CommandContainer commandContainer,
        SnagCardGameMainGameClass mainGame,
        SnagCardGameVMData viewModel,
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
        _model.Bar1.SendEnableProcesses(this, () =>
        {
            if (_mainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
            {
                return false;
            }
            return !_mainGame.SaveRoot.FirstCardPlayed;
        });
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
        if (_mainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
        {
            return false;
        }
        return _mainGame.SaveRoot.FirstCardPlayed;
    }
    public DeckRegularDict<SnagCardGameCardInformation> GetCurrentHandList()
    {
        if (_mainGame!.SaveRoot!.FirstCardPlayed == false)
        {
            return _model.Bar1!.PossibleList.ToRegularDeckDict();
        }
        return _mainGame.SingleInfo!.MainHandList.ToRegularDeckDict();
    }
    public int CardSelected()
    {
        if (_mainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Only self should know what is selected");
        }
        if (_mainGame.SaveRoot!.FirstCardPlayed == false)
        {
            return _model.Bar1!.ObjectSelected();
        }
        return _model.PlayerHand1!.ObjectSelected();
    }
    public void RemoveCard(int deck)
    {
        if (_mainGame!.SaveRoot!.FirstCardPlayed == false)
        {
            _model.Bar1!.HandList.RemoveObjectByDeck(deck);
        }
        else
        {
            _mainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
        }
    }
}