namespace SorryCardGame.Core.ViewModels;
[InstanceGame]
public class SorryCardGameMainViewModel(CommandContainer commandContainer,
        SorryCardGameMainGameClass mainGame,
        SorryCardGameVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<SorryCardGameCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        viewModel.OtherPile!.SendEnableProcesses(this, () =>
        {
            return mainGame.SaveRoot!.GameStatus == EnumGameStatus.Regular;
        });
        viewModel.PlayerHand1!.Maximum = 8;
        CommandContainer!.ExecutingChanged = CommandContainer_ExecutingChanged;
        Stops = new();
        Stops.MaxTime = 7000;
        Stops.TimeUp += Stops_TimeUp;
        viewModel.OtherPile!.PileClickedAsync = OtherPile_PileClickedAsync;
        base.AdditionalInitialize();
    }
    private async Task OtherPile_PileClickedAsync()
    {
        var thisList = viewModel.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count == 0)
        {
            Toast.ShowUserErrorToast("Must choose at least one card to play");
            return;
        }
        bool rets = mainGame!.IsValidMove(thisList);
        if (rets == false)
        {
            Toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("regularplay", thisList.GetDeckListFromObjectList());
        }
        await mainGame.PlaySeveralCards(thisList);
    }
    protected override Task CloseAsync()
    {
        Stops?.TimeUp -= Stops_TimeUp;
        return base.CloseAsync();
    }
    private async void Stops_TimeUp()
    {
        await StopTimerAsync();
    }
    private async Task StopTimerAsync()
    {
        CommandContainer!.ManuelFinish = true;
        CommandContainer.IsExecuting = true;
        int myPlayer = mainGame!.PlayerList!.GetSelf().Id;
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("timeout", myPlayer);
        }
        await mainGame.NoSorryAsync(myPlayer);
    }
    private void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer!.IsExecuting)
        {
            return;
        }
        if (mainGame!.SaveRoot!.GameStatus == EnumGameStatus.HasDontBeSorry || mainGame.SaveRoot.GameStatus == EnumGameStatus.WaitForSorry21)
        {
            viewModel.PlayerHand1!.AutoSelect = EnumHandAutoType.None;
            Stops!.StartTimer();
            return;
        }
    }
    public CustomStopWatchCP? Stops;
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
    private async Task PlaySorryCardAsync(SorryCardGameCardInformation thisCard)
    {
        int myID = mainGame!.PlayerList!.GetSelf().Id;
        if (mainGame.BasicData!.MultiPlayer)
        {
            SorryPlay thisPlay = new();
            thisPlay.Deck = thisCard.Deck;
            thisPlay.Player = myID;
            await mainGame.Network!.SendAllAsync("sorrycard", thisPlay);
        }
        await mainGame.PlaySorryCardAsync(thisCard, myID);
    }
    protected override async Task ProcessHandClickedAsync(SorryCardGameCardInformation thisCard, int index)
    {
        if (mainGame!.SaveRoot!.GameStatus == EnumGameStatus.WaitForSorry21)
        {
            Stops!.PauseTimer();
            if (thisCard.Sorry == EnumSorry.At21)
            {
                await PlaySorryCardAsync(thisCard);
                return;
            }
            Toast.ShowUserErrorToast("Illegal Move");
            await StopTimerAsync();
            return;
        }
        if (mainGame.SaveRoot.GameStatus == EnumGameStatus.HasDontBeSorry)
        {
            Stops!.PauseTimer();
            if (thisCard.Sorry == EnumSorry.Dont)
            {
                await PlaySorryCardAsync(thisCard);
                return;
            }
            Toast.ShowUserErrorToast("Illegal Move");
            await StopTimerAsync();
            return;
        }
        throw new CustomBasicException("If the game status is not wait for sorry21 or has don't be sorry; then can't choose just one card to play");
    }
}