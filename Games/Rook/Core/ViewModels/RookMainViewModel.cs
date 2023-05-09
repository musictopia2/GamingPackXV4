namespace Rook.Core.ViewModels;
[InstanceGame]
public partial class RookMainViewModel : BasicCardGamesVM<RookCardInformation>
{
    private readonly RookMainGameClass _mainGame;
    public readonly RookVMData Model;
    private readonly IToast _toast;
    private readonly INestProcesses _nestProcesses;
    private readonly IBidProcesses _bidProcesses;
    private readonly ITrumpProcesses _trumpProcesses;
    public RookMainViewModel(CommandContainer commandContainer,
        RookMainGameClass mainGame,
        RookVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast,
        INestProcesses nestProcesses,
        IBidProcesses bidProcesses,
        ITrumpProcesses trumpProcesses
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        Model = viewModel;
        _toast = toast;
        _nestProcesses = nestProcesses;
        _bidProcesses = bidProcesses;
        _trumpProcesses = trumpProcesses;
        Model.Deck1.NeverAutoDisable = true;
        Model.Dummy1.SendEnableProcesses(this, () =>
        {
            if (_mainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
            {
                return false;
            }
            if (_mainGame.PlayerList.Count == 3)
            {
                return false;
            }
            return _mainGame.SaveRoot.DummyPlay;
        });
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public EnumStatusList GameStatus => _mainGame.SaveRoot.GameStatus;
    public override bool CanEndTurn()
    {
        return false;
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
    protected override bool AlwaysEnableHand()
    {
        return false;
    }
    protected override bool CanEnableHand()
    {
        if (_mainGame!.SaveRoot!.GameStatus == EnumStatusList.SelectNest)
        {
            return true;
        }
        if (_mainGame.SaveRoot.GameStatus == EnumStatusList.Normal)
        {
            return !_mainGame.SaveRoot.DummyPlay;
        }
        return false;
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task ChooseNestAsync()
    {
        var thisList = Model.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count != 5)
        {
            _toast.ShowUserErrorToast("Sorry, you must choose 5 cards to throw away");
            return;
        }
        await _nestProcesses.ProcessNestAsync(thisList);
    }
    public bool CanBid => Model.BidChosen > -1;
    [Command(EnumCommandCategory.Plain)]
    public async Task BidAsync()
    {
        await _bidProcesses.ProcessBidAsync();
    }
    public bool CanPass => Model.CanPass;
    [Command(EnumCommandCategory.Plain)]
    public async Task PassAsync()
    {
        await _bidProcesses.PassBidAsync();
    }
    public bool CanTrump => Model.ColorChosen != EnumColorTypes.None;
    [Command(EnumCommandCategory.Plain)]
    public async Task TrumpAsync()
    {
        await _trumpProcesses.ProcessTrumpAsync();
    }
}