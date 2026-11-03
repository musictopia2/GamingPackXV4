namespace BowlingDiceGame.Core.ViewModels;
[InstanceGame]
public partial class BowlingDiceGameMainViewModel(CommandContainer commandContainer,
        BowlingDiceGameMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BowlingDiceGameVMData data) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public readonly BowlingDiceGameMainGameClass MainGame = mainGame;
    public BowlingDiceGameVMData VMData { get; set; } = data;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        CommandContainer.AddCommand(RollCommand!);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    private bool _lastTurn;
    public override bool CanEndTurn()
    {
        if (base.CanEndTurn() == false)
        {
            return false;
        }
        if (VMData.WhichPart < 3)
        {
            return false;
        }
        if (_lastTurn == true)
        {
            return false;
        }
        return !VMData.IsExtended;
    }
    public bool CanContinueTurn => VMData.IsExtended;
    [Command(EnumCommandCategory.Game)]
    public async Task ContinueTurnAsync()
    {
        MainGame.SaveRoot.IsExtended = false;
        _lastTurn = true;
        await Task.Delay(10);
    }
    public bool CanRoll
    {
        get
        {
            if (VMData.WhichPart < 3)
            {
                return true;
            }
            return _lastTurn;
        }
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollAsync()
    {
        _lastTurn = false;
        await MainGame.RollDiceAsync();
    }
}