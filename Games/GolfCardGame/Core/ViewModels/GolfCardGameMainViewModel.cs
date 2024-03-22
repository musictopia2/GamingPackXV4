namespace GolfCardGame.Core.ViewModels;
[InstanceGame]
public partial class GolfCardGameMainViewModel : BasicCardGamesVM<RegularSimpleCard>
{
    private readonly GolfCardGameMainGameClass _mainGame;
    private readonly GolfCardGameVMData _model;
    private readonly GolfCardGameGameContainer _gameContainer;
    private readonly IToast _toast;
    public GolfCardGameMainViewModel(CommandContainer commandContainer,
        GolfCardGameMainGameClass mainGame,
        GolfCardGameVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        GolfCardGameGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _gameContainer = gameContainer;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        _model.OtherPile!.PileClickedAsync = OtherPile_PileClickedAsync;
        _model.OtherPile.SendEnableProcesses(this, () => gameContainer.AlreadyDrew);
        _model.GolfHand1.SendEnableProcesses(this, () => gameContainer.AlreadyDrew);
        _model.HiddenCards1.SendEnableProcesses(this, () => gameContainer.AlreadyDrew);
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer commandContainer);
    private async Task OtherPile_PileClickedAsync()
    {
        var tempCard = _model.OtherPile!.GetCardInfo();
        if (_gameContainer!.PreviousCard == tempCard.Deck)
        {
            _toast.ShowUserErrorToast("Sorry; you cannot throwaway the same card you picked up from the discard pile");
            return;
        }
        await _gameContainer.SendDiscardMessageAsync(tempCard.Deck);
        await _mainGame.DiscardAsync(tempCard.Deck);
    }
    protected override bool CanEnableDeck()
    {
        return !_gameContainer!.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return !_gameContainer!.AlreadyDrew;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await _mainGame!.PickupFromDiscardAsync();
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public bool CanKnock => !_mainGame.PlayerList.Any(items => items.Knocked == true) && !_gameContainer.AlreadyDrew;
    [Command(EnumCommandCategory.Game)]
    public async Task KnockAsync()
    {
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("knock");
        }
        await _mainGame.KnockAsync();
    }
}