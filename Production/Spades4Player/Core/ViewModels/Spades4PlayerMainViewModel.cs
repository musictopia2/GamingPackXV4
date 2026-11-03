namespace Spades4Player.Core.ViewModels;
[InstanceGame]
public partial class Spades4PlayerMainViewModel(CommandContainer commandContainer,
        Spades4PlayerMainGameClass mainGame,
        Spades4PlayerVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<Spades4PlayerCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        Model.Deck1.NeverAutoDisable = true;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    //anything else needed is here.
    public Spades4PlayerVMData Model { get; private set; } = viewModel;
    partial void CreateCommands(CommandContainer command);
    protected override bool CanEnableDeck()
    {
        //todo:  decide whether to enable deck.
        return false; //otherwise, can't compile.
    }

    protected override bool CanEnablePile1()
    {
        //todo:  decide whether to enable deck.
        return false; //otherwise, can't compile.
    }

    protected override async Task ProcessDiscardClickedAsync()
    {
        //if we have anything, will be here.
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task BidAsync()
    {
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("bid", Model.BidAmount);
        }
        await mainGame.ProcessBidAsync();
    }
}