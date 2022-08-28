namespace Risk.Core.Logic;
[SingletonGame]
public class RiskMainGameClass
    : SimpleBoardGameClass<RiskPlayerItem, RiskSaveInfo, EnumColorChoice, int>
    , IBeginningColors<EnumColorChoice, RiskPlayerItem, RiskSaveInfo>
    , IMiscDataNM, ISerializable
{
    public RiskMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        RiskVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        RiskGameContainer container,
        GameBoardProcesses gameBoardProcesses,
        DrawShuffleClass<RiskCardInfo, RiskPlayerItem> shuffle,
        ISystemError error,
        IToast toast
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, container, error, toast)
    {
        _model = model;
        _container = container;
        _container.EndMoveAsync = EndMoveAsync;
        _gameBoardProcesses = gameBoardProcesses;
        _shuffle = shuffle;
        _shuffle.RemovePossibleReshuffledCards = RemoveCardsFromDeck;
        //not sure yet where we will put the afterrolling.
        _shuffle.CurrentPlayer = CurrentPlayer;
        _shuffle.AfterDrawingAsync = AfterDrawingAsync; //i think needs to end turn.
    }
    private void RemoveCardsFromDeck(IListShuffler<RiskCardInfo> deck)
    {
        if (deck is DeckRegularDict<RiskCardInfo> improvs)
        {
            foreach (var player in PlayerList)
            {
                player.MainHandList.ForEach(card =>
                {
                    improvs.RemoveObjectByDeck(card.Deck); //hopefully okay because its using the address.
                });
            }
        }
        else
        {
            throw new CustomBasicException("Must use deck regular dictionary");
        }
    }
    private RiskPlayerItem CurrentPlayer()
    {
        return SingleInfo!;
    }
    private async Task DrawAsync()
    {
        await _shuffle.DrawAsync();
    }
    private async Task AfterDrawingAsync()
    {
        if (SaveRoot.CurrentCard is null || SaveRoot.CurrentCard.Deck == 0)
        {
            throw new CustomBasicException("There is no current card");
        }
        SingleInfo!.MainHandList.Add(SaveRoot.CurrentCard);
        SaveRoot.CurrentCard = null;
        await EndTurnAsync();
    }
    private readonly RiskVMData _model;
    private readonly RiskGameContainer _container;
    private readonly GameBoardProcesses _gameBoardProcesses;
    private readonly DrawShuffleClass<RiskCardInfo, RiskPlayerItem> _shuffle;
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadCups(true);
        _shuffle.SaveRoot = SaveRoot;
        BoardGameSaved();
        SetHand();
        await PopulateTerritoriesAsync();
    }
    private void SetHand()
    {
        if (PlayerList.DidChooseColors())
        {
            SingleInfo = PlayerList.GetSelf();
            _model.PlayerHand1.HandList = SingleInfo.MainHandList;
            SingleInfo = PlayerList.GetWhoPlayer(); //i think.
        }
    }
    private void LoadCups(bool autoResume)
    {
        SaveRoot!.LoadMod(_model!);
        _model.LoadCups(SaveRoot, autoResume);
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true; //i think needs to be here.
    }
    protected override async Task ComputerTurnAsync()
    {
        if (PlayerList.DidChooseColors() == true)
        {
            throw new CustomBasicException("Should not goto computer turn");
        }
        if (SingleInfo!.InGame == false)
        {
            throw new CustomBasicException("Not even in game");
        }
        await base.ComputerTurnAsync();
    }
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        if (IsLoaded == false)
        {
            LoadCups(false);
        }
        LoadControls();
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        SaveRoot!.ImmediatelyStartTurn = true;
        await FinishUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case nameof(IMultiplayerModel.SendAttackData):
                if (_container.StartSentAttack is not null)
                {
                    _container.StartSentAttack.Invoke();
                }
                SendAttackResult result = await js.DeserializeObjectAsync<SendAttackResult>(content); //can be iffy.
                if (_container.SentAttackProcessesAsync is null)
                {
                    throw new CustomBasicException("Must have send attack result"); //in newer version of c#, hopefully null checks are even better
                }
                await _container.SentAttackProcessesAsync.Invoke(result);
                break;
            case nameof(IMultiplayerModel.AttackArmies):
                SaveRoot.ArmiesInBattle = int.Parse(content);
                Network!.IsEnabled = true; //to wait for next message now i think.
                break;
            case nameof(IMultiplayerModel.SelectTerritory):
                TerritoryModel territory = _container.GetTerritory(int.Parse(content));
                await _gameBoardProcesses.TerritorySelectedAsync(territory, false);
                break;
            case nameof(IMultiplayerModel.ToNextStep):
                await ToNextStepAsync();
                break;
            case nameof(IMultiplayerModel.MoveArmies):
                _model.ArmiesChosen = int.Parse(content);
                await _gameBoardProcesses.MoveArmiesAsync(true);
                break;
            case nameof(IMultiplayerModel.ReturnRiskCards):
                var thisList = await content.GetSavedIntegerListAsync();
                await ReturnRiskCardsAsync(thisList);
                break;
            case nameof(IMultiplayerModel.PlaceArmies):
                _model.ArmiesChosen = int.Parse(content);
                await _gameBoardProcesses.PlaceArmiesAsync(true);
                break;

            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    private void ReenforcementProcesses()
    {
        SingleInfo = PlayerList.GetWhoPlayer();
        SaveRoot.Stage = EnumStageList.Place; //now you need to place them.
        SaveRoot.BonusReenforcements = BonusReEnforcements();
        SaveRoot.ArmiesToPlace = RegularNewReenforcements() + SaveRoot.BonusReenforcements + SingleInfo!.ReinforcementsGained;
        SingleInfo.ReinforcementsGained = 0;
        SaveRoot.ArmiesInBattle = 0;
        _gameBoardProcesses.ResetMove();
        _model.ArmiesChosen = 0;
        if (SaveRoot.ArmiesToPlace == 0)
        {
            throw new CustomBasicException("Cannot have 0 armies to place");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        if (PlayerList.DidChooseColors())
        {
            PrepStartTurn(); //try this too.
            SaveRoot.Stage = EnumStageList.Begin; //now its being.
            SaveRoot.ConqueredOne = false;
        }
        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    private int RegularNewReenforcements()
    {
        int howManyTerritories = _gameBoardProcesses.HowManyTerritories(WhoTurn);
        if (howManyTerritories <= 9)
        {
            return 3;
        }
        int incs = howManyTerritories / 3;
        return incs;
    }
    private int BonusReEnforcements()
    {
        int output = 0;
        if (_gameBoardProcesses.ContinentControlled(EnumContinent.Asia))
        {
            output += 7;
        }
        if (_gameBoardProcesses.ContinentControlled(EnumContinent.Europe))
        {
            output += 5;
        }
        if (_gameBoardProcesses.ContinentControlled(EnumContinent.NorthAmerica))
        {
            output += 5;
        }
        if (_gameBoardProcesses.ContinentControlled(EnumContinent.Africa))
        {
            output += 3;
        }
        if (_gameBoardProcesses.ContinentControlled(EnumContinent.SouthAmerica))
        {
            output += 2;
        }
        if (_gameBoardProcesses.ContinentControlled(EnumContinent.Austrailia))
        {
            output += 2;
        }
        return output;
    }
    public override async Task ContinueTurnAsync()
    {
        SingleInfo = PlayerList.GetWhoPlayer(); //try this way now.
        if (PlayerList.DidChooseColors() && SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
        {
            await EndTurnAsync(); //this means computer skips turn unless they need to choose colors.
            return;
        }
        if (PlayerList.DidChooseColors())
        {
            if ((SaveRoot.Stage == EnumStageList.Move || SaveRoot.Stage == EnumStageList.TransferAfterBattle) && SaveRoot.PreviousTerritory > 0 && SaveRoot.CurrentTerritory > 0)
            {
                _container.PopulateMoveArmies();
            }
            if (SaveRoot.Stage == EnumStageList.Place && SaveRoot.PreviousTerritory > 0)
            {
                _container.PopulatePlaceArmies();
            }
            _container.PopulateInstructions();
            SingleInfo!.ReinforcementsGained = 0; //i think.

            if (SaveRoot.Stage == EnumStageList.Begin && SingleInfo.MainHandList.Count < 3)
            {
                await ToNextStepAsync();
                return; //will do rest automatically.
            }
        }
        await base.ContinueTurnAsync();
    }
    public override Task MakeMoveAsync(int space)
    {
        throw new CustomBasicException("There is no move to make.  Too customized for this game");
    }
    public override async Task EndTurnAsync()
    {
        WhoTurn = await PlayerList!.CalculateWhoTurnAsync();
        if (PlayerList.DidChooseColors())
        {
            _model.PlayerHand1.EndTurn();
        }
        await StartNewTurnAsync();
    }
    private static async Task PopulateTerritoriesAsync()
    {
        await TerritoryHelpers.PopulateTerritoriesAsync();
    }
    public override async Task AfterChoosingColorsAsync()
    {
        if (BasicData.MultiPlayer && BasicData.Client)
        {
            Network!.IsEnabled = true;
            return; //has to wait for instructions from host.
        }
        if (MiscDelegates.FillRestColors == null)
        {
            throw new CustomBasicException("Nobody is handling filling the rest of the colors.  Rethink");
        }
        MiscDelegates.FillRestColors.Invoke();
        await PopulateTerritoriesAsync();
        SaveRoot.TerritoryList = PlayerList.GetTerritories();
        SaveRoot.ConqueredOne = false;
        SaveRoot.TerritoryList.PopulateTerritories(PlayerList); //this is the claiming processes.
        WhoTurn = WhoStarts;
        SingleInfo = PlayerList.GetWhoPlayer();
        SaveRoot.Stage = EnumStageList.Begin;
        _shuffle.SaveRoot = SaveRoot;
        await _shuffle.FirstShuffleAsync(false);
        if (BasicData!.MultiPlayer)
        {
            SaveRoot!.ImmediatelyStartTurn = true;
            await Network!.SendRestoreGameAsync(SaveRoot);
        }
        SetHand();
        await ContinueTurnAsync();
    }
    private void InitHand()
    {
        SingleInfo = PlayerList!.GetSelf();
        _model.PlayerHand1.HandList.ReplaceRange(SingleInfo.MainHandList);
    }
    public async Task ToNextStepAsync()
    {
        _gameBoardProcesses.ResetMove();
        _model.PlayerHand1.EndTurn();
        if (SaveRoot.Stage == EnumStageList.StartAttack)
        {
            SaveRoot.Stage = EnumStageList.Move;
        }
        else if (SaveRoot.Stage == EnumStageList.Move)
        {
            await EndMoveAsync();
        }
        else if (SaveRoot.Stage == EnumStageList.Begin)
        {
            ReenforcementProcesses();
        }
        else
        {
            throw new CustomBasicException("Only start attacks and moves can currently go to next step");
        }
        await ContinueTurnAsync();
    }
    private async Task EndMoveAsync()
    {
        SaveRoot.Stage = EnumStageList.EndTurn;
        if (SaveRoot.ConqueredOne)
        {
            await DrawAsync();
            return; //i think.
        }
        await ContinueTurnAsync();
    }
    public async Task ReturnRiskCardsAsync(BasicList<int> cards)
    {
        if (cards.Count is not 3)
        {
            throw new CustomBasicException("You must have 3 risk cards to turn in");
        }
        cards.ForEach(card =>
        {
            SingleInfo!.MainHandList.RemoveObjectByDeck(card); //hopefully this simple.
        });
        SaveRoot.SetsReturned++;
        SingleInfo!.ReinforcementsGained = _container.RiskCardsReturnedExtraReenforcements();
        SaveRoot.Stage = EnumStageList.Place;
        ReenforcementProcesses();
        await ContinueTurnAsync(); //hopefully this simple.
    }
    public async Task StartAttackAsync()
    {
        SaveRoot.Stage = EnumStageList.Roll;
        if (SaveRoot.PreviousTerritory == 0)
        {
            throw new CustomBasicException("Cannot start attack if there are no territories attacking from");
        }
        TerritoryModel territory = _container.GetTerritory(SaveRoot.PreviousTerritory);
        if (territory.Armies == 1)
        {
            throw new CustomBasicException("Must have at least 2 armies.  Of can't even attack since there is nothing left behind ");
        }
        int armies;
        if (territory.Armies >= 4)
        {
            armies = 3;
        }
        else
        {
            armies = territory.Armies - 1;
        }
        _container.PopulateAttackArmies(armies);
        await ContinueTurnAsync();
    }
}