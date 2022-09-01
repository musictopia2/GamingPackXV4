namespace Rook.Core.ViewModels;
[InstanceGame]
public partial class RookBiddingViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly RookVMData Model;
    private readonly IBidProcesses _processes;
    public RookBiddingViewModel(CommandContainer commandContainer,
        RookVMData model,
        IBidProcesses processes,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        Model = model;
        _processes = processes;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    public bool CanBid => Model.BidChosen > -1;
    [Command(EnumCommandCategory.Plain)]
    public async Task BidAsync()
    {
        await _processes.ProcessBidAsync();
    }
    public bool CanPass => Model.CanPass;
    [Command(EnumCommandCategory.Plain)]
    public async Task PassAsync()
    {
        await _processes.PassBidAsync();
    }
}