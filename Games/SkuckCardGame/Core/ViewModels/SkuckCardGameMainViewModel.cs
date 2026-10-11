namespace SkuckCardGame.Core.ViewModels;
[InstanceGame]
[UseStateChanged]
public partial class SkuckCardGameMainViewModel(CommandContainer commandContainer,
        SkuckCardGameMainGameClass mainGame,
        SkuckCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<SkuckCardGameCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.ChangeScreen = ScreenChangeAsync; //try this now.
        viewModel.Deck1.NeverAutoDisable = true;
        LoadPlayerControls();
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    protected override Task CloseAsync()
    {
        _closed = true;
        CloseBiddingScreen();
        CloseChoosePlayScreen();
        CloseSuitScreen();
        return base.CloseAsync();
    }
    partial void CreateCommands(CommandContainer command);
    private bool _closed;
    [StateProperty]
    public partial SkuckBiddingViewModel? BidScreen { get; set; }
    [StateProperty]
    public partial SkuckChoosePlayViewModel? ChoosePlayScreen { get; set; }
    [StateProperty]
    public partial SkuckSuitViewModel? SuitScreen { get; set; }
    private void LoadPlayerControls()
    {
        mainGame!.PlayerList!.ForEach(thisPlayer =>
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
                return mainGame.SaveRoot!.WhatStatus == EnumStatusList.NormalPlay;
            });

            if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisPlayer.TempHand.SelectedCard = TempHand_SelectedCard;
            }
        });
    }
    private void TempHand_SelectedCard()
    {
        viewModel.PlayerHand1!.UnselectAllObjects();
    }
    protected override Task OpenAsync()
    {
        ScreenChangeAsync();
        return base.OpenAsync();
    }
    
    private void ScreenChangeAsync()
    {
        if (viewModel == null || _closed)
        {
            return;
        }
        if (mainGame.SaveRoot.WhatStatus == EnumStatusList.NormalPlay)
        {
            CloseChoosePlayScreen();
            CloseBiddingScreen();
            CloseSuitScreen();
            viewModel!.TrickArea1.Visible = true;
            return;
        }
        viewModel.TrickArea1.Visible = false;
        if (mainGame.SaveRoot.WhatStatus == EnumStatusList.ChooseBid)
        {
            CloseChoosePlayScreen();
            CloseSuitScreen();
            OpenBidding();
            return;
        }
        if (mainGame.SaveRoot.WhatStatus == EnumStatusList.ChoosePlay)
        {
            CloseBiddingScreen();
            CloseSuitScreen();
            OpenChoosePlay();
            return;
        }
        if (mainGame.SaveRoot.WhatStatus == EnumStatusList.ChooseTrump)
        {
            CloseBiddingScreen();
            CloseChoosePlayScreen();
            OpenSuit();
            return;
        }
        throw new CustomBasicException("Not supported.  Rethink");
    }
    private void CloseBiddingScreen()
    {
        if (BidScreen == null)
        {
            return;
        }
        BidScreen = null;
    }
    private void CloseChoosePlayScreen()
    {
        if (ChoosePlayScreen == null)
        {
            return;
        }
        ChoosePlayScreen = null;
    }
    private void CloseSuitScreen()
    {
        if (SuitScreen == null)
        {
            return;
        }
        SuitScreen = null;
    }
    private void OpenBidding()
    {
        if (BidScreen != null)
        {
            return;
        }
        BidScreen = Resolver.Resolve<SkuckBiddingViewModel>();
    }
    private void OpenChoosePlay()
    {
        if (ChoosePlayScreen != null)
        {
            return;
        }
        ChoosePlayScreen = Resolver.Resolve<SkuckChoosePlayViewModel>();
    }
    private void OpenSuit()
    {
        if (SuitScreen != null)
        {
            return;
        }
        SuitScreen = Resolver.Resolve<SkuckSuitViewModel>();
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
        SkuckCardGamePlayerItem thisPlayer = mainGame!.PlayerList!.GetSelf();
        thisPlayer.TempHand!.UnselectAllCards();
        return base.OnAutoSelectedHandAsync();
    }
}