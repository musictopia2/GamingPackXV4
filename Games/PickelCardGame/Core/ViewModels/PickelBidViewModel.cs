namespace PickelCardGame.Core.ViewModels;
[InstanceGame]
public partial class PickelBidViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly PickelCardGameVMData Model;
    private readonly PickelCardGameGameContainer _gameContainer;
    private readonly IBidProcesses _processes;
    public PickelBidViewModel(
        CommandContainer commandContainer,
        PickelCardGameVMData model,
        PickelCardGameGameContainer gameContainer,
        IBidProcesses processes,
        IEventAggregator aggregator
        ) : base(aggregator)
    {
        CommandContainer = commandContainer;
        Model = model;
        _gameContainer = gameContainer;
        _processes = processes;
        Model.Bid1.ChangedNumberValueAsync = Bid1_ChangedNumberValueAsync;
        Model.Suit1.ItemSelectionChanged = Suit1_ItemSelectionChanged;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private void Suit1_ItemSelectionChanged(EnumSuitList piece)
    {
        _gameContainer!.SaveRoot!.TrumpSuit = piece;
    }
    private Task Bid1_ChangedNumberValueAsync(int chosen)
    {
        Model.BidAmount = chosen;
        return Task.CompletedTask;
    }
    public CommandContainer CommandContainer { get; set; }
    public bool CanProcessBid()
    {
        if (Model.BidAmount == -1 || Model.TrumpSuit == EnumSuitList.None)
        {
            return false;
        }
        return true;
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task ProcessBidAsync()
    {
        await _processes.ProcessBidAsync();
    }
    public bool CanPass => _processes.CanPass();
    [Command(EnumCommandCategory.Plain)]
    public async Task PassAsync()
    {
        await _processes.PassBidAsync();
    }
}
