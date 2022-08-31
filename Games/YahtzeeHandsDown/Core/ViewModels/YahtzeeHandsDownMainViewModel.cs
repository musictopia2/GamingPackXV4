namespace YahtzeeHandsDown.Core.ViewModels;
[InstanceGame]
public partial class YahtzeeHandsDownMainViewModel : BasicCardGamesVM<YahtzeeHandsDownCardInformation>
{
    private readonly YahtzeeHandsDownMainGameClass _mainGame;
    private readonly YahtzeeHandsDownVMData _model;
    private readonly YahtzeeHandsDownGameContainer _gameContainer;
    private readonly IToast _toast;
    public YahtzeeHandsDownMainViewModel(CommandContainer commandContainer,
        YahtzeeHandsDownMainGameClass mainGame,
        YahtzeeHandsDownVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        YahtzeeHandsDownGameContainer gameContainer,
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
        _model.PlayerHand1.Maximum = 5;
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        if (_mainGame!.PlayerList.Count > 2)
        {
            return !_gameContainer.AlreadyDrew;
        }
        if (_mainGame.SaveRoot!.ExtraTurns < 4)
        {
            return !_gameContainer.AlreadyDrew;
        }
        return false;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        var thisList = _model.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count == 0)
        {
            _toast.ShowUserErrorToast("Must choose at least one card to discard to get new cards");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            var nextList = thisList.GetDeckListFromObjectList();
            await _mainGame.Network!.SendAllAsync("replacecards", nextList);
        }
        await _mainGame!.ReplaceCardsAsync(thisList);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        return !CanEnablePile1();
    }
    [Command(EnumCommandCategory.Game)]
    public async Task GoOutAsync()
    {
        var results = _mainGame!.GetResults();
        if (results.Count == 0)
        {
            _toast.ShowUserErrorToast("Cannot go out");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("wentout", results.First());
        }
        await _mainGame.PlayerGoesOutAsync(results.First());
    }
}