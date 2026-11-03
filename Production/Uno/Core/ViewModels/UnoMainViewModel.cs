namespace Uno.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class UnoMainViewModel(CommandContainer commandContainer,
        UnoMainGameClass mainGame,
        UnoVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        UnoGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<UnoCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    [StateProperty]
    public partial SayUnoViewModel? SayUnoScreen { get; set; }
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        gameContainer.OpenSaidUnoAsync = LoadSayUnoAsync;
        gameContainer.CloseSaidUnoAsync = CloseSayUnoAsync;
        base.AdditionalInitialize();
    }
    protected override async Task OpenAsync()
    {
        if (gameContainer.SaveRoot.GameStatus == EnumGameStatus.WaitingForUno)
        {
            await LoadSayUnoAsync();
        }
    }
    protected override async Task CloseAsync()
    {
        await CloseSayUnoAsync();
    }
    private async Task LoadSayUnoAsync()
    {
        if (gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            return;
        }
        SayUnoScreen = Resolver.Resolve<SayUnoViewModel>();
    }
    private async Task CloseSayUnoAsync()
    {
        SayUnoScreen?.Close();
        SayUnoScreen = null;
    }
    protected override bool CanEnableDeck()
    {
        if (mainGame!.SaveRoot!.GameStatus != EnumGameStatus.NormalPlay)
        {
            return false;
        }
        if (gameContainer.AlreadyDrew == true)
        {
            return false;
        }
        return CanDraw();
    }
    protected override bool CanEnablePile1()
    {
        return mainGame!.SaveRoot!.GameStatus == EnumGameStatus.NormalPlay;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int deck = viewModel.PlayerHand1!.ObjectSelected();
        if (deck == 0)
        {
            Toast.ShowUserErrorToast("You must select a card first");
            return;
        }
        if (mainGame!.CanPlay(deck) == false)
        {
            Toast.ShowUserErrorToast("Illegal Move");
            viewModel.PlayerHand1.UnselectAllObjects();
            return;
        }
        await mainGame.ProcessPlayAsync(deck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    private bool CanDraw()
    {
        return !mainGame!.SingleInfo!.MainHandList.Any(x => mainGame.CanPlay(x.Deck));
    }
    public override bool CanEndTurn()
    {
        if (mainGame!.SaveRoot!.GameStatus != EnumGameStatus.NormalPlay)
        {
            return false;
        }
        if (CanDraw() == false)
        {
            return false;
        }
        return gameContainer.AlreadyDrew;
    }
}