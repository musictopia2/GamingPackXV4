namespace Blackjack.Core.Logic;
[SingletonGame]
public class BlackjackMainGameClass : RegularDeckOfCardsGameClass<BlackjackCardInfo>, IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly IAsyncDelayer _delayer;
    private readonly IToast _toast;
    private readonly ISystemError _error;
    private readonly CommandContainer _command;
    internal BlackjackSaveInfo _saveRoot;
    internal bool GameGoing { get; set; }
    private BlackjackMainViewModel? _model;
    private int _oneNeeded;
    private bool _computerStartChoice; //should have been boolean
    public BlackjackMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IGamePackageResolver container,
        IAsyncDelayer delayer,
        IToast toast,
        ISystemError error,
        CommandContainer command
        )
    {
        _thisState = thisState;
        Aggregator = aggregator;
        _delayer = delayer;
        _toast = toast;
        _error = error;
        _command = command;
        _saveRoot = container.ReplaceObject<BlackjackSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
    }
    public override Task NewGameAsync(DeckObservablePile<BlackjackCardInfo> deck)
    {
        throw new CustomBasicException("You must send in the view model this time");
    }
    public Task NewGameAsync(DeckObservablePile<BlackjackCardInfo> deck, BlackjackMainViewModel model)
    {
        _model = model; //i think should be here.
        GameGoing = true;
        return base.NewGameAsync(deck);
    }
    public IEventAggregator Aggregator { get; }
    public override async Task<bool> CanOpenSavedSinglePlayerGameAsync()
    {
        return await _thisState.CanOpenSavedSinglePlayerGameAsync();
    }
    public override async Task OpenSavedGameAsync()
    {
        DeckList.OrderedObjects(); //i think
        _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<BlackjackSaveInfo>();
        if (_saveRoot.DeckList.Count > 0)
        {
            var newList = _saveRoot.DeckList.GetNewObjectListFromDeckList(DeckList);
            DeckPile!.OriginalList(newList);
        }
    }
    private bool _isBusy;
    public async Task SaveStateAsync()
    {
        if (_isBusy)
        {
            return;
        }
        _isBusy = true;
        _saveRoot.DeckList = DeckPile!.GetCardIntegers();
        await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot); //i think
        _isBusy = false;
    }

    public async Task ShowWinAsync()
    {
        _toast.ShowSuccessToast("Congratulations, you won");
        await Task.Delay(2000);
        GameGoing = false;
        await this.SendGameOverAsync(_error);
    }
    protected override void AfterShuffle()
    {
        DeckRegularDict<BlackjackCardInfo> newCol;
        if (_model == null)
        {
            throw new CustomBasicException("Model must be populated when running aftershuffle function");
        }
        int x;
        int y;
        bool choicess = false;
        _oneNeeded = 0;
        _model.NeedsAceChoice = false; // defaults to false.  must be proven true.
        _model.CanHitOrStay = false; // has to start that it can't hit or stay.
        for (x = 1; x <= 2; x++)
        {
            newCol = new DeckRegularDict<BlackjackCardInfo>();
            for (y = 1; y <= 2; y++)
            {
                newCol.Add(_model.DeckPile.DrawCard());
            }
            if (x == 2)
            {
                _model.ComputerStack!.ClearBoard(newCol, ref _computerStartChoice);
            }
            else
            {
                _model.HumanStack!.ClearBoard(newCol, ref choicess);
            }
        }
        if (choicess == true)
        {
            _oneNeeded = 2;
            _model.NeedsAceChoice = true;
        }
        else
        {
            _model.HumanPoints = _model.HumanStack!.CalculateScore(_model);
        }
    }
    public async Task HumanHitAsync()
    {
        bool choicess = false;
        _model!.HumanStack!.HitMe(_model.DeckPile!.DrawCard(), ref choicess);
        int points;
        if (choicess == false)
        {
            points = _model.HumanStack.CalculateScore(_model);
            _model.HumanPoints = points;
            _command.UpdateAll();
            if (points > 21)
            {
                await PrivateGameOverAsync();
                return;
            }
            if (points == 21)
            {
                await PrivateGameOverAsync();
                return;
            }
        }
        if (choicess == true)
        {
            _oneNeeded = 1;
            _model.NeedsAceChoice = true;
        }
    }
    private async Task PrivateGameOverAsync()
    {
        string messageToShow;
        _command.UpdateAll(); //so i can see the results before hand.
        if (_model!.HumanPoints == 21)
        {
            messageToShow = "Congratuations, you won because you got 21 points exactly";
            _model.Wins += 1;
        }
        else if (_model.ComputerPoints == 21)
        {
            messageToShow = "Sorry, you lost because the dealer got 21 points exactly";
            _model.Losses += 1;
        }
        else if (_model.HumanPoints > 21)
        {
            messageToShow = "Sorry, you lost because you busted for going over 21 points";
            _model.Losses += 1;
        }
        else if (_model.HumanPoints == _model.ComputerPoints)
        {
            messageToShow = "Its a draw";
            _model.Draws += 1;
        }
        else if (_model.ComputerPoints > 21)
        {
            messageToShow = "Congratulations, you won because the dealer went over 21 points";
            _model.Wins += 1;
        }
        else if (_model.HumanPoints > _model.ComputerPoints)
        {
            messageToShow = "Congratulations, you won because you had more points than the dealer had";
            _model.Wins += 1;
        }
        else
        {
            messageToShow = "Sorry, you lost because the dealer got more points than you had";
            _model.Losses += 1;
        }
        _toast.ShowInfoToast(messageToShow);
        await Task.Delay(2000);
        await this.SendGameOverAsync(_error);
    }
    public async Task HumanSelectAsync(bool choicess)
    {
        _model!.SelectedYet = true;
        _model.HumanStack!.Reveal(1);
        if (choicess == true)
        {
            _oneNeeded = 1;
            _model.NeedsAceChoice = true;
            return;
        }
        _model.HumanPoints = _model.HumanStack.CalculateScore(_model);
        if (_model.HumanPoints == 21)
        {
            await PrivateGameOverAsync();
            return;
        }
        if (_model.HumanPoints > 21)
        {
            await PrivateGameOverAsync();
            return;
        }
    }
    public async Task HumanAceAsync(EnumAceChoice whichOne)
    {
        _model!.NeedsAceChoice = false;
        if (whichOne == EnumAceChoice.Low)
        {
            _model.HumanStack!.AceChose(_oneNeeded, true);
        }
        else
        {
            _model.HumanStack!.AceChose(_oneNeeded, false);
        }
        _model.HumanPoints = _model.HumanStack.CalculateScore(_model);
        if (_model.HumanPoints == 21)
        {
            await PrivateGameOverAsync();
            return;
        }
        if (_model.HumanPoints > 21)
        {
            await PrivateGameOverAsync();
            return;
        }
    }
    public async Task HumanStayAsync()
    {
        await ComputerTurnAsync();
    }
    private async Task ComputerTurnAsync()
    {
        _model!.SelectedYet = false;
        _model.ComputerStack!.Reveal(2);
        _command.UpdateAll();
        if (_computerStartChoice == false)
        {
            _model.ComputerPoints = _model.ComputerStack.CalculateScore(_model);
        }
        await _delayer.DelaySeconds(0.55);
        if (_computerStartChoice == true)
        {
            _model.ComputerStack.AceChose(2, true);
        }
        _model.ComputerPoints = _model.ComputerStack.CalculateScore(_model);
        bool Choicess = false;
        _model.ComputerStack.ComputerSelectFirst(ref Choicess);
        _model.SelectedYet = true;
        _model.ComputerStack.Reveal(1);
        _model.CommandContainer.UpdateAll();
        if (Choicess == true)
        {
            _model.ComputerStack.AceChose(1, WillComputerChoose1());
        }
        do
        {
            _model.ComputerPoints = _model.ComputerStack.CalculateScore(_model);
            _model.CommandContainer.UpdateAll();
            await _delayer.DelaySeconds(0.5);
            if (_model.ComputerPoints > _model.HumanPoints)
            {
                await PrivateGameOverAsync();
                return;
            }
            if (_model.ComputerPoints > 21)
            {
                await PrivateGameOverAsync();
                return;
            }
            if (_model.ComputerPoints == _model.HumanPoints && _model.ComputerPoints > 15)
            {
                await PrivateGameOverAsync();
                return;
            }
            Choicess = false;
            _model.ComputerStack.HitMe(_model.DeckPile!.DrawCard(), ref Choicess);
            _model.CommandContainer.UpdateAll();
            if (Choicess == true)
            {
                _model.ComputerStack.AceChose(1, WillComputerChoose1());
            }
            _model.CommandContainer.UpdateAll();
            await _delayer.DelaySeconds(0.5);
        }
        while (true);
    }
    private bool WillComputerChoose1()
    {
        int Points1;
        int Points2;
        // 1 is with the 1
        // 2 is with the 11
        Points1 = _model!.ComputerPoints + 1;
        Points2 = _model.ComputerPoints + 11;
        if (Points1 == 21)
        {
            return true;
        }
        if (Points2 == 21)
        {
            return false;// because that will get to 21
        }
        if (Points2 > 21)
        {
            return true;
        }
        if (Points2 > _model.HumanPoints)
        {
            return false;
        }
        if (Points1 > _model.HumanPoints)
        {
            return true;
        }
        if (Points1 == _model.HumanPoints && Points1 > 15)
        {
            return true;
        }
        if (Points2 == _model.HumanPoints)
        {
            return false; //would rather tie than risk the human winning;
        }
        return false;
    }
}