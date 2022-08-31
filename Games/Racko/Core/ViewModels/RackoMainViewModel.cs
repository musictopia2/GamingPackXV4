namespace Racko.Core.ViewModels;
[InstanceGame]
public partial class RackoMainViewModel : BasicCardGamesVM<RackoCardInformation>
{
    private readonly RackoMainGameClass _mainGame;
    private readonly RackoVMData _model;
    private readonly RackoGameContainer _gameContainer;
    private readonly IToast _toast;
    public RackoMainViewModel(CommandContainer commandContainer,
        RackoMainGameClass mainGame,
        RackoVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        RackoGameContainer gameContainer,
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
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
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
    public bool CanPlayOnPile => _gameContainer!.AlreadyDrew;
    public async Task PlayOnPileAsync(RackoCardInformation card)
    {
        await _gameContainer!.SendDiscardMessageAsync(card.Deck);
        await _mainGame.DiscardAsync(card);
    }
    public bool CanDiscardCurrent => _gameContainer!.AlreadyDrew;
    [Command(EnumCommandCategory.Game)]
    public async Task DiscardCurrentAsync()
    {
        if (_model.OtherPile!.PileEmpty() == true)
        {
            _toast.ShowUserErrorToast("You must have a card to discard");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("discardcurrent");
        }
        await _mainGame!.DiscardCurrentAsync();
    }
    public bool CanRacko => !_gameContainer!.AlreadyDrew;
    [Command(EnumCommandCategory.Game)]
    public async Task RackoAsync()
    {
        if (_mainGame!.HasRacko() == false)
        {
            _toast.ShowUserErrorToast("There is no Racko");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("racko");
        }
        await _mainGame.EndRoundAsync();
    }
}