namespace CrazyEights.Core.ViewModels;
[InstanceGame]
public partial class CrazyEightsMainViewModel : BasicCardGamesVM<RegularSimpleCard>, IHandle<ChooseSuitEventModel>
{
    private readonly CrazyEightsMainGameClass _mainGame;
    private readonly IGamePackageResolver _resolver;
    private readonly IToast _toast;
    private readonly CrazyEightsVMData _model;
    public CrazyEightsMainViewModel(CommandContainer commandContainer,
        CrazyEightsMainGameClass mainGame,
        CrazyEightsVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _resolver = resolver;
        _toast = toast;
        _model = viewModel;
        viewModel.Deck1.NeverAutoDisable = true;
    }
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        CheckSuitScreens();
    }
    protected override async Task TryCloseAsync()
    {
        if (SuitScreen is not null)
        {
            await CloseSpecificChildAsync(SuitScreen);
        }
        await base.TryCloseAsync();
    }
    protected override bool CanEnableDeck()
    {
        return SuitScreen == null;
    }
    public ChooseSuitViewModel? SuitScreen { get; set; }
    private async void CheckSuitScreens()
    {
        if (_model.ChooseSuit == true)
        {
            if (SuitScreen != null)
            {
                return;
            }
            SuitScreen = _resolver.Resolve<ChooseSuitViewModel>();
            await LoadScreenAsync(SuitScreen);
            return;
        }
        if (SuitScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(SuitScreen);
        SuitScreen = null;
    }
    protected override bool CanEnablePile1()
    {
        return SuitScreen == null;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int deck = _model.PlayerHand1!.ObjectSelected();
        if (deck == 0)
        {
            _toast.ShowUserErrorToast("You must select a card first");
            return;
        }
        if (_mainGame.IsValidMove(deck) == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            _model.PlayerHand1.UnselectAllObjects();
            return;
        }
        await _mainGame.PlayCardAsync(deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    void IHandle<ChooseSuitEventModel>.Handle(ChooseSuitEventModel message)
    {
        CheckSuitScreens();
    }
}