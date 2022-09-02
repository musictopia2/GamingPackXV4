namespace RageCardGame.Core.ViewModels;
[InstanceGame]
public partial class RageBiddingViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly RageCardGameVMData Model;
    private readonly IBidProcesses _processes;
    public RageBiddingViewModel(CommandContainer commandContainer, RageCardGameVMData model, IBidProcesses processes, RageCardGameGameContainer gameContainer, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        Model = model;
        _processes = processes;
        GameContainer = gameContainer;
        NormalTurn = Model.NormalTurn;
        TrumpSuit = Model.TrumpSuit;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    public RageCardGameGameContainer GameContainer { get; }
    public bool CanBid => Model.BidAmount > -1;
    [Command(EnumCommandCategory.Plain)]
    public async Task BidAsync()
    {
        await _processes.ProcessBidAsync();
    }
    public string NormalTurn { get; set; } = "";
    public EnumColor TrumpSuit { get; set; }
}
