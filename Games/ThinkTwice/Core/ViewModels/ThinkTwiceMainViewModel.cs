namespace ThinkTwice.Core.ViewModels;
[InstanceGame]
public partial class ThinkTwiceMainViewModel(CommandContainer commandContainer,
        ThinkTwiceMainGameClass mainGame,
        ThinkTwiceVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IStandardRollProcesses roller,
        IEventAggregator aggregator) : DiceGamesVM<SimpleDice>(commandContainer, mainGame, viewModel, basicData, resolver, roller, aggregator)
{
    public ThinkTwiceVMData VMData { get; set; } = viewModel;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    protected override async Task OpenAsync()
    {
        ScoreScreen = Resolver.Resolve<ScoreViewModel>();
        await base.OpenAsync();
    }
    partial void CreateCommands(CommandContainer command);
    public DiceCup<SimpleDice> GetCup => VMData.Cup!;
    public PlayerCollection<ThinkTwicePlayerItem> PlayerList => mainGame.PlayerList;
    public ScoreViewModel? ScoreScreen { get; set; }
    protected override Task CloseAsync()
    {
        ScoreScreen = null;
        return base.CloseAsync();
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
        if (mainGame.BasicData.MultiPlayer) //so it can send the scores to the other players.
        {
            await mainGame.SendSelectedAsync();
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
            return mainGame.SaveRoot.WhichMulti == -1;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollMultAsync()
    {
        await mainGame.RollMultsAsync();
    }
}