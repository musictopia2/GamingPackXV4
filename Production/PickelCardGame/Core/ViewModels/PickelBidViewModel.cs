namespace PickelCardGame.Core.ViewModels;
[InstanceGame]
public partial class PickelBidViewModel(CommandContainer commandContainer,
        PickelCardGameVMData model,
        PickelCardGameGameContainer gameContainer,
        IBidProcesses processes) : IPromptVM
{
    public readonly PickelCardGameVMData Model = model;
    partial void CreateCommands(CommandContainer command);
    private void Suit1_ItemSelectionChanged(EnumSuitList piece)
    {
        gameContainer!.SaveRoot!.TrumpSuit = piece;
    }
    private Task Bid1_ChangedNumberValueAsync(int chosen)
    {
        Model.BidAmount = chosen;
        return Task.CompletedTask;
    }
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
        await processes.ProcessBidAsync();
    }
    public bool CanPass => processes.CanPass();
    [Command(EnumCommandCategory.Plain)]
    public async Task PassAsync()
    {
        await processes.PassBidAsync();
    }

    void IPromptVM.Initialize()
    {
        Model.Bid1.ChangedNumberValueAsync = Bid1_ChangedNumberValueAsync;
        Model.Suit1.ItemSelectionChanged = Suit1_ItemSelectionChanged;
        CreateCommands(commandContainer);
    }
}
