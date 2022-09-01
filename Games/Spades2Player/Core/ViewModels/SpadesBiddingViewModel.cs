namespace Spades2Player.Core.ViewModels;
[InstanceGame]
public partial class SpadesBiddingViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly Spades2PlayerVMData Model;
    private readonly Spades2PlayerMainGameClass _mainGame;
    public SpadesBiddingViewModel(CommandContainer commandContainer, Spades2PlayerVMData model, Spades2PlayerMainGameClass mainGame, IEventAggregator aggregator) : base(aggregator)
    {
        CommandContainer = commandContainer;
        Model = model;
        _mainGame = mainGame;
        Model.Bid1.ChangedNumberValueAsync += Bid1_ChangedNumberValueAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override Task TryCloseAsync()
    {
        Model.Bid1.ChangedNumberValueAsync -= Bid1_ChangedNumberValueAsync;
        return base.TryCloseAsync();
    }
    public CommandContainer CommandContainer { get; set; }
    private Task Bid1_ChangedNumberValueAsync(int chosen)
    {
        Model.BidAmount = chosen;
        return Task.CompletedTask;
    }
    public bool CanBid => Model.BidAmount > -1;
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
