namespace SkuckCardGame.Core.ViewModels;
[InstanceGame]
public partial class SkuckBiddingViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly SkuckCardGameVMData Model;
    private readonly SkuckCardGameGameContainer _gameContainer;
    private readonly IBidProcesses _processes;
    public SkuckBiddingViewModel(CommandContainer commandContainer,
        SkuckCardGameVMData model,
        SkuckCardGameGameContainer gameContainer,
        IBidProcesses processes,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        Model = model;
        _gameContainer = gameContainer;
        _processes = processes;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public CommandContainer CommandContainer { get; set; }
    public bool CanBid => Model.BidAmount > -1;
    [Command(EnumCommandCategory.Plain)]
    public async Task BidAsync()
    {
        if (_gameContainer.BasicData!.MultiPlayer == true)
        {
            await _gameContainer.Network!.SendAllAsync("bid", Model.BidAmount);
        }
        await _processes.ProcessBidAmountAsync();
    }
}