namespace A8RoundRummy.Core.ViewModels;
[InstanceGame]
public partial class A8RoundRummyMainViewModel : BasicCardGamesVM<A8RoundRummyCardInformation>
{
    private readonly A8RoundRummyMainGameClass _mainGame;
    private readonly A8RoundRummyVMData _model;
    private readonly A8RoundRummyGameContainer _gameContainer;
    private readonly IToast _toast;
    public A8RoundRummyMainViewModel(CommandContainer commandContainer,
        A8RoundRummyMainGameClass mainGame,
        A8RoundRummyVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        A8RoundRummyGameContainer gameContainer,
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
        return true;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        if (_mainGame!.CanProcessDiscard(out bool pickUp, out int deck, out string message) == false)
        {
            _toast.ShowUserErrorToast(message);
            return;
        }
        if (pickUp == true)
        {
            await _mainGame.PickupFromDiscardAsync();
            return;
        }
        await _gameContainer.SendDiscardMessageAsync(deck);
        await _mainGame.DiscardAsync(deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public bool CanGoOut => _model.PlayerHand1.HandList.Count == 8; //if 8, you can go out since you have to draw first.
    [Command(EnumCommandCategory.Game)]
    public async Task GoOutAsync()
    {
        if (_mainGame!.CanGoOut() == false)
        {
            _toast.ShowUserErrorToast("Sorry; you cannot go out");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            MultiplayerOut thisOut = new();
            thisOut.Deck = _mainGame.CardForDiscard!.Deck;
            thisOut.WasGuaranteed = _mainGame.WasGuarantee;
            await _mainGame.Network!.SendAllAsync("goout", thisOut);
        }
        await _mainGame.GoOutAsync();
    }
}