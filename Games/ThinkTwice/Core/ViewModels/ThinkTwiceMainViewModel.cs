namespace ThinkTwice.Core.ViewModels;
[InstanceGame]
public partial class ThinkTwiceMainViewModel : DiceGamesVM<SimpleDice>
{
    private readonly ThinkTwiceMainGameClass _mainGame; //if we don't need, delete.
    public ThinkTwiceVMData VMData { get; set; }
    private readonly IGamePackageResolver _resolver;
    public ThinkTwiceMainViewModel(CommandContainer commandContainer,
        ThinkTwiceMainGameClass mainGame,
        ThinkTwiceVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, roller, aggregator)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _resolver = resolver;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<ThinkTwicePlayerItem> PlayerList => _mainGame.PlayerList;
    public ScoreViewModel? ScoreScreen { get; set; }
    protected override async Task ActivateAsync()
    {
        ScoreScreen = _resolver.Resolve<ScoreViewModel>();
        await LoadScreenAsync(ScoreScreen);
        await base.ActivateAsync();
    }
    protected override async Task TryCloseAsync()
    {
        if (ScoreScreen is not null)
        {
            await CloseSpecificChildAsync(ScoreScreen);
        }
        await base.TryCloseAsync();
    }
    protected override bool CanEnableDice()
    {
        return CanRollDice();
    }
    public override bool CanEndTurn()
    {
        return VMData.ItemSelected > -1;
    }
    public override async Task EndTurnAsync()
    {
        if (_mainGame.BasicData.MultiPlayer) //so it can send the scores to the other players.
        {
            await _mainGame.SendSelectedAsync();
        }
        await base.EndTurnAsync();
    }
    public override bool CanRollDice()
    {
        return VMData.RollNumber <= 3; //has to increase by one this time though.
    }
    public bool CanRollMult
    {
        get
        {
            if (VMData.RollNumber == 1)
            {
                return false;
            }
            return _mainGame.SaveRoot.WhichMulti == -1;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollMultAsync()
    {
        await _mainGame.RollMultsAsync();
    }
}