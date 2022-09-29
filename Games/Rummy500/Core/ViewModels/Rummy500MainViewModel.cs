namespace Rummy500.Core.ViewModels;
[InstanceGame]
public partial class Rummy500MainViewModel : BasicCardGamesVM<RegularRummyCard>
{
    private readonly Rummy500MainGameClass _mainGame;
    private readonly Rummy500VMData _model;
    private readonly Rummy500GameContainer _gameContainer;
    private readonly IToast _toast;
    public Rummy500MainViewModel(CommandContainer commandContainer,
        Rummy500MainGameClass mainGame,
        Rummy500VMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        Rummy500GameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _gameContainer = gameContainer;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        _model.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        _model.MainSets1.SetClickedAsync = MainSets1_SetClickedAsync;
        _model.DiscardList1.ObjectClickedAsync = DiscardList1_ObjectClickedAsync;
        _model.DiscardList1.BoardClickedAsync = DiscardList1_BoardClickedAsync;
        _model.MainSets1.SendEnableProcesses(this, () => _gameContainer!.AlreadyDrew);
        CreateCommands(CommandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override bool CanEnableDeck()
    {
        return !_gameContainer.AlreadyDrew;
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
    private bool _didClickDiscard;
    private async Task DiscardList1_BoardClickedAsync()
    {
        if (_didClickDiscard == true)
        {
            _didClickDiscard = false;
            return;
        }
        _didClickDiscard = true;
        await NewDiscardClickAsync(0);
    }
    private async Task DiscardList1_ObjectClickedAsync(RegularRummyCard thisObject, int index)
    {
        _didClickDiscard = true;
        await NewDiscardClickAsync(thisObject.Deck);
    }
    private async Task MainSets1_SetClickedAsync(int setNumber, int section, int deck)
    {
        var newcol = _model.PlayerHand1!.ListSelectedObjects();
        if (newcol.Count == 0)
        {
            _toast.ShowUserErrorToast("There is no card selected");
            return;
        }
        if (newcol.Count > 1)
        {
            _toast.ShowUserErrorToast("Only can expand one card at a time");
            return;
        }
        if (_model.PlayerHand1.HandList.Count == 1)
        {
            _toast.ShowUserErrorToast("Sorry, must have a card left for discard");
            return;
        }
        if (_mainGame.NeedsExpansion(newcol))
        {
            _toast.ShowUserErrorToast("Needs to use the extra card picked up first before anything else");
            return;
        }
        var thisCard = newcol.First();
        RummySet thisSet = _model.MainSets1!.GetIndividualSet(setNumber);
        int pos = thisSet.PositionToPlay(thisCard);
        if (pos == 0)
        {
            _toast.ShowUserErrorToast("This cannot be used to expand upto");
            return;
        }
        var thisCol = _model.MainSets1.SetList.ToBasicList();
        int x = 0;
        int nums = 0;
        thisCol.ForEach(newSet =>
        {
            x++;
            if (newSet.Equals(thisSet))
            {
                nums = x;
            }
        });
        if (nums == 0)
        {
            throw new CustomBasicException("Cannot find the rummy set that matches");
        }
        if (_gameContainer.BasicData!.MultiPlayer == true)
        {
            SendAddSet thisSend = new();
            thisSend.Deck = thisCard.Deck;
            thisSend.Position = pos;
            thisSend.Index = nums;
            await _gameContainer.Network!.SendAllAsync("addtoset", thisSend);
        }
        await _mainGame!.AddToSetAsync(nums, thisCard.Deck, pos);
    }
    private async Task NewDiscardClickAsync(int deck)
    {
        if (_mainGame!.CanProcessDiscard(out bool pickUp, ref deck, out string message) == false)
        {
            _toast.ShowUserErrorToast(message);
            return;
        }
        if (pickUp == true)
        {
            var thisCol = _model.DiscardList1!.DiscardListSelected(deck);
            if (thisCol.Count > 1)
            {
                var newCol = _mainGame.AppendDiscardList(thisCol);
                if (_mainGame.CardContainsRummy(deck, newCol) == false)
                {
                    _toast.ShowUserErrorToast("Sorry, cannot pick up more than one card because either invalid rummy or no card left for discard");
                    return;
                }
            }
            if (_gameContainer.BasicData!.MultiPlayer == true)
            {
                await _gameContainer.Network!.SendAllAsync("pickupfromdiscard", deck);
            }
            await _mainGame.PickupFromDiscardAsync(deck);
            return;
        }
        await _gameContainer.SendDiscardMessageAsync(deck);
        await _mainGame.DiscardAsync(deck);
    }
    public bool CanCreateSet => _gameContainer.AlreadyDrew;
    [Command(EnumCommandCategory.Game)]
    public async Task CreateSetAsync()
    {
        var thisCol = _model.PlayerHand1.ListSelectedObjects();
        if (thisCol.Count == _model.PlayerHand1.HandList.Count)
        {
            _toast.ShowUserErrorToast("Sorry, you must have one card left over to discard");
            _model.PlayerHand1.UnselectAllObjects();
            return;
        }
        if (_mainGame.NeedsExpansion(thisCol))
        {
            _toast.ShowUserErrorToast("Needs to use the extra card picked up first before anything else");
            return;
        }
        if (_mainGame!.IsValidRummy(thisCol, out EnumWhatSets settype, out bool seconds) == false)
        {
            _toast.ShowUserErrorToast("This is not a valid rummy");
            _model.PlayerHand1.UnselectAllObjects();
            return;
        }
        if (thisCol.Count == 1)
        {
            if (thisCol.Single().Deck == _gameContainer.PreviousCard)
            {
                _toast.ShowUserErrorToast("Sorry, since the last card left is the card picked up, then cannot put down the rummy");
                return;
            }
        }
        if (_gameContainer.BasicData!.MultiPlayer == true)
        {
            SendNewSet thisNew = new();
            thisNew.DeckList = thisCol.GetDeckListFromObjectList();
            thisNew.SetType = settype;
            thisNew.UseSecond = seconds;
            await _gameContainer.Network!.SendAllAsync("newset", thisNew);
        }
        await _mainGame.CreateNewSetAsync(thisCol, settype, seconds);
    }
    public bool CanDiscardCurrent => _gameContainer.AlreadyDrew;
    [Command(EnumCommandCategory.Game)]
    public async Task DiscardCurrentAsync()
    {
        await NewDiscardClickAsync(0);
    }
}