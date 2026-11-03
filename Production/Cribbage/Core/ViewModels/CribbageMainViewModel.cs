namespace Cribbage.Core.ViewModels;
[InstanceGame]
public partial class CribbageMainViewModel(CommandContainer commandContainer,
        CribbageMainGameClass mainGame,
        CribbageVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<CribbageCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = false;
        viewModel.Pile1.Text = "Start Card";
        if (mainGame.PlayerList.Count == 2)
        {
            viewModel.CribFrame.Maximum = 4;
        }
        else
        {
            viewModel.CribFrame.Maximum = 3;
        }
        viewModel.MainFrame.Maximum = 10;
        viewModel.PlayerHand1.Maximum = 6;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
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
    protected override bool AlwaysEnableHand()
    {
        return false;
    }
    protected override bool CanEnableHand()
    {
        return mainGame!.SaveRoot!.WhatStatus == EnumGameStatus.PlayCard || mainGame.SaveRoot.WhatStatus == EnumGameStatus.CardsForCrib;
    }
    public bool CanContinue => mainGame!.SaveRoot!.WhatStatus == EnumGameStatus.GetResultsCrib
            || mainGame.SaveRoot.WhatStatus == EnumGameStatus.GetResultsHand;
    [Command(EnumCommandCategory.Game)]
    public async Task ContinueAsync()
    {
        mainGame!.SingleInfo = mainGame.PlayerList!.GetSelf();
        mainGame.SingleInfo.FinishedLooking = true;
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("endview", mainGame.SingleInfo.Id);
        }
        await mainGame.EndViewAsync();
    }

    public bool CanPlay => mainGame!.SaveRoot!.WhatStatus == EnumGameStatus.PlayCard;
    [Command(EnumCommandCategory.Game)]
    public async Task PlayAsync()
    {
        var thisList = viewModel.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count > 1)
        {
            throw new CustomBasicException("It should have only allowed selecting one at a time now");
        }
        if (thisList.Count == 0)
        {
            Toast.ShowUserErrorToast("You must choose a card to play");
            return;
        }
        if (mainGame!.IsValidMove(thisList.Single()) == false)
        {
            Toast.ShowUserErrorToast("You cannot play a card that makes it more than 31 points");
            return;
        }
        await mainGame!.PlayCardAsync(thisList.Single());
    }
    public bool CanCrib => mainGame!.SaveRoot!.WhatStatus == EnumGameStatus.CardsForCrib;
    [Command(EnumCommandCategory.Game)]
    public async Task CribAsync()
    {
        var thisList = viewModel.PlayerHand1!.ListSelectedObjects();
        if (mainGame!.PlayerList.Count == 3 && thisList.Count > 1)
        {
            throw new CustomBasicException("It should have only allowed selecting one card at a time because 3 players");
        }
        int maxs;
        if (mainGame.PlayerList.Count == 3)
        {
            maxs = 1;
        }
        else
        {
            maxs = 2;
        }
        if (thisList.Count != maxs)
        {
            Toast.ShowUserErrorToast($"Must select {maxs} cards for crib");
            return;
        }
        mainGame.SingleInfo = mainGame.PlayerList!.GetSelf();
        await mainGame.ProcessCribAsync(thisList);
    }
}