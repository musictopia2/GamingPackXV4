namespace Xactika.Core.ViewModels;
[InstanceGame]
public partial class XactikaBidViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly XactikaVMData Model;
    private readonly XactikaGameContainer _gameContainer;
    private readonly IBidProcesses _processes;
    public XactikaBidViewModel(CommandContainer commandContainer,
        XactikaVMData model,
        XactikaGameContainer gameContainer,
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
    public bool CanBid => Model.BidChosen > -1;
    [Command(EnumCommandCategory.Plain)]
    public async Task BidAsync()
    {
        if (_gameContainer.BasicData.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("bid", Model.BidChosen);
        }
        await _processes.ProcessBidAsync();
    }
}
