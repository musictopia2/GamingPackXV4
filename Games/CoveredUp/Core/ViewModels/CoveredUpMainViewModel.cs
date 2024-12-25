namespace CoveredUp.Core.ViewModels;
[InstanceGame]
public class CoveredUpMainViewModel : BasicCardGamesVM<RegularSimpleCard>
{
    private readonly CoveredUpMainGameClass _mainGame; //if we don't need, delete.
    private readonly CoveredUpGameContainer _gameContainer;
    private readonly IToast _toast;
    public CoveredUpVMData VMData { get; set; }
    public CoveredUpMainViewModel(CommandContainer commandContainer,
        CoveredUpMainGameClass mainGame,
        CoveredUpVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        CoveredUpGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        VMData = viewModel;
        _gameContainer = gameContainer;
        _toast = toast;
        foreach (var player in _mainGame.PlayerList)
        {
            player.LoadPlayerBoard(gameContainer, viewModel);
            if (player.PlayerCategory == EnumPlayerCategory.Self)
            {
                SelfBoard = player.PlayerBoard!; //all others has to have their boards (even though displayed differently).
                SelfBoard.SendEnableProcesses(this, () =>
                {
                    return VMData.OtherPile!.PileEmpty() == false;
                });
            }
        }
    }
    public IEnumerable<CoveredUpPlayerItem> Players => _gameContainer.PlayerList!;
    public PlayerBoardCP? SelfBoard { get; }

    //anything else needed is here.
    //if i need something extra, will add to template as well.
    protected override bool CanEnableDeck()
    {
        return VMData.OtherPile!.PileEmpty();
    }
    protected override bool CanEnablePile1()
    {
        return true;
        //if (VMData.OtherPile!.PileEmpty() == false)
        //{
        //    return false;
        //}

        //return VMData.OtherPile!.PileEmpty();
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int oldDeck;
        if (VMData.OtherPile!.PileEmpty() == true)
        {
            oldDeck = 0;
        }
        else
        {
            oldDeck = VMData.OtherPile.GetCardInfo().Deck;
        }
        if (_gameContainer.SaveRoot.WentOut && oldDeck == 0)
        {
            _toast.ShowUserErrorToast("Cannot pick up from discard anymore because somebody already went out");
            return;
        }
        if (oldDeck == 0)
        {
            await _mainGame!.PickupFromDiscardAsync();
            return;
        }
        if (oldDeck == _gameContainer!.PreviousCard)
        {
            _toast.ShowUserErrorToast("Cannot discard the same card that was picked up");
            return;
        }
        if (_mainGame.BasicData.MultiPlayer == true)
        {
            await _gameContainer.SendDiscardMessageAsync(oldDeck);
        }
        await _mainGame.DiscardAsync(oldDeck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}