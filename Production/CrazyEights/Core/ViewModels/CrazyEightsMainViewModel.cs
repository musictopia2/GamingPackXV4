namespace CrazyEights.Core.ViewModels;
[InstanceGame]
[EventAggregatorFromMemberClass(nameof(Aggregator))]
[UseStateChanged]
public partial class CrazyEightsMainViewModel(CommandContainer commandContainer,
        CrazyEightsMainGameClass mainGame,
        CrazyEightsVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<RegularSimpleCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast), 
    IHandle<ChooseSuitEventModel>
{
    private partial void Subscribe();
    private partial void Unsubscribe();
    private readonly CrazyEightsVMData _model = viewModel;
    protected override void AdditionalInitialize()
    {
        Subscribe();
        _model.Deck1.NeverAutoDisable = true;
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        Unsubscribe();
        SuitScreen?.Close();
        SuitScreen = null;
        return base.CloseAsync();
    }
    protected override async Task OpenAsync()
    {
        CheckSuitScreens();
    }
    
    protected override bool CanEnableDeck()
    {
        return SuitScreen == null;
    }
    [StateProperty]
    public partial ChooseSuitViewModel? SuitScreen { get; set; }
    private async void CheckSuitScreens()
    {
        if (_model.ChooseSuit == true)
        {
            if (SuitScreen != null)
            {
                return;
            }
            SuitScreen = Resolver.Resolve<ChooseSuitViewModel>();
            return;
        }
        if (SuitScreen == null)
        {
            return;
        }
        SuitScreen.Close();
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
            Toast.ShowUserErrorToast("You must select a card first");
            return;
        }
        if (mainGame.IsValidMove(deck) == false)
        {
            Toast.ShowUserErrorToast("Illegal Move");
            _model.PlayerHand1.UnselectAllObjects();
            return;
        }
        await mainGame.PlayCardAsync(deck);
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