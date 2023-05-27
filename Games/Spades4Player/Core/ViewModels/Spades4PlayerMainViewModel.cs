namespace Spades4Player.Core.ViewModels;
[InstanceGame]
public partial class Spades4PlayerMainViewModel : BasicCardGamesVM<Spades4PlayerCardInformation>
{
    private readonly Spades4PlayerMainGameClass _mainGame; //if we don't need, delete.
    private readonly Spades4PlayerGameContainer _gameContainer; //if not needed, delete.
    public Spades4PlayerMainViewModel(CommandContainer commandContainer,
        Spades4PlayerMainGameClass mainGame,
        Spades4PlayerVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        Spades4PlayerGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _gameContainer = gameContainer;
        Model = viewModel;
        Model.Deck1.NeverAutoDisable = true;
        CreateCommands(commandContainer);
    }
    //anything else needed is here.
    public Spades4PlayerVMData Model { get; private set; }
    partial void CreateCommands(CommandContainer command);
    public EnumGameStatus GameStatus => _gameContainer.SaveRoot.GameStatus;
    protected override bool CanEnableDeck()
    {
        //todo:  decide whether to enable deck.
        return false; //otherwise, can't compile.
    }

    protected override bool CanEnablePile1()
    {
        //todo:  decide whether to enable deck.
        return false; //otherwise, can't compile.
    }

    protected override async Task ProcessDiscardClickedAsync()
    {
        //if we have anything, will be here.
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task BidAsync()
    {
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("bid", Model.BidAmount);
        }
        await _mainGame.ProcessBidAsync();
    }
}