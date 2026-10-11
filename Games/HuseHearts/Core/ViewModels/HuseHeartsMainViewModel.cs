namespace HuseHearts.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class HuseHeartsMainViewModel(CommandContainer commandContainer,
        HuseHeartsMainGameClass mainGame,
        HuseHeartsVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<HuseHeartsCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.Blind1.SendEnableProcesses(this, () => false);
        viewModel.Dummy1.SendEnableProcesses(this, () =>
        {
            if (mainGame!.SaveRoot!.GameStatus != EnumStatus.Normal)
            {
                return false;
            }
            return viewModel!.TrickArea1!.FromDummy;
        });
        viewModel.ChangeScreen = ChangeScreen;
        base.AdditionalInitialize();
    }
    protected override Task OpenAsync()
    {
        ChangeScreen();
        return base.OpenAsync();
    }
    protected override Task CloseAsync()
    {
        CloseMoon();
        ClosePassing();
        return base.CloseAsync();
    }
    [StateProperty]
    public partial MoonViewModel? MoonScreen { get; set; }
    [StateProperty]
    public partial PassingViewModel? PassingScreen { get; set; }
    private void LoadMoon()
    {
        if (MoonScreen != null)
        {
            return;
        }
        MoonScreen = Resolver.Resolve<MoonViewModel>();
    }
    private void ChangeScreen()
    {
        if (viewModel == null)
        {
            return;
        }
        viewModel.TrickArea1.Visible = viewModel.GameStatus == EnumStatus.Normal;
        if (viewModel.GameStatus == EnumStatus.ShootMoon)
        {
            LoadMoon();
            return;
        }
        CloseMoon();
        if (viewModel.GameStatus == EnumStatus.Passing)
        {
            LoadPassing();
            return;
        }
        ClosePassing();
    }
    private void ClosePassing()
    {
        if (PassingScreen != null)
        {
            PassingScreen = null;
        }
    }
    private void CloseMoon()
    {
        if (MoonScreen == null)
        {
            return;
        }
        MoonScreen = null;
    }
    private void LoadPassing()
    {
        if (PassingScreen != null)
        {
            return;
        }
        PassingScreen = Resolver.Resolve<PassingViewModel>();
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
        if (mainGame!.SaveRoot!.GameStatus == EnumStatus.Passing)
        {
            return true;
        }
        if (mainGame.SaveRoot.GameStatus == EnumStatus.Normal)
        {
            if (viewModel!.TrickArea1!.FromDummy == true)
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