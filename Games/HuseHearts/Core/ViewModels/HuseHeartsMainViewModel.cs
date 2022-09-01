namespace HuseHearts.Core.ViewModels;
[InstanceGame]
public class HuseHeartsMainViewModel : BasicCardGamesVM<HuseHeartsCardInformation>
{
    private readonly HuseHeartsMainGameClass _mainGame;
    private readonly HuseHeartsVMData _model;
    private readonly IGamePackageResolver _resolver;
    public HuseHeartsMainViewModel(CommandContainer commandContainer,
        HuseHeartsMainGameClass mainGame,
        HuseHeartsVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _resolver = resolver;
        _model.Deck1.NeverAutoDisable = true;
        _model.Blind1.SendEnableProcesses(this, () => false);
        _model.Dummy1.SendEnableProcesses(this, () =>
        {
            if (_mainGame!.SaveRoot!.GameStatus != EnumStatus.Normal)
            {
                return false;
            }
            return _model!.TrickArea1!.FromDummy;
        });
        _model.ChangeScreen = ChangeScreenAsync;
    }
    protected override async Task ActivateAsync()
    {
        await base.ActivateAsync();
        ChangeScreenAsync();
    }
    protected override async Task TryCloseAsync()
    {
        await CloseMoonAsync();
        await ClosePassingAsync();
        await base.TryCloseAsync();
    }
    public MoonViewModel? MoonScreen { get; set; }
    public PassingViewModel? PassingScreen { get; set; }
    private async Task LoadMoonAsync()
    {
        if (MoonScreen != null)
        {
            return;
        }
        MoonScreen = _resolver.Resolve<MoonViewModel>();
        await LoadScreenAsync(MoonScreen);
    }
    private async void ChangeScreenAsync()
    {
        if (_model == null)
        {
            return;
        }
        _model.TrickArea1.Visible = _model.GameStatus == EnumStatus.Normal;
        if (_model.GameStatus == EnumStatus.ShootMoon)
        {
            await LoadMoonAsync();
            return;
        }
        await CloseMoonAsync();
        if (_model.GameStatus == EnumStatus.Passing)
        {
            await LoadPassingAsync();
            return;
        }
        await ClosePassingAsync();
    }
    private async Task ClosePassingAsync()
    {
        if (PassingScreen != null)
        {
            await CloseSpecificChildAsync(PassingScreen);
            PassingScreen = null;
        }
    }
    private async Task CloseMoonAsync()
    {
        if (MoonScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(MoonScreen);
        MoonScreen = null;
    }
    private async Task LoadPassingAsync()
    {
        if (PassingScreen != null)
        {
            return;
        }
        PassingScreen = _resolver.Resolve<PassingViewModel>();
        await LoadScreenAsync(PassingScreen);
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
        if (_mainGame!.SaveRoot!.GameStatus == EnumStatus.Passing)
        {
            return true;
        }
        if (_mainGame.SaveRoot.GameStatus == EnumStatus.Normal)
        {
            if (_model!.TrickArea1!.FromDummy == true)
            {
                return false;
            }
            return true;
        }
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
}