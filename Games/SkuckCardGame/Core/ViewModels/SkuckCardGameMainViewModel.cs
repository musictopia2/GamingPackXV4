namespace SkuckCardGame.Core.ViewModels;
[InstanceGame]
public partial class SkuckCardGameMainViewModel : BasicCardGamesVM<SkuckCardGameCardInformation>
{
    private readonly SkuckCardGameMainGameClass _mainGame;
    private readonly SkuckCardGameVMData _model;
    private readonly IGamePackageResolver _resolver;
    public SkuckCardGameMainViewModel(CommandContainer commandContainer,
        SkuckCardGameMainGameClass mainGame,
        SkuckCardGameVMData viewModel,
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
        _model.ChangeScreen = ScreenChangeAsync; //try this now.
        _resolver = resolver;
        _model.Deck1.NeverAutoDisable = true;
        LoadPlayerControls();
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    private bool _closed;
    protected override async Task TryCloseAsync()
    {
        _closed = true;
        await CloseBiddingScreenAsync();
        await CloseChoosePlayScreenAsync();
        await CloseSuitScreenAsync();
        await base.TryCloseAsync();
    }
    public SkuckBiddingViewModel? BidScreen { get; set; }
    public SkuckChoosePlayViewModel? ChoosePlayScreen { get; set; }
    public SkuckSuitViewModel? SuitScreen { get; set; }
    private void LoadPlayerControls()
    {
        _mainGame!.PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.TempHand == null)
            {
                throw new CustomBasicException("TempHand was never created.  Rethink");
            }
            thisPlayer.TempHand.SendEnableProcesses(this, () =>
            {
                if (thisPlayer.PlayerCategory != EnumPlayerCategory.Self)
                {
                    return false;
                }
                return _mainGame.SaveRoot!.WhatStatus == EnumStatusList.NormalPlay;
            });

            if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisPlayer.TempHand.SelectedCard = TempHand_SelectedCard;
            }
        });
    }
    private void TempHand_SelectedCard()
    {
        _model.PlayerHand1!.UnselectAllObjects();
    }
    protected override Task ActivateAsync()
    {
        ScreenChangeAsync();
        return base.ActivateAsync();
    }
    private async void ScreenChangeAsync()
    {
        if (_model == null || _closed)
        {
            return;
        }
        if (_mainGame.SaveRoot.WhatStatus == EnumStatusList.NormalPlay)
        {
            await CloseChoosePlayScreenAsync();
            await CloseBiddingScreenAsync();
            await CloseSuitScreenAsync();
            _model!.TrickArea1.Visible = true;
            return;
        }
        _model.TrickArea1.Visible = false;
        if (_mainGame.SaveRoot.WhatStatus == EnumStatusList.ChooseBid)
        {
            await CloseChoosePlayScreenAsync();
            await CloseSuitScreenAsync();
            await OpenBiddingAsync();
            return;
        }
        if (_mainGame.SaveRoot.WhatStatus == EnumStatusList.ChoosePlay)
        {
            await CloseBiddingScreenAsync();
            await CloseSuitScreenAsync();
            await OpenChoosePlayAsync();
            return;
        }
        if (_mainGame.SaveRoot.WhatStatus == EnumStatusList.ChooseTrump)
        {
            await CloseBiddingScreenAsync();
            await CloseChoosePlayScreenAsync();
            await OpenSuitAsync();
            return;
        }
        throw new CustomBasicException("Not supported.  Rethink");
    }
    private async Task CloseBiddingScreenAsync()
    {
        if (BidScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(BidScreen);
        BidScreen = null;
    }
    private async Task CloseChoosePlayScreenAsync()
    {
        if (ChoosePlayScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(ChoosePlayScreen);
        ChoosePlayScreen = null;
    }
    private async Task CloseSuitScreenAsync()
    {
        if (SuitScreen == null)
        {
            return;
        }
        await CloseSpecificChildAsync(SuitScreen);
        SuitScreen = null;
    }
    private async Task OpenBiddingAsync()
    {
        if (BidScreen != null)
        {
            return;
        }
        BidScreen = _resolver.Resolve<SkuckBiddingViewModel>();
        await LoadScreenAsync(BidScreen);
    }
    private async Task OpenChoosePlayAsync()
    {
        if (ChoosePlayScreen != null)
        {
            return;
        }
        ChoosePlayScreen = _resolver.Resolve<SkuckChoosePlayViewModel>();
        await LoadScreenAsync(ChoosePlayScreen);
    }
    private async Task OpenSuitAsync()
    {
        if (SuitScreen != null)
        {
            return;
        }
        SuitScreen = _resolver.Resolve<SkuckSuitViewModel>();
        await LoadScreenAsync(SuitScreen);
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
    protected override Task OnAutoSelectedHandAsync()
    {
        SkuckCardGamePlayerItem thisPlayer = _mainGame!.PlayerList!.GetSelf();
        thisPlayer.TempHand!.UnselectAllCards();
        return base.OnAutoSelectedHandAsync();
    }
}