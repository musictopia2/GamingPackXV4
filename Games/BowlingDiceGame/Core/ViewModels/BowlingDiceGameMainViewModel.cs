namespace BowlingDiceGame.Core.ViewModels;
[InstanceGame]
public partial class BowlingDiceGameMainViewModel : BasicMultiplayerMainVM
{
    public readonly BowlingDiceGameMainGameClass MainGame; //if we don't need, delete.
    public BowlingDiceGameVMData VMData { get; set; }
    public BowlingDiceGameMainViewModel(CommandContainer commandContainer,
        BowlingDiceGameMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BowlingDiceGameVMData data
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        MainGame = mainGame;
        VMData = data;
        CreateCommands(commandContainer);
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
        CommandContainer.ManualReport(); //try this.
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