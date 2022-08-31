namespace YaBlewIt.Core.ViewModels;
[InstanceGame]
public partial class YaBlewItMainViewModel : BasicCardGamesVM<YaBlewItCardInformation>
{
    private readonly YaBlewItMainGameClass _mainGame; //if we don't need, delete.
    private readonly YaBlewItGameContainer _gameContainer;
    private readonly IChooseColorProcesses _colorProcesses;
    private readonly IToast _toast;
    public YaBlewItVMData VMData { get; set; }
    public YaBlewItMainViewModel(CommandContainer commandContainer,
        YaBlewItMainGameClass mainGame,
        YaBlewItGameContainer gameContainer,
        YaBlewItVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IChooseColorProcesses colorProcesses,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _gameContainer = gameContainer;
        VMData = viewModel;
        _colorProcesses = colorProcesses;
        VMData.ColorPicker.ItemClickedAsync += ColorPicker_ItemClickedAsync;
        _toast = toast;
        CreateCommands(commandContainer);
    }
    protected override Task TryCloseAsync()
    {
        VMData.ColorPicker.ItemClickedAsync -= ColorPicker_ItemClickedAsync;
        return base.TryCloseAsync();
    }
    partial void CreateCommands(CommandContainer command);
    public bool CanRollDice()
    {
        if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.StartGambling
            || _mainGame.SaveRoot.GameStatus == EnumGameStatus.FinishGambling
            || _mainGame.SaveRoot.GameStatus == EnumGameStatus.MinerRolling
            || _mainGame.SaveRoot.GameStatus == EnumGameStatus.ResolveFire)
        {
            return true; //you can decide to roll.
        }
        return false;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task RollDiceAsync()
    {
        await _mainGame!.Roller!.RollDiceAsync();
    }
    public override bool CanEndTurn()
    {
        if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.ProspectorContinues
            || _mainGame.SaveRoot.GameStatus == EnumGameStatus.EndingTurn
            || _mainGame.SaveRoot.GameStatus == EnumGameStatus.ProspectorStarts)
        {
            return true; //well see about other conditions.
        }
        return false;
    }
    public bool CanPass()
    {
        if (_mainGame.SaveRoot.PlayedFaulty)
        {
            return false; //now you have to roll because you played it.
        }
        if (_mainGame.SaveRoot.GameStatus == EnumGameStatus.MinerRolling)
        {
            return true;
        }
        return false; //for now.
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PassAsync()
    {
        await _mainGame.PassAsync();
    }
    public bool CanPlayCard => _mainGame.SaveRoot.PlayedFaulty == false
        && _mainGame.SaveRoot.GameStatus != EnumGameStatus.Safe //if a safe card was played, has to protect a color.  after choosing card to protect, status has to be resolve fire again.
        && _mainGame.SaveRoot.GameStatus != EnumGameStatus.StartGambling
        && _mainGame.SaveRoot.GameStatus != EnumGameStatus.FinishGambling
        && VMData.PlayerHand1.HandList.Any(x => x.CardCategory != EnumCardCategory.Gem);
    [Command(EnumCommandCategory.Game)]
    public async Task PlayCardAsync()
    {
        //let you play  but if invalid, then toast.  that is best for this game.
        if (VMData.PlayerHand1.HasSelectedObject() == false)
        {
            _toast.ShowUserErrorToast("You need to select a card to play");
            return;
        }
        int deck = VMData.PlayerHand1.ObjectSelected();
        YaBlewItCardInformation card = _gameContainer.DeckList.GetSpecificItem(deck);
        if (card.CardCategory == EnumCardCategory.Gem)
        {
            _toast.ShowUserErrorToast("You can never play a gem card");
            return;
        }
        if (card.CardCategory == EnumCardCategory.None)
        {
            _toast.ShowUserErrorToast("The card category should never be none");
            return;
        }
        if (card.CardCategory == EnumCardCategory.Faulty)
        {
            if (_mainGame.SaveRoot.GameStatus != EnumGameStatus.EndingTurn)
            {
                _toast.ShowUserErrorToast("The faulty card should be played at the end of your turn to reroll");
                return;
            }
            await _mainGame.PlayCardAsync(deck);
            return;
        }
        if (card.CardCategory == EnumCardCategory.Safe)
        {
            if (_mainGame.SaveRoot.GameStatus != EnumGameStatus.ResolveFire)
            {
                _toast.ShowUserErrorToast("The safe card can only be played when resolving fires");
                return;
            }
            if (SafeColorListClass.GetColorChoices().Count == 0)
            {
                _toast.ShowUserErrorToast("There are no more colors you need to protect.  Therefore, no need to play a safe card");
                return;
            }
            //you can protect more than one via fire.
            await _mainGame.PlayCardAsync(deck);
            return;
        }
        if (card.CardCategory == EnumCardCategory.Jumper)
        {
            if (_mainGame.SaveRoot.GameStatus != EnumGameStatus.ProspectorStarts)
            {
                _toast.ShowUserErrorToast("The jumper can only be played when prospector starts");
                return;
            }
            await _mainGame.PlayCardAsync(deck);
            return;
        }
        if (card.CardCategory == EnumCardCategory.Fire)
        {
            _toast.ShowUserErrorToast("The fire card should never be in your hand");
            return;
        }
        _toast.ShowUserErrorToast("Not sure what to do about this card");
        await Task.CompletedTask;
    }
    public bool ShowSafeScreen => _mainGame.SaveRoot.GameStatus == EnumGameStatus.Safe;
    private async Task ColorPicker_ItemClickedAsync(EnumColors piece)
    {
        await _colorProcesses.ColorChosenAsync(piece);
    }
    public bool CanGamble => _mainGame.SaveRoot.GameStatus == EnumGameStatus.GamblingDecision && _mainGame.SaveRoot.PlayedFaulty == false;
    [Command(EnumCommandCategory.Game)]
    public async Task GambleAsync()
    {
        await _mainGame.GambleAsync();
    }
    public bool CanTakeClaim => _mainGame.SaveRoot.GameStatus == EnumGameStatus.GamblingDecision || _mainGame.SaveRoot.GameStatus == EnumGameStatus.ProspectorContinues; //if you can gamble then you can take claim period.
    [Command(EnumCommandCategory.Game)]
    public async Task TakeClaimAsync()
    {
        await _mainGame.TakeClaimAsync(false);
    }
    protected override bool CanEnableDeck()
    {
        return _mainGame.SaveRoot.GameStatus == EnumGameStatus.StartGambling && _mainGame.SaveRoot.DrewExtra == false;
    }
    protected override bool CanEnablePile1()
    {
        return false; //you do nothing with the discard piles.
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        //if we have anything, will be here.
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
}