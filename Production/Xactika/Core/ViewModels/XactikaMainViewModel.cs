namespace Xactika.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class XactikaMainViewModel(CommandContainer commandContainer,
        XactikaMainGameClass mainGame,
        XactikaVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        XactikaGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<XactikaCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        gameContainer.LoadBidding = LoadBidding;
        gameContainer.CloseBidding = CloseBidding;
        gameContainer.LoadShapeButton = LoadShape;
        gameContainer.CloseShapeButton = CloseShape;
        base.AdditionalInitialize();
    }
    protected override async Task CloseAsync()
    {
        CloseBidding();
        CloseShape();
        await base.CloseAsync();
    }
    public void LoadShape()
    {
        if (ShapeScreen != null)
        {
            return;
        }
        ShapeScreen = Resolver.Resolve<XactikaSubmitShapeViewModel>();
    }
    public void CloseShape()
    {
        if (ShapeScreen == null)
        {
            return;
        }
        ShapeScreen = null;
    }
    [StateProperty]
    public partial XactikaSubmitShapeViewModel? ShapeScreen { get; set; }
    private void LoadBidding()
    {
        if (BidScreen != null)
        {
            return;
        }
        viewModel.TrickArea1.Visible = false;
        viewModel.ShapeChoose1.Visible = false;
        BidScreen = Resolver.Resolve<XactikaBidViewModel>();
    }
    private void CloseBidding()
    {
        if (BidScreen == null)
        {
            return;
        }
        BidScreen = null;
        viewModel.TrickArea1.Visible = true;
        
    }
    [StateProperty]
    public partial XactikaBidViewModel? BidScreen { get; set; }
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
}