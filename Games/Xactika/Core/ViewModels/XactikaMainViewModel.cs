namespace Xactika.Core.ViewModels;
[InstanceGame]
public class XactikaMainViewModel : BasicCardGamesVM<XactikaCardInformation>
{
    private readonly XactikaVMData _model;
    private readonly IGamePackageResolver _resolver;
    private readonly XactikaGameContainer _gameContainer;
    public XactikaMainViewModel(CommandContainer commandContainer,
        XactikaMainGameClass mainGame,
        XactikaVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        XactikaGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _model = viewModel;
        _resolver = resolver;
        _gameContainer = gameContainer;
        _model.Deck1.NeverAutoDisable = true;
        _gameContainer.LoadBiddingAsync = LoadBiddingAsync;
        _gameContainer.CloseBiddingAsync = CloseBiddingAsync;
        _gameContainer.LoadShapeButtonAsync = LoadShapeAsync;
        _gameContainer.CloseShapeButtonAsync = CloseShapeAsync;
    }
    protected override async Task TryCloseAsync()
    {
        await CloseBiddingAsync();
        await CloseShapeAsync();
        await base.TryCloseAsync();
    }
    public async Task LoadShapeAsync()
    {
        if (ShapeScreen != null)
        {
            return;
        }
        ShapeScreen = _resolver.Resolve<XactikaSubmitShapeViewModel>();
        await LoadScreenAsync(ShapeScreen);
    }
    public async Task CloseShapeAsync()
    {
        if (ShapeScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(ShapeScreen);
        ShapeScreen = null;
    }
    public XactikaSubmitShapeViewModel? ShapeScreen { get; set; }
    private async Task LoadBiddingAsync()
    {
        if (BidScreen != null)
        {
            return;
        }
        _model.TrickArea1.Visible = false;
        _model.ShapeChoose1.Visible = false;
        BidScreen = _resolver.Resolve<XactikaBidViewModel>();
        await LoadScreenAsync(BidScreen);
    }
    private async Task CloseBiddingAsync()
    {
        if (BidScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(BidScreen);
        BidScreen = null;
        _model.TrickArea1.Visible = true;
    }
    public XactikaBidViewModel? BidScreen { get; set; }
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