namespace YaBlewIt.Core.Logic;
[SingletonGame]
public class YaBlewItMainGameClass
    : CardGameClass<YaBlewItCardInformation, YaBlewItPlayerItem, YaBlewItSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly YaBlewItVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly YaBlewItGameContainer _gameContainer; //if we don't need it, take it out.
    private bool _autoDrew;
    public StandardRollProcesses<EightSidedDice, YaBlewItPlayerItem> Roller;
    private readonly IChooseColorProcesses _colorProcesses;
    private readonly ComputerAI _ai;
    private readonly IToast _toast;
    public YaBlewItMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        YaBlewItVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<YaBlewItCardInformation> cardInfo,
        CommandContainer command,
        YaBlewItGameContainer gameContainer,
        StandardRollProcesses<EightSidedDice, YaBlewItPlayerItem> roller,
        IChooseColorProcesses colorProcesses,
        ComputerAI ai,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        Roller = roller;
        _colorProcesses = colorProcesses;
        _ai = ai;
        _toast = toast;
        Roller.AfterRollingAsync = AfterRollingAsync;
        Roller.CurrentPlayer = () => SingleInfo!;
        _gameContainer = gameContainer;
    }
    public int OtherTurn
    {
        get
        {
            return SaveRoot!.PlayOrder.OtherTurn;
        }
        set
        {
            SaveRoot!.PlayOrder.OtherTurn = value;
        }
    }
    public async Task AfterRollingAsync()
    {
        SaveRoot.PlayedFaulty = false; //if you rolled, then you can play the faulty again.
        if (SaveRoot.GameStatus == EnumGameStatus.MinerRolling)
        {
            BasicList<int> possibleList = _model.Claims.HandList.GetContainedNumbers();
            if (possibleList.Any(x => x == _model.Cup!.ValueOfOnlyDice) || Test!.DoubleCheck)
            {
                //double checking will show somebody as winning the claim (so i can figure out how the turn information got hosed).
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                {
                    _toast.ShowSuccessToast("You won the claim since the dice roll was contained in the claim");
                }
                else
                {
                    _toast.ShowWarningToast($"{SingleInfo.NickName} has won claim since the dice roll was contained in the claim");
                }
                await TakeClaimAsync(true);
                return;
            }
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _toast.ShowWarningToast("You failed to win the claim.  Therefore you are out of the current round unless you play the faulty detonator card to try again");
            }
            else
            {
                _toast.ShowInfoToast($"{SingleInfo.NickName} failed to win the claim.  Therefore; {SingleInfo.NickName} is out of the current round unless {SingleInfo.NickName} plays the faulty detonator card to try again");
            }
            SingleInfo.InGame = false;
            SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
            SaveRoot.PreviousStatus = EnumGameStatus.MinerRolling; //you may decide to roll again.

            await ContinueTurnAsync(); //this means you have a chance to play a card to get another roll.
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.FinishGambling ||
            SaveRoot.GameStatus == EnumGameStatus.StartGambling)
        {
            BasicList<int> badList = _model.Claims.HandList.GetContainedNumbers();
            if (badList.Any(x => x == _model.Cup!.ValueOfOnlyDice) == false)
            {
                if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
                {
                    _toast.ShowSuccessToast($"You won the claim by gambling by the dice roll not contained in the claim");
                }
                else
                {
                    _toast.ShowWarningToast($"{SingleInfo.NickName} has won the claim by gambling by the dice roll not contained in the claim");
                }
                await TakeClaimAsync(true);
                return;
            }
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _toast.ShowWarningToast("You failed to win the gambling claim since the dice roll was contained in the roll unless you play the faulty detonator card to try again");
            }
            else
            {
                _toast.ShowInfoToast($"{SingleInfo.NickName} failed to win the claim gambling claim unless {SingleInfo.NickName} plays the faulty detonator card to try again");
            }
            SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
            SaveRoot.PreviousStatus = EnumGameStatus.FinishGambling;
            await ContinueTurnAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ResolveFire)
        {
            BasicList<int> possibleList = SingleInfo!.MainHandList.GetContainedNumbers();
            if (possibleList.Count == 0)
            {
                await EndTurnAsync();
                return; //this is easy (because there was nothing to take.
            }
            if (possibleList.Any(x => x == _model.Cup!.ValueOfOnlyDice) == false)
            {
                foreach (var safe in SaveRoot.SafeList)
                {
                    YaBlewItCardInformation card = new();
                    card.Populate(safe);
                    SingleInfo.MainHandList.Add(card);
                }
                SaveRoot.SafeList.Clear();
                SaveRoot.ProtectedColors.Clear();
                SortCards();
                await EndTurnAsync();
                return; //because they lost nothing
            }
            //this means there are some possible losses.
            SaveRoot.GameStatus = EnumGameStatus.EndingTurn;
            SaveRoot.PreviousStatus = EnumGameStatus.ResolveFire;
            await ContinueTurnAsync(); //this means you have a chance to play a faulty one to roll again.
            return;
        }
        throw new CustomBasicException($"Needs to figure out dice rolling.  The status was {SaveRoot.GameStatus}");
    }
    public override Task PopulateSaveRootAsync()
    {
        SaveRoot.Claims = _model.Claims.HandList.GetDeckListFromObjectList();
        return base.PopulateSaveRootAsync();
    }
    public override Task FinishGetSavedAsync()
    {
        _model.Claims.ClearHand(); //try to clear this first because can be rejoining.
        SaveRoot.Claims.ForEach(deck =>
        {
            YaBlewItCardInformation card = new();
            card.Populate(deck);
            _model.Claims.HandList.Add(card);
        });
        _model!.LoadCup(SaveRoot, true);
        LoadControls();

        //anything else needed is here.
        return base.FinishGetSavedAsync();
    }
    protected override bool ShowNewCardDrawn(YaBlewItPlayerItem tempPlayer)
    {
        return false; //will not show as drawn period.
    }
    protected override Task AddCardAsync(YaBlewItCardInformation thisCard, YaBlewItPlayerItem tempPlayer)
    {
        thisCard.IsUnknown = false;
        _model.Claims.HandList.Add(thisCard); //try this.
        return Task.CompletedTask;
    }
    protected override async Task AfterDrawingAsync()
    {
        var card = _model.Claims.HandList.Last(); //i think.
        if (card.CardCategory == EnumCardCategory.Fire)
        {
            SaveRoot.GameStatus = EnumGameStatus.ResolveFire;
            if (SaveRoot.GameStatus == EnumGameStatus.StartGambling)
            {
                SaveRoot.OldestStatus = EnumGameStatus.StartGambling;
            }
            else
            {
                SaveRoot.OldestStatus = SaveRoot.PreviousStatus;
            }
            await base.AfterDrawingAsync();
            return;
        }
        if (card.CardCategory != EnumCardCategory.Gem)
        {
            await DrawAsync(); //has to keep drawing until you get a gem.
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ProspectorDraws && SaveRoot.PreviousStatus == EnumGameStatus.Beginning)
        {
            SaveRoot.GameStatus = EnumGameStatus.ProspectorStarts;
            SaveRoot.PreviousStatus = EnumGameStatus.None;
        }
        else if (SaveRoot.GameStatus == EnumGameStatus.ProspectorDraws && SaveRoot.PreviousStatus == EnumGameStatus.ProspectorContinues)
        {
            SaveRoot.GameStatus = EnumGameStatus.ProspectorStarts;
            SaveRoot.PreviousStatus = EnumGameStatus.None;
            await EndTurnAsync(); //try this way.
            return;
        }
        else if (SaveRoot.GameStatus == EnumGameStatus.ProspectorDraws && SaveRoot.PreviousStatus == EnumGameStatus.None)
        {
            SaveRoot.GameStatus = EnumGameStatus.ProspectorContinues;
        }
        else if (SaveRoot.GameStatus == EnumGameStatus.StartGambling && SaveRoot.DrewExtra)
        {
            SaveRoot.GameStatus = EnumGameStatus.FinishGambling;
        }
        else if (SaveRoot.GameStatus == EnumGameStatus.StartGambling && _autoDrew == false)
        {
            SaveRoot.DrewExtra = true; //this means you decided to draw extra and its completed.
        }
        await base.AfterDrawingAsync();
    }
    public override async Task ContinueTurnAsync()
    {
        _model.Instructions = ""; //to reset
        _autoDrew = false;
        if (OtherTurn > 0)
        {
            SingleInfo = PlayerList.GetOtherPlayer();
        }
        else
        {
            SingleInfo = PlayerList.GetWhoPlayer();
            if (SaveRoot.GameStatus == EnumGameStatus.MinerRolling)
            {
                throw new CustomBasicException("The prospector can never be a miner"); //best to show the error before it gets to this part (so autoresume won't fix).  otherwise, bug remains hidden.
            }
        }
        if (SaveRoot.GameStatus == EnumGameStatus.Beginning)
        {
            SaveRoot.DrewExtra = false;
            ResetMiners();
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ProspectorDraws)
        {
            _autoDrew = true;
            _model.Instructions = "Waiting for miner to draw";
            if (_model.Claims.HandList.Count == 0)
            {
                SaveRoot.PreviousStatus = EnumGameStatus.Beginning;
            }
            _command.UpdateAll();
            await Task.Delay(500);
            await DrawAsync();
            return; //will go back later.
        }
        if (OtherTurn > 0)
        {
            SingleInfo = PlayerList!.GetOtherPlayer();
            _model!.OtherLabel = SingleInfo.NickName;
        }
        else
        {
            SingleInfo = PlayerList!.GetWhoPlayer();
            _model!.OtherLabel = "None";
        }
        UpdateInstructions();
        await base.ContinueTurnAsync();
    }
    protected override void GetPlayerToContinueTurn()
    {
        //do nothing because that process was already done now.
    }
    private void UpdateInstructions()
    {
        if (SaveRoot.GameStatus == EnumGameStatus.ProspectorStarts)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.Instructions = "Decide to play claim jumper or end turn";
            }
            else
            {
                _model.Instructions = "Waiting for prospector to decide to take play jumper to take claim or end turn";
            }
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ProspectorContinues)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.Instructions = "Decide to take the claim or end turn";
            }
            else
            {
                _model.Instructions = "Waiting for prospector to decide to take claim or end turn";
            }
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.GamblingDecision)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.Instructions = "Decide whether to gamble or take claim";
            }
            else
            {
                _model.Instructions = "Waiting for prospector to decide whether to gamble or take claim";
            }
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.StartGambling && SaveRoot.DrewExtra == false)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.Instructions = "Decide whether to draw a second card or roll";
            }
            else
            {
                _model.Instructions = "Waiting for prospector to decide whether to draw a second card or roll";
            }
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.FinishGambling)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.Instructions = "Roll.  If any number matches the claim, you lose all of it.  Otherwise, you win the extra claim";
            }
            else
            {
                _model.Instructions = "Waiting for prospector to roll to figure out if the prospector wins the claim";
            }
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.EndingTurn)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.Instructions = "Either end turn or play faulty detonator to roll again";
            }
            else
            {
                _model.Instructions = $"Waiting for {SingleInfo.NickName} to decide to end turn or play fauly detonator to roll again";
            }
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.Safe)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.Instructions = "Choose color to protect";
            }
            else
            {
                _model.Instructions = $"Waiting for {SingleInfo.NickName} to choose color to protect";
            }
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.MinerRolling)
        {
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.Instructions = "Either roll to try to take the claim or pass.  If you roll, then if the dice rolls any number on the claim, you win.  Otherwise, you are out of the current round";
            }
            else
            {
                _model.Instructions = $"Waiting for {SingleInfo.NickName} to decide to roll to take the claim or pass";
            }
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ResolveFire)
        {
            string protectedColors = "";
            if (SaveRoot.ProtectedColors.Count == 1)
            {
                protectedColors = $"{SaveRoot.ProtectedColors.Single()} is protected.  So any numbers rolled of that color you still keep";
            }
            //there are only 4 safes total though.
            else if (SaveRoot.ProtectedColors.Count == 2)
            {
                protectedColors = $"{SaveRoot.ProtectedColors.First()} and {SaveRoot.ProtectedColors.Last()} are protected.  So any numbers rolled of those colors you still keep.";
            }
            else if (SaveRoot.ProtectedColors.Count == 3)
            {
                protectedColors = $"{SaveRoot.ProtectedColors.First()}, {SaveRoot.ProtectedColors[1]}, and {SaveRoot.ProtectedColors.Last()} are protected.  So any numbers rolled of those colors you still keep";
            }
            else if (SaveRoot.ProtectedColors.Count == 3)
            {
                protectedColors = $"{SaveRoot.ProtectedColors.First()}, {SaveRoot.ProtectedColors[1]}, {SaveRoot.ProtectedColors[2]}, and {SaveRoot.ProtectedColors.Last()} are protected.  So any numbers rolled of those colors you still keep";
            }
            if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self && protectedColors == "")
            {
                _model.Instructions = $"There is a fire to resolve.  Therefore, either choose a safe card to protect a color or roll.  You lose all cards of the value rolled.";
            }
            else if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _model.Instructions = $"There is a fire to resolve.  Therefore, either choose a safe card to protect a color or roll.  You lose all cards of the value rolled.  {protectedColors}";
            }
            else if (protectedColors == "")
            {
                _model.Instructions = $"There is a fire to resolve.  Waiting for {SingleInfo.NickName} to choose a safe card to protect a color or roll. {SingleInfo.NickName} will lose all the cards of the value rolled.";
            }
            else
            {
                _model.Instructions = $"There is a fire to resolve.  Waiting for {SingleInfo.NickName} to choose a safe card to protect a color or roll. {SingleInfo.NickName} will lose all the cards of the value rolled.  {protectedColors}";
            }
            return;
        }
        _model.Instructions = "Unknown";
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true; //i think needs to be here.
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        YaBlewItCardInformation last = _gameContainer.DeckList.GetSpecificItem(64); //will be fire.
        if (last.CardCategory != EnumCardCategory.Fire)
        {
            throw new CustomBasicException("The last card has to be fire");
        }
        _model.Deck1.ManuallyRemoveSpecificCard(last);
        _model.Deck1.AddCard(last);
        BasicList<YaBlewItCardInformation> faults = _gameContainer.DeckList.Where(x => x.CardCategory == EnumCardCategory.Faulty).ToBasicList();
        var list = EnumColors.ColorList.ToBasicList(); //i have to make copy of list.  otherwise, gets hosed later.
        list.ShuffleList();
        foreach (var player in PlayerList)
        {
            //CardInfo faulty = _gameContainer.de
            YaBlewItCardInformation current = faults.First();
            player.MainHandList.Add(current);
            player.CursedGem = list.First();
            list.RemoveFirstItem();
            _model.Deck1.ManuallyRemoveSpecificCard(current);
            faults.RemoveFirstItem();
        }
        SingleInfo = PlayerList.GetWhoPlayer();
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    protected override async Task ComputerTurnAsync()
    {
        if (SaveRoot.GameStatus == EnumGameStatus.MinerRolling)
        {
            await Task.Delay(500); //showing taking a half a second to decide to pass or roll.
            bool doPass = _ai.DoPass();
            if (doPass)
            {
                await PassAsync();
                return;
            }
            await Roller.RollDiceAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ProspectorStarts)
        {
            bool jumps = _ai.TakeFirstClaim();
            if (jumps)
            {
                int deck = SingleInfo!.MainHandList.First(x => x.CardCategory == EnumCardCategory.Jumper).Deck;
                await PlayCardAsync(deck);
                return;
            }
            await EndTurnAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ProspectorContinues)
        {
            bool takes = _ai.TakeClaim();
            if (takes == false)
            {
                await EndTurnAsync();
                return;
            }
            await TakeClaimAsync(false);
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.GamblingDecision)
        {
            await TakeClaimAsync(false); //the computer will never gamble.
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.EndingTurn)
        {
            int deck = _ai.FaultyCardPlayed();
            if (deck > 0)
            {
                await PlayCardAsync(deck);
                return;
            }
            await EndTurnAsync(); //for now, will not use any special card.  later can rethink
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ResolveFire)
        {
            int deck = _ai.SafeCardPlayed();
            if (deck > 0)
            {
                await PlayCardAsync(deck);
                return;
            }
            //for now, will go ahead and roll.  eventually the computer needs to be smarter.
            await Roller.RollDiceAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.Safe)
        {
            EnumColors colorChosen = _ai.SafeColorChosen();
            await _colorProcesses.ColorChosenAsync(colorChosen);
            return;
        }
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        if (IsLoaded == false)
        {
            _model!.LoadCup(SaveRoot, false);
            SaveRoot.DiceList.MainContainer = MainContainer;
        }
        foreach (var player in PlayerList)
        {
            player.MainHandList.Clear();
        }
        SaveRoot.GameStatus = EnumGameStatus.Beginning;
        SaveRoot.PreviousStatus = EnumGameStatus.None;
        SaveRoot.PlayedFaulty = false;
        SaveRoot.DrewExtra = false;
        LoadControls();
        return base.StartSetUpAsync(isBeginning);
    }
    public async Task GambleAsync()
    {
        if (SingleInfo!.CanSendMessage(BasicData))
        {
            await Network!.SendAllAsync(nameof(IMultiplayerModel.Gamble));
        }
        SaveRoot.GameStatus = EnumGameStatus.StartGambling;
        _autoDrew = true; //at first was auto.
        await DrawAsync();
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.
            case nameof(IMultiplayerModel.Pass):
                await PassAsync();
                break;
            case nameof(IMultiplayerModel.Played):
                await PlayCardAsync(int.Parse(content));
                break;
            case nameof(IMultiplayerModel.Gamble):
                //figure out gambling
                await GambleAsync();
                break;
            case nameof(IMultiplayerModel.TakeClaim):
                await TakeClaimAsync(false);
                //figure out taking claim.
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();

        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    private void ResetMiners()
    {
        if (SaveRoot.GameStatus != EnumGameStatus.Beginning)
        {
            throw new CustomBasicException("Only beginning game status should reset the miners");
        }
        foreach (var player in PlayerList)
        {
            if (player.NickName != SingleInfo!.NickName)
            {
                player.InGame = true; //has to use ingame so the processes can do this.
            }
            else
            {
                player.InGame = false;
            }
        }
        SaveRoot.GameStatus = EnumGameStatus.ProspectorDraws;
    }
    public async Task PassAsync()
    {
        if (SingleInfo!.CanSendMessage(BasicData))
        {
            await Network!.SendAllAsync(nameof(IMultiplayerModel.Pass));
        }
        await EndOtherTurnAsync();
    }
    private async Task EndOtherTurnAsync()
    {
        OtherTurn = await PlayerList.CalculateOtherTurnAsync();
        if (OtherTurn == 0)
        {
            if (PlayerList.All(x => x.InGame == false))
            {
                SaveRoot.GameStatus = EnumGameStatus.GamblingDecision; //has to decide if they will gamble.
            }
            else
            {
                SaveRoot.GameStatus = EnumGameStatus.ProspectorContinues; //hopefully won't cause another issue (?)
            }
            SaveRoot.PreviousStatus = EnumGameStatus.None; //i think
            SingleInfo = PlayerList.GetWhoPlayer(); //i think this too.
        }
        await ContinueTurnAsync(); //hopefully this simple.
    }
    public async Task PlayCardAsync(int deck)
    {
        if (SingleInfo!.CanSendMessage(BasicData))
        {
            await Network!.SendAllAsync(nameof(IMultiplayerModel.Played), deck);
        }
        YaBlewItCardInformation card = SingleInfo!.MainHandList.GetSpecificItem(deck);
        async Task PrivateDiscardAsync()
        {
            SingleInfo.MainHandList.RemoveSpecificItem(card);
            await DiscardAsync(card);
        }
        if (card.CardCategory == EnumCardCategory.Jumper)
        {
            //this means the prospector takes the claim immediately.  but then discards it.
            //discard first.

            await PrivateDiscardAsync();
            await TakeClaimAsync(true, false);
            //this means needs to draw again.
            SaveRoot.PreviousStatus = EnumGameStatus.None;
            SaveRoot.OldestStatus = EnumGameStatus.None;
            SaveRoot.GameStatus = EnumGameStatus.ProspectorDraws; //try this again.  if he wants to repeat, can.
            await ContinueTurnAsync();
            return;
        }
        if (card.CardCategory == EnumCardCategory.Faulty)
        {
            //this means a person gets another roll.
            SaveRoot.GameStatus = SaveRoot.PreviousStatus;
            SaveRoot.PlayedFaulty = true;
            await PrivateDiscardAsync();
            await ContinueTurnAsync();
            return; //try this way.  hopefully its this simple.
        }
        if (card.CardCategory == EnumCardCategory.Safe)
        {
            SaveRoot.GameStatus = EnumGameStatus.Safe;
            _model.ColorPicker.LoadEntireList(); //this is needed since the colors on the list can change.
            SaveRoot.SafeList.Add(deck); //the deck is all i need for this.
            SingleInfo.MainHandList.RemoveSpecificItem(card); //you for sure have to remove the item though.
            await ContinueTurnAsync(); //only discards if colors was protected and you rolled that color but decided to not get another roll (for fires).
            return;
        }
        throw new CustomBasicException("Not sure how to play other cards for now");
    }
    protected override Task AfterDiscardingAsync()
    {
        return Task.CompletedTask; //already taken care of (?)
    }
    public async Task TakeClaimAsync(bool automated, bool endTurn = true)
    {
        if (automated == false && SingleInfo!.CanSendMessage(BasicData))
        {
            await Network!.SendAllAsync(nameof(IMultiplayerModel.TakeClaim));
        }
        SingleInfo!.MainHandList.AddRange(_model.Claims.HandList);
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            SortCards();
        }
        _model.Claims.ClearHand();
        if (endTurn)
        {
            await EndTurnAsync();
        }
    }
    private async Task StartMiningAsync()
    {
        //ResetPlayers();
        _command.ManuelFinish = true;
        OtherTurn = await PlayerList.CalculateOtherTurnAsync(includeOutPlayers: false);
        SaveRoot.PreviousStatus = EnumGameStatus.None;
        SaveRoot.GameStatus = EnumGameStatus.MinerRolling;
        SaveRoot.PlayedFaulty = false;
        await StartNewTurnAsync(); //i think.
    }
    private void ResetPlayers()
    {
        foreach (var player in PlayerList)
        {
            player.InGame = true; //i think.  if wrong, rethink.
        }
    }
    private async Task NextMinerAsync()
    {
        ResetPlayers(); //hopefully okay here (?)
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        OtherTurn = 0; //i think this too (?)
        SaveRoot.PreviousStatus = EnumGameStatus.None;
        SaveRoot.PlayedFaulty = false;
        SaveRoot.GameStatus = EnumGameStatus.Beginning; //i think.
        await StartNewTurnAsync();
    }
    private async Task RemoveClaimAsync()
    {
        //this means its completely lost (all discarded).
        var list = _model.Claims.HandList.ToBasicList();
        foreach (var item in list)
        {
            _model.Claims.HandList.RemoveSpecificItem(item);
            await DiscardAsync(item);
        }
    }
    private async Task FinishFireLossesAsync()
    {
        if (SaveRoot.ProtectedColors.Count != SaveRoot.SafeList.Count)
        {
            throw new CustomBasicException("The protected colors don't match the safe cards");
        }
        //if (SaveRoot.ProtectedColors.Count > 0)
        //{
        //    _toast.ShowInfoToast("Needs to figure out when colors are protected");
        //    return;
        //}
        int value = _model.Cup!.ValueOfOnlyDice;
        var list = SingleInfo!.MainHandList.Where(x => x.FirstNumber == value || x.SecondNumber == value).ToBasicList();
        if (list.Count == 0)
        {
            throw new CustomBasicException("Should not had to finish fire here if there was nothing found to lose");
        }
        HashSet<EnumColors> fulfillList = new();
        foreach (var item in list)
        {
            if (SaveRoot.ProtectedColors.Contains(item.CardColor))
            {
                fulfillList.Add(item.CardColor);
            }
            else
            {
                SingleInfo.MainHandList.RemoveSpecificItem(item);
                await DiscardAsync(item);
            }
        }
        YaBlewItCardInformation safe;
        int deck;
        foreach (var item in fulfillList)
        {
            deck = SaveRoot.SafeList.First();
            safe = new();
            safe.Populate(deck);
            //since it was fulfilled, then you will lose it.
            await DiscardAsync(safe);
            SaveRoot.SafeList.RemoveFirstItem();
        }
        foreach (var item in SaveRoot.SafeList)
        {
            deck = item;
            safe = new();
            safe.Populate(item);
            SingleInfo.MainHandList.Add(safe);
        }
        SortCards(); //i think (hopefully this simple).
        SaveRoot.PreviousStatus = EnumGameStatus.None;
        SaveRoot.GameStatus = EnumGameStatus.ResolveFire;
        SaveRoot.ProtectedColors.Clear(); //now has to clear the protected list because the turn is completely over now.
        SaveRoot.SafeList.Clear();
        await EndTurnAsync();
    }
    private async Task FinishFireForPlayerAsync()
    {
        OtherTurn = await PlayerList.CalculateOtherTurnAsync();
        if (OtherTurn == 0)
        {
            await CompletelyFinshedResolvingFireAsync();
            return;
        }
        await ContinueTurnAsync(); //well see if its this simple.
    }
    private async Task CompletelyFinshedResolvingFireAsync()
    {
        var card = _model.Claims.HandList.Last();
        if (card.CardCategory != EnumCardCategory.Fire)
        {
            throw new CustomBasicException("Only fire cards can be removed when resolving fires");
        }
        _model.Claims.HandList.RemoveLastItem(); //i think.
        await DiscardAsync(card);
        if (_model.Deck1.IsEndOfDeck())
        {
            await ProcessEndGameAsync();
            return; //this means its the end of the game.
        }
        if (SaveRoot.OldestStatus == EnumGameStatus.StartGambling)
        {
            SaveRoot.GameStatus = EnumGameStatus.StartGambling;
            SaveRoot.PreviousStatus = EnumGameStatus.None;
        }
        else
        {
            SaveRoot.GameStatus = EnumGameStatus.ProspectorDraws;
            SaveRoot.PreviousStatus = SaveRoot.OldestStatus;
        }
        SaveRoot.OldestStatus = EnumGameStatus.None;
        await DrawAsync();
    }
    private async Task ProcessEndGameAsync()
    {
        //at this point, whoever whoturn is has to take the final claim.
        if (_model.Claims.HandList.Count > 0)
        {
            SingleInfo = PlayerList.GetWhoPlayer();
            await TakeClaimAsync(true, false); //whoever is the prospector has to take the claim now.
        }
        SingleInfo = PlayerList.OrderByDescending(x => x.TotalScore).First(); //i think
        await ShowWinAsync();
    }
    public override async Task EndTurnAsync()
    {
        //can't always set to to whoturn anymore.
        //SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo!.MainHandList.UnhighlightObjects(); //i think this is best.
        SaveRoot.PlayedFaulty = false; //i think.
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        if (SaveRoot.GameStatus == EnumGameStatus.EndingTurn && SaveRoot.PreviousStatus == EnumGameStatus.MinerRolling)
        {
            await EndOtherTurnAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.EndingTurn && SaveRoot.PreviousStatus == EnumGameStatus.ResolveFire)
        {
            await FinishFireLossesAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ResolveFire)
        {
            await FinishFireForPlayerAsync();
            return;
        }
        //if (SaveRoot.GameStatus == EnumGameStatus.EndingTurn && SaveRoot.PreviousStatus == EnumGameStatus.None)
        //{
        //    await StartMiningAsync();
        //    return;
        //}
        if (SaveRoot.GameStatus == EnumGameStatus.EndingTurn && SaveRoot.PreviousStatus == EnumGameStatus.FinishGambling)
        {
            //this means the claim is completely lost.
            await RemoveClaimAsync();
            await NextMinerAsync();
            return;
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ProspectorStarts)
        {
            await StartMiningAsync();
            return;
        }
        //if the claims has none, then needs to go to the next minor (otherwise taking claim allows extra turns).  if that is wrong, rethink.
        if (_model.Claims.HandList.Count == 0)
        {
            await NextMinerAsync();
            return; //try this (?)
        }
        if (SaveRoot.GameStatus == EnumGameStatus.ProspectorContinues && SaveRoot.PreviousStatus == EnumGameStatus.None)
        {
            SaveRoot.GameStatus = EnumGameStatus.ProspectorDraws;
            SaveRoot.PreviousStatus = EnumGameStatus.ProspectorContinues; //since that is the previous state, then 
            await ContinueTurnAsync();
            return;
        }

        throw new CustomBasicException("Not sure what to do about ending turn in this stage");
    }
}