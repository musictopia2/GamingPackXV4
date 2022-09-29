namespace SorryCardGame.Core.ViewModels;
[InstanceGame]
public class SorryCardGameMainViewModel : BasicCardGamesVM<SorryCardGameCardInformation>
{
    private readonly SorryCardGameMainGameClass _mainGame;
    private readonly SorryCardGameVMData _model;
    private readonly IToast _toast;
    public SorryCardGameMainViewModel(CommandContainer commandContainer,
        SorryCardGameMainGameClass mainGame,
        SorryCardGameVMData viewModel,
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
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        _model.OtherPile!.SendEnableProcesses(this, () =>
        {
            return _mainGame.SaveRoot!.GameStatus == EnumGameStatus.Regular;
        });
        _model.PlayerHand1!.Maximum = 8;
        CommandContainer!.ExecutingChanged = CommandContainer_ExecutingChanged;
        Stops = new();
        Stops.MaxTime = 7000;
        Stops.TimeUp += Stops_TimeUp;
        _model.OtherPile.PileClickedAsync = OtherPile_PileClickedAsync;
    }
    private async Task OtherPile_PileClickedAsync()
    {
        var thisList = _model.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count == 0)
        {
            _toast.ShowUserErrorToast("Must choose at least one card to play");
            return;
        }
        bool rets = _mainGame!.IsValidMove(thisList);
        if (rets == false)
        {
            _toast.ShowUserErrorToast("Illegal Move");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("regularplay", thisList.GetDeckListFromObjectList());
        }
        await _mainGame.PlaySeveralCards(thisList);
    }
    protected override Task TryCloseAsync()
    {
        Stops.TimeUp -= Stops_TimeUp;
        return base.TryCloseAsync();
    }
    private async void Stops_TimeUp()
    {
        await StopTimerAsync();
    }
    private async Task StopTimerAsync()
    {
        CommandContainer!.ManuelFinish = true;
        CommandContainer.IsExecuting = true;
        int myPlayer = _mainGame!.PlayerList!.GetSelf().Id;
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("timeout", myPlayer);
        }
        await _mainGame.NoSorryAsync(myPlayer);
    }
    private void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer!.IsExecuting)
        {
            return;
        }
        if (_mainGame!.SaveRoot!.GameStatus == EnumGameStatus.HasDontBeSorry || _mainGame.SaveRoot.GameStatus == EnumGameStatus.WaitForSorry21)
        {
            _model.PlayerHand1!.AutoSelect = EnumHandAutoType.None;
            Stops!.StartTimer();
            return;
        }
    }
    public CustomStopWatchCP Stops;
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
        int myID = _mainGame!.PlayerList!.GetSelf().Id;
        if (_mainGame.BasicData!.MultiPlayer)
        {
            SorryPlay thisPlay = new();
            thisPlay.Deck = thisCard.Deck;
            thisPlay.Player = myID;
            await _mainGame.Network!.SendAllAsync("sorrycard", thisPlay);
        }
        await _mainGame.PlaySorryCardAsync(thisCard, myID);
    }
    protected override async Task ProcessHandClickedAsync(SorryCardGameCardInformation thisCard, int index)
    {
        if (_mainGame!.SaveRoot!.GameStatus == EnumGameStatus.WaitForSorry21)
        {
            Stops!.PauseTimer();
            if (thisCard.Sorry == EnumSorry.At21)
            {
                await PlaySorryCardAsync(thisCard);
                return;
            }
            _toast.ShowUserErrorToast("Illegal Move");
            await StopTimerAsync();
            return;
        }
        if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.HasDontBeSorry)
        {
            Stops!.PauseTimer();
            if (thisCard.Sorry == EnumSorry.Dont)
            {
                await PlaySorryCardAsync(thisCard);
                return;
            }
            _toast.ShowUserErrorToast("Illegal Move");
            await StopTimerAsync();
            return;
        }
        throw new CustomBasicException("If the game status is not wait for sorry21 or has don't be sorry; then can't choose just one card to play");
    }
}