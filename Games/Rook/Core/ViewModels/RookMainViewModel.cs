namespace Rook.Core.ViewModels;
[InstanceGame]
public partial class RookMainViewModel(CommandContainer commandContainer,
        RookMainGameClass mainGame,
        RookVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast,
        INestProcesses nestProcesses,
        IBidProcesses bidProcesses,
        ITrumpProcesses trumpProcesses) : BasicCardGamesVM<RookCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    public readonly RookVMData Model = viewModel;
    protected override void AdditionalInitialize()
    {
        CreateCommands(CommandContainer);
        Model.Deck1.NeverAutoDisable = true;
        Model.Dummy1.SendEnableProcesses(this, () =>
        {
            if (mainGame!.SaveRoot!.GameStatus != EnumStatusList.Normal)
            {
                return false;
            }
            if (mainGame.PlayerList.Count == 3)
            {
                return false;
            }
            return mainGame.SaveRoot.DummyPlay;
        });
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public override bool CanEndTurn()
    {
        return false;
    }
    protected override bool CanEnableDeck()
    {
        return false;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    protected override bool AlwaysEnableHand()
    {
        return false;
    }
    protected override bool CanEnableHand()
    {
        if (mainGame!.SaveRoot!.GameStatus == EnumStatusList.SelectNest)
        {
            return true;
        }
        if (mainGame.SaveRoot.GameStatus == EnumStatusList.Normal)
        {
            return !mainGame.SaveRoot.DummyPlay;
        }
        return false;
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task ChooseNestAsync()
    {
        var thisList = Model.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count != 5)
        {
            Toast.ShowUserErrorToast("Sorry, you must choose 5 cards to throw away");
            return;
        }
        await nestProcesses.ProcessNestAsync(thisList);
    }
    public bool CanBid => Model.BidChosen > -1;
    [Command(EnumCommandCategory.Plain)]
    public async Task BidAsync()
    {
        await bidProcesses.ProcessBidAsync();
    }
    public bool CanPass => Model.CanPass;
    [Command(EnumCommandCategory.Plain)]
    public async Task PassAsync()
    {
        await bidProcesses.PassBidAsync();
    }
    public bool CanTrump => Model.ColorChosen != EnumColorTypes.None;
    [Command(EnumCommandCategory.Plain)]
    public async Task TrumpAsync()
    {
        await trumpProcesses.ProcessTrumpAsync();
    }
}