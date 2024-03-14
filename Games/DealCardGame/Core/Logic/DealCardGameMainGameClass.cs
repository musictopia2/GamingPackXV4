namespace DealCardGame.Core.Logic;
[SingletonGame]
public class DealCardGameMainGameClass
    : CardGameClass<DealCardGameCardInformation, DealCardGamePlayerItem, DealCardGameSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly DealCardGameVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly DealCardGameGameContainer _gameContainer; //if we don't need it, take it out.
    private readonly IToast _toast;
    private readonly PrivateAutoResumeProcesses _privateAutoResume;
#pragma warning disable IDE0290 // Use primary constructor
    public DealCardGameMainGameClass(IGamePackageResolver mainContainer,
#pragma warning restore IDE0290 // Use primary constructor
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        DealCardGameVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<DealCardGameCardInformation> cardInfo,
        CommandContainer command,
        DealCardGameGameContainer gameContainer,
        ISystemError error,
        IToast toast,
        PrivateAutoResumeProcesses privateAutoResume
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
        _privateAutoResume = privateAutoResume;
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
    private bool _fromAutoResume;
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        _fromAutoResume = true;
        bool rets = await _privateAutoResume.HasAutoResumeAsync();
        if (rets)
        {
            await _privateAutoResume.RestoreStateAsync(_gameContainer);
        }
        if (SaveRoot.GameStatus != EnumGameStatus.NeedsPayment)
        {
            _gameContainer.PersonalInformation.NeedsPayment = false;
            _gameContainer.PersonalInformation.State = new();
            _gameContainer.PersonalInformation.Payments.Clear(); //clear those things out.
            await _privateAutoResume.SaveStateAsync(_gameContainer);
        }
        else if (rets == false)
        {
            //redo the state again to show at the beginning.
            await StartPaymentProcessesForSelectedPlayerAsync();
        }
        if (SaveRoot.GameStatus == EnumGameStatus.StartDebtCollector)
        {
            LoadPlayerPicker(); //i think.
        }
        GetPlayerToContinueTurn();
        _gameContainer.IsJustSayNoSelf = SingleInfo!.PlayerCategory == EnumPlayerCategory.Self;
        //anything else needed is here.
        await base.FinishGetSavedAsync();
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
        //if there is nothing, then just won't do anything.
        await Task.CompletedTask;
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        foreach (var player in PlayerList)
        {
            var list = EnumColor.ColorList;
            player.SetData.Clear();
            foreach (var item in list)
            {
                SetPropertiesModel model = new()
                {
                    Color = item
                };
                player.SetData.Add(model);
            }
        }
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        return base.StartSetUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        SetCardModel setCard;
        //DealCardGameCardInformation actionCard;
        //DealCardGamePlayerItem currentPlayer;
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.
            case "playaction":
                await PlayActionAsync(int.Parse(content));
                return;
            case "bank":
                await BankAsync(int.Parse(content));
                return;
            case "playproperty":
                setCard = await js1.DeserializeObjectAsync<SetCardModel>(content);
                await PlayPropertyAsync(setCard.Deck, setCard.Color);
                return;
            case "playerpayment":
                await SelectSinglePlayerForPaymentAsync(int.Parse(content), 5); //hopefully always 5 for this.
                return;
            case "resume":
                await ResumeAsync();
                return;
            case "finishpayment":
                BasicList<int> payments = await js1.DeserializeObjectAsync<BasicList<int>>(content);
                await ProcessPaymentsAsync(payments);
                return;
            case "stealset":
                StealSetModel stealSet = await js1.DeserializeObjectAsync<StealSetModel>(content);
                await StartToStealSetAsync(stealSet.Deck, stealSet.PlayerId, stealSet.Color);
                return;
            case "rentrequest":
                RentModel rent = await js1.DeserializeObjectAsync<RentModel>(content);
                await PossibleRentRequestAsync(rent);
                return;
            case "stealproperty":
                StealPropertyModel stealProperty = await js1.DeserializeObjectAsync<StealPropertyModel>(content);
                await PossibleStealingPropertyAsync(stealProperty);
                return;
            case "playerchosenfordebt":
                await ChosePlayerForDebtAsync(int.Parse(content));
                return;
            case "trade":
                TradePropertyModel tradeProperty = await js1.DeserializeObjectAsync<TradePropertyModel>(content);
                await PossibleTradingPropertyAsync(tradeProperty);
                return;
            case "accept":
                //should have the information to finish up.
                await ProcessAcceptanceAsync();
                return;
            case "reject":
                await ProcessRejectionAsync();
                return;
            default:
                _toast.ShowUserErrorToast($"Nothing for status {status}");
                return;
                //throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    private int GetNextPlayerToDecideBasedOnAcceptance()
    {
        GetPlayerToContinueTurn();
        SingleInfo!.AllPlayerStatus = EnumAllPlayerStatus.Accept; //this means they can't decide again.
        return GetNextPlayerToDecidePeriod();
        
    }
    private int GetNextPlayerToDecideBasedOnRejection()
    {
        GetPlayerToContinueTurn();
        SingleInfo!.AllPlayerStatus = EnumAllPlayerStatus.Reject;
        return GetNextPlayerToDecidePeriod();
    }
    private int GetNextPlayerToDecidePeriod()
    {
        if (PlayerList.Any(x => x.AllPlayerStatus == EnumAllPlayerStatus.NeedsToDecide))
        {
            DealCardGamePlayerItem player = PlayerList.First(x => x.AllPlayerStatus == EnumAllPlayerStatus.NeedsToDecide);
            _gameContainer.IsJustSayNoSelf = player!.PlayerCategory == EnumPlayerCategory.Self;
            SaveRoot.PlayerUsedAgainst = player.Id;
            OtherTurn = player.Id;
            SaveRoot.GameStatus = EnumGameStatus.ConsiderJustSayNo;
            GetPlayerToContinueTurn();
            return player.Id;
        }
        OtherTurn = 0;
        GetPlayerToContinueTurn(); //i think.
        return 0; //because there is nobody who needs to decide.
    }
    public async Task ProcessAcceptanceAsync()
    {
        DealCardGameCardInformation actionCard;
        if (SaveRoot.ActionCardUsed == 0)
        {
            _toast.ShowUserErrorToast("No action card.  Rethink");
            return;
        }
        actionCard = _gameContainer.DeckList.GetSpecificItem(SaveRoot.ActionCardUsed);
        
        if (OtherTurn == 0)
        {
            ClearJustSayNo();
            //this means you are accepting you cannot play your card.
            await ContinueTurnAsync();
            return; //no need to let anybody know in this case.
        }
        int nextPlayer;
        if (actionCard.ActionCategory == EnumActionCategory.Birthday)
        {
            nextPlayer = GetNextPlayerToDecideBasedOnAcceptance();
            if (nextPlayer == 0)
            {
                ClearJustSayNo();
                await StartFiguringOutPaymentsForAllPlayersAsync(2); //because its birthday.
                return;
            }
            await ContinueTurnAsync();
            return;
        }
        if (actionCard.CardType == EnumCardType.ActionRent && actionCard.AnyColor == false)
        {
            nextPlayer = GetNextPlayerToDecideBasedOnAcceptance();
            if (nextPlayer == 0)
            {
                int owed = SaveRoot.PaymentOwed;
                ClearJustSayNo();
                await StartFiguringOutPaymentsForAllPlayersAsync(owed);
                return;
            }
            await ContinueTurnAsync();
            return;
        }
        await CompleteRealActionAsync(actionCard);
    }
    private async Task CompleteRealActionAsync(DealCardGameCardInformation actionCard)
    {
        OtherTurn = 0; //for sure no matter what.
        int player = SaveRoot.PlayerUsedAgainst;
        GetPlayerToContinueTurn(); //try this just in case.
        EnumColor opponentColor = SaveRoot.OpponentColorChosen;
        if (actionCard.ActionCategory == EnumActionCategory.DealBreaker)
        {
            ClearJustSayNo();
            await FinishStealingSetAsync(player, opponentColor);
            return;
        }
        if (actionCard.ActionCategory == EnumActionCategory.SlyDeal)
        {
            //this means needs to capture the information needed in order to steal.
            StealPropertyModel steal = new()
            {
                CardChosen = SaveRoot.CardStolen,
                CardPlayed = actionCard.Deck,
                Color = opponentColor,
                PlayerId = player,
                StartStealing = false
            };
            ClearJustSayNo();
            await FinishStealingPropertyAsync(steal);
            return;
        }
        if (actionCard.ActionCategory == EnumActionCategory.ForcedDeal)
        {
            TradePropertyModel trade = new()
            {
                CardPlayed = actionCard.Deck,
                YourCard = SaveRoot.YourTrade,
                OpponentCard = SaveRoot.OpponentTrade,
                PlayerId = player,
                StartTrading = false,
                YourColor = SaveRoot.YourColorChosen,
                OpponentColor = SaveRoot.OpponentColorChosen
            };
            ClearJustSayNo();
            await FinishTradingPropertyAsync(trade);
            return;
        }
        if (actionCard.ActionCategory == EnumActionCategory.DebtCollector)
        {
            ClearJustSayNo();
            await SelectSinglePlayerForPaymentAsync(player, 5);
            return;
        }
        if (actionCard.CardType == EnumCardType.ActionRent && actionCard.AnyColor)
        {
            int amountOwed = SaveRoot.PaymentOwed;
            await SelectSinglePlayerForPaymentAsync(player, amountOwed);
            return;
        }
        _toast.ShowUserErrorToast("Not supported yet");
        return;
    }
    private void ClearJustSayNo()
    {
        SaveRoot.GameStatus = EnumGameStatus.None;
        SaveRoot.ActionCardUsed = 0;
        SaveRoot.PlayerUsedAgainst = 0;
        SaveRoot.OpponentColorChosen = EnumColor.None;
        SaveRoot.YourColorChosen = EnumColor.None;
        SaveRoot.OpponentTrade = 0;
        SaveRoot.YourTrade = 0;
    }
    private int GetPlayerToDecideAgain()
    {
        //if it returns 0, then anybody who rejects must now accept.
        
        PlayerList.ForConditionalItems(x => x.AllPlayerStatus == EnumAllPlayerStatus.Reject, player =>
        {
            if (player.MainHandList.Any(x => x.ActionCategory == EnumActionCategory.JustSayNo))
            {
                player.AllPlayerStatus = EnumAllPlayerStatus.NeedsToDecide;
            }
            else
            {
                player.AllPlayerStatus = EnumAllPlayerStatus.Accept; //because its been countered and you have no more counteres
            }
        });
        var player = PlayerList.SingleOrDefault(x => x.AllPlayerStatus == EnumAllPlayerStatus.NeedsToDecide);
        if (player is null)
        {
            return 0;
        }

        return player.Id;
    }
    private async Task AdvancedRejectionAsync(DealCardGameCardInformation card)
    {
        //bool wasSelf;
        if (OtherTurn == 0)
        {
            //this means its back to me.
            //i rejected it too.
            int otherId = GetPlayerToDecideAgain();
            if (otherId > 0)
            {
                OtherTurn = otherId;
                SaveRoot.PlayerUsedAgainst = OtherTurn;
                SaveRoot.GameStatus = EnumGameStatus.ConsiderJustSayNo;
                await ContinueTurnAsync();
                return;
            }
        }
        //if (OtherTurn > 0)
        //{
        //    wasSelf = true;
        //    OtherTurn = 0;
        //}
        //else
        //{
        //    wasSelf = false;
        //    OtherTurn = SaveRoot.PlayerUsedAgainst;
        //    //if (card.ActionCategory == EnumActionCategory.bir)
        //}

        int nextPlayer;
        nextPlayer = GetNextPlayerToDecideBasedOnRejection();
        if (nextPlayer == 0)
        {
            //can check to see if you can counter it.
            OtherTurn = 0;
            GetPlayerToContinueTurn();
            if (SingleInfo!.MainHandList.Any(x => x.ActionCategory == EnumActionCategory.JustSayNo))
            {
                await ContinueTurnAsync();
                return;
            }
            if (card.ActionCategory == EnumActionCategory.Birthday)
            {
                ClearJustSayNo();
                await StartFiguringOutPaymentsForAllPlayersAsync(2); //because its birthday.
                return;
            }
            if (card.CardType == EnumCardType.ActionRent && card.AnyColor == false)
            {
                int owed = SaveRoot.PaymentOwed;
                ClearJustSayNo();
                await StartFiguringOutPaymentsForAllPlayersAsync(owed);
                return;
            }
            _toast.ShowUserErrorToast("Unable to figure out advanced rejection");
            return;
        }
        SaveRoot.GameStatus = EnumGameStatus.ConsiderJustSayNo;
        await ContinueTurnAsync();
    }
    public async Task ProcessRejectionAsync()
    {
        GetPlayerToContinueTurn();
        var card = SingleInfo!.MainHandList.First(x => x.ActionCategory == EnumActionCategory.JustSayNo);
        SingleInfo.MainHandList.RemoveSpecificItem(card); //its played period.
        await AnimatePlayAsync(card);
        if (card.ActionCategory == EnumActionCategory.Birthday || card.CardType == EnumCardType.ActionRent && card.AnyColor == false)
        {
            await AdvancedRejectionAsync(card);
            return;
        }
        bool wasSelf;
        
        if (OtherTurn > 0)
        {
            wasSelf = true;
            OtherTurn = 0;
        }
        else
        {
            wasSelf = false;
            OtherTurn = SaveRoot.PlayerUsedAgainst;
            //if (card.ActionCategory == EnumActionCategory.bir)
        }
        GetPlayerToContinueTurn();
        if (SingleInfo.MainHandList.Any(x => x.ActionCategory == EnumActionCategory.JustSayNo))
        {
            SaveRoot.GameStatus = EnumGameStatus.ConsiderJustSayNo;
            _gameContainer.IsJustSayNoSelf = SingleInfo!.PlayerCategory == EnumPlayerCategory.Self;
            await ContinueTurnAsync();
            return;
        }
        SaveRoot.GameStatus = EnumGameStatus.None;
        if (wasSelf)
        {
            //since you don't have anything to counter, then just continue turn.
            await CancelActionAsync();
            return;
        }
        else
        {
            DealCardGameCardInformation actionCard = _gameContainer.DeckList.GetSpecificItem(SaveRoot.ActionCardUsed);
            await CompleteRealActionAsync(actionCard);
        }
    }
    private async Task CancelActionAsync()
    {
        DealCardGameCardInformation action = _gameContainer.DeckList.GetSpecificItem(SaveRoot.ActionCardUsed);
        if (action.ActionCategory == EnumActionCategory.DealBreaker)
        {
            //this is the easiest.
            SaveRoot.GameStatus = EnumGameStatus.None;
            OtherTurn = 0;
            SaveRoot.OpponentColorChosen = EnumColor.None;
            SaveRoot.PlayerUsedAgainst = 0;
            await ContinueTurnAsync(); //because nothing else.
            return;
        }
        if (action.ActionCategory == EnumActionCategory.SlyDeal)
        {
            SaveRoot.GameStatus = EnumGameStatus.None;
            OtherTurn = 0;
            SaveRoot.OpponentColorChosen = EnumColor.None;
            SaveRoot.CardStolen = 0;
            SaveRoot.PlayerUsedAgainst = 0;
            await ContinueTurnAsync();
            return;
        }
        if (action.ActionCategory == EnumActionCategory.ForcedDeal)
        {
            SaveRoot.GameStatus = EnumGameStatus.None;
            OtherTurn = 0;
            SaveRoot.OpponentTrade = 0;
            SaveRoot.YourTrade = 0;
            SaveRoot.YourColorChosen = EnumColor.None;
            SaveRoot.PlayerUsedAgainst = 0;
            SaveRoot.OpponentColorChosen = EnumColor.None;
            await ContinueTurnAsync();
            return;
        }
        if (action.ActionCategory == EnumActionCategory.DebtCollector || action.ActionCategory == EnumActionCategory.Birthday)
        {
            SaveRoot.GameStatus = EnumGameStatus.None;
            OtherTurn = 0;
            SaveRoot.PaymentOwed = 0;
            await ContinueTurnAsync();
            return;
        }
        if (action.CardType == EnumCardType.ActionRent && action.AnyColor)
        {
            SaveRoot.GameStatus = EnumGameStatus.None;
            OtherTurn = 0; //no problem to repeat for now.
            SaveRoot.PaymentOwed = 0;
            await ContinueTurnAsync();
            return;
        }
        _toast.ShowUserErrorToast("Has to figure out how to cancel the action for other cases for now");
    }
    protected override void GetPlayerToContinueTurn()
    {
        if (OtherTurn == 0)
        {
            base.GetPlayerToContinueTurn();
            return;
        }
        SingleInfo = PlayerList.GetOtherPlayer();
    }
    public override async Task ContinueTurnAsync()
    {
        if (_fromAutoResume == false)
        {
            await base.ContinueTurnAsync();
        }
        if (SaveRoot.GameStatus != EnumGameStatus.NeedsPayment)
        {
            await base.ContinueTurnAsync();
            return;
        }
        var player = PlayerList.GetOtherPlayer();
        if (_gameContainer.PersonalInformation.NeedsPayment == false && player.PlayerCategory == EnumPlayerCategory.Self)
        {
            //this means you already paid.  but needs to send the information to other players.
            BasicList<int> cards = _gameContainer.PersonalInformation.Payments.GetDeckListFromObjectList();
            await ProcessPaymentsAsync(cards);
            return;
        }
        await base.ContinueTurnAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        _fromAutoResume = false; //no longer from autoresume.
        await DrawToStartAsync();
        //await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    private async Task DrawToStartAsync()
    {
        GetPlayerToContinueTurn();
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Computer)
        {
            await ContinueTurnAsync();
            return;
        }
        if (SingleInfo!.MainHandList.Count == 0)
        {
            LeftToDraw = 5;
        }
        else
        {
            LeftToDraw = 2;
        }
        PlayerDraws = WhoTurn;
        await DrawAsync(); //hopefully that after drawing, will continueturn (?)
    }
    public async Task PlayActionAsync(int deck)
    {
        var card = GetPlayerSelectedSingleCard(deck);
        if (card.ActionCategory == EnumActionCategory.Gos)
        {
            await PlayPassGoAsync(card);
            return;
        }
        if (card.ActionCategory == EnumActionCategory.Birthday)
        {
            await StartBirthdayAsync(card);
            return;
        }
        if (card.ActionCategory == EnumActionCategory.DebtCollector)
        {
            await StartDebtCollectorAsync(card);
            return;
        }
    }
    private void StartPossiblePaymentProcesses()
    {
        _gameContainer.PersonalInformation.Payments.Clear();
        _gameContainer.PersonalInformation.State = new(); //clear out.
    }
    private async Task StartDebtCollectorAsync(DealCardGameCardInformation card)
    {
        await AnimatePlayAsync(card);
        StartPossiblePaymentProcesses();
        if (PlayerList.Count > 2)
        {
            SaveRoot.ActionCardUsed = card.Deck;
            SaveRoot.GameStatus = EnumGameStatus.StartDebtCollector;
            LoadPlayerPicker();
            await ContinueTurnAsync();
            return;
        }
        var player = PlayerList.Single(x => x.Id != WhoTurn);
        if (player.MainHandList.Any(x => x.ActionCategory == EnumActionCategory.JustSayNo) && player.Money > 0)
        {
            UpdateToJustSayNo(player, card.Deck);
            await ContinueTurnAsync();
            return;
        }
        await SelectSinglePlayerForPaymentAsync(player.Id, 5);
        //await StartFiguringOutPaymentsForAllPlayersAsync(5);
    }
    public async Task ChosePlayerForDebtAsync(int id)
    {
        var player = PlayerList.Single(x => x.Id == id);
        if (player.MainHandList.Any(x => x.ActionCategory == EnumActionCategory.JustSayNo))
        {
            UpdateToJustSayNo(player, SaveRoot.ActionCardUsed);
            await ContinueTurnAsync(); //cannot choose a player yet for payment because don't know if they will accept.
            return;
        }
        SaveRoot.GameStatus = EnumGameStatus.None;
        await SelectSinglePlayerForPaymentAsync(id, 5);
    }
    public void LoadPlayerPicker()
    {
        var list = PlayerList.AllPlayersExceptForCurrent().Select(x => x.NickName).ToBasicList();
        _model.PlayerPicker.SelectedIndex = 0; //because its one based this time.
        _model.PlayerPicker.LoadTextList(list);
    }
    private BasicList<DealCardGamePlayerItem> GetAllPlayersForPossibleJustSayNo()
    {
        var output = PlayerList.Where(x => x.Id != WhoTurn && x.MainHandList.Any(y => y.ActionCategory == EnumActionCategory.JustSayNo) && x.Money > 0).ToBasicList();
        if (output.Count == 0)
        {
            PlayerList.ForEach(x => x.AllPlayerStatus = EnumAllPlayerStatus.Accept);
            var fins = PlayerList.GetWhoPlayer();
            fins.AllPlayerStatus = EnumAllPlayerStatus.None;
            return output;
        }
        output.ForEach(player => player.AllPlayerStatus = EnumAllPlayerStatus.NeedsToDecide);
        PlayerList.ForConditionalItems(x => x.AllPlayerStatus != EnumAllPlayerStatus.NeedsToDecide, player =>
        {
            if (player.Id == WhoTurn)
            {
                player.AllPlayerStatus = EnumAllPlayerStatus.None;
            }
            else
            {
                player.AllPlayerStatus = EnumAllPlayerStatus.Accept; //just accept it anyways (will process and do nothing).
            }
        });
        return output;
    }
    private async Task StartBirthdayAsync(DealCardGameCardInformation card)
    {
        await AnimatePlayAsync(card);
        StartPossiblePaymentProcesses();
        var list = GetAllPlayersForPossibleJustSayNo();
        
        //needs to stick to a pattern.
        
        if (list.Count == 0)
        {
            await StartFiguringOutPaymentsForAllPlayersAsync(2);
            return;
        }
        //hopefully no need for another variable based on the action card.
        //obvious based on action.
        //SaveRoot.PaymentOwed = amountOwed;
        UpdateToJustSayNo(list.First(), card.Deck);
        await ContinueTurnAsync();

        //all players has to have a turn to be able to just say no.
        //before the main player can counter.
        //if the main player counters, then if nobody has anything else, move on.
        //however, if they accept, then needs to know the acceptance.

        //information needed:
        //1.  the id of the player
        //2.  status (None, NeedsToDecide, Accept, Rejected)

        //if they accept, then easy.
        //if its none, then easy
        //if its needstodecide, then waiting for them.
        //if its rejected, then if main player rejects, then, if the other player does not have another one, then goes to accept


        //await StartFiguringOutPaymentsForAllPlayersAsync(2);
    }
    public async Task SelectSinglePlayerForPaymentAsync(int player, int owed)
    {
        OtherTurn = player;
        SingleInfo = PlayerList.GetOtherPlayer();
        SingleInfo.Debt = owed; //i think has to be here.
        _model.ChosenPlayer = SingleInfo.NickName;
        _command.UpdateAll();
        await Delay!.DelayMilli(700);
        _model.ChosenPlayer = "";
        AttemptToAutomatePayment(SingleInfo, owed);
        if (SingleInfo.Debt == 0 && SingleInfo.Payments.Count == 0)
        {
            SaveRoot.GameStatus = EnumGameStatus.None; //i think.
            OtherTurn = 0; //because nobody owes anything.
            await ContinueTurnAsync();
            return;
        }
        if (SingleInfo.Payments.Count > 0)
        {
            await AfterPaymentsAsync();
            return;
        }
        await StartPaymentProcessesForSelectedPlayerAsync();
        await ContinueTurnAsync(); //i think.
    }
    private async Task StartFiguringOutPaymentsForAllPlayersAsync(int owed)
    {
        var list = PlayerList.Where(x => x.AllPlayerStatus == EnumAllPlayerStatus.Accept);

        foreach (var player in list)
        {
            if (player.Id != WhoTurn)
            {
                if (player.Money <= owed)
                {
                    player.Debt = player.Money;
                }
                else
                {
                    player.Debt = owed;
                }
            }
        }
        foreach (var player in list)
        {
            AttemptToAutomatePayment(player, owed);
        }
        if (list.All(x => x.Debt == 0 && x.Payments.Count == 0))
        {
            SaveRoot.GameStatus = EnumGameStatus.None; //i think.
            await ContinueTurnAsync();
            return;
        }
        if (list.Any(x => x.Debt > 0))
        {
            await StartPaymentProcessesForSelectedPlayerAsync();
            await ContinueTurnAsync(); //i think.
            return;
        }
        await AfterPaymentsAsync();
    }
    private async Task StartPaymentProcessesForSelectedPlayerAsync()
    {
        SingleInfo = PlayerList.First(x => x.Debt > 0 && x.Payments.Count == 0);
        SaveRoot.GameStatus = EnumGameStatus.NeedsPayment;
        await RecordPersonalStartPaymentProcessesAsync();
        OtherTurn = SingleInfo.Id;
    }
    private async Task RecordPersonalStartPaymentProcessesAsync()
    {
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _gameContainer.PersonalInformation.NeedsPayment = true;
        }
        else
        {
            _gameContainer.PersonalInformation.NeedsPayment = false; //i think.
        }
        _gameContainer.PersonalInformation.Payments.Clear();
        SingleInfo.ClonePlayerProperties(_gameContainer.PersonalInformation);
        _gameContainer.PersonalInformation.State.BankedCards = SingleInfo.BankedCards.ToRegularDeckDict();
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    private static void AttemptToAutomatePayment(DealCardGamePlayerItem currentPlayer, int owed)
    {
        if (currentPlayer.Debt == 0)
        {
            currentPlayer.AllPlayerStatus = EnumAllPlayerStatus.None; //back to none.
            return;
        }
        if (currentPlayer.Money <= owed && currentPlayer.Money > 0)
        {
            currentPlayer.Debt = 0;
            if (currentPlayer.Money == 0)
            {
                currentPlayer.AllPlayerStatus = EnumAllPlayerStatus.None; //because they did not have any.
            }
            currentPlayer.Payments.Clear();
            foreach (var item in currentPlayer.BankedCards)
            {
                currentPlayer.Payments.Add(item.Deck);
            }
            foreach (var item in currentPlayer.SetData)
            {
                var list = item.Cards.ToRegularDeckDict();
                list.RemoveAllOnly(x => x.ClaimedValue == 0);
                foreach (var card in list)
                {
                    currentPlayer.Payments.Add(card.Deck);
                }
            }
        }
    }
    public async Task PlayPropertyAsync(int deck, EnumColor color)
    {
        var card = GetPlayerSelectedSingleCard(deck);
        if (card.CardType == EnumCardType.PropertyWild)
        {
            card.MainColor = color;
        }
        SingleInfo!.AddSingleCardToPlayerPropertySet(card, color);
        SingleInfo!.Money += card.ClaimedValue; //because this can be used in order to pay other players.
        await ShowCardTemporarilyAsync(card);
        await ContinueTurnAsync();
    }
    private DealCardGameCardInformation GetPlayerSelectedSingleCard(int deck)
    {
        GetPlayerToContinueTurn();
        SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
        DealCardGameCardInformation output = _gameContainer.DeckList!.GetSpecificItem(deck); //i think
        output.IsSelected = false;
        output.Drew = false;
        return output;
    }
    private async Task PlayPassGoAsync(DealCardGameCardInformation card)
    {
        PlayerDraws = WhoTurn;
        LeftToDraw = 2;
        await AnimatePlayAsync(card);
        await DrawAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects(); //i think this is best.

        //anything else is here.  varies by game.


        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    private async Task ShowCardTemporarilyAsync(DealCardGameCardInformation card)
    {
        _model.ShownCard = card;
        _command.UpdateAll();
        await Delay!.DelayMilli(400);
        _model.ShownCard = null;
    }
    public async Task BankAsync(int deck)
    {
        var card = GetPlayerSelectedSingleCard(deck);
        SingleInfo!.Money += card.ClaimedValue;
        SingleInfo.BankedCards.Add(card); //this card is put into the bank.  needs to show up there so if you have to pay up, can use these cards.
        await ShowCardTemporarilyAsync(card);
        await ContinueTurnAsync();
    }
    public async Task ProcessPaymentsAsync(BasicList<int> cards)
    {
        _fromAutoResume = false; //not anymore.
        _command.ResetCustomStates();
        _command.UpdateAll(); //i can see if it at least updates my screen.
        if (_gameContainer.CanSendMessage())
        {
            await _gameContainer.Network!.SendAllAsync("finishpayment", cards);
        }
        var player = PlayerList.GetOtherPlayer();
        player.Payments.ReplaceRange(cards);
        player.AllPlayerStatus = EnumAllPlayerStatus.None; //because paid up.
        player.Debt = 0; //because you no longer owe.
        if (PlayerList.All(x => x.Debt == 0))
        {
            _gameContainer.PersonalInformation.NeedsPayment = false;
            await _privateAutoResume.SaveStateAsync(_gameContainer);
            await AfterPaymentsAsync();
            return;
        }
        OtherTurn = player.Id;
        SingleInfo = PlayerList.GetOtherPlayer();
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _gameContainer.PersonalInformation.State = new();
            _gameContainer.PersonalInformation.Payments.Clear();
            _gameContainer.PersonalInformation.NeedsPayment = true; //for everybody, will record that we need payment.
        }
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }
    public async Task ConfirmPaymentAsync()
    {
        OtherTurn = 0;
        GetPlayerToContinueTurn(); //will finish processing.
        foreach (var player in PlayerList)
        {
            player.Payments.ForEach(deck =>
            {
                SetPropertiesModel? property = player.GetPropertyFromCard(deck);

                if (property is not null)
                {
                    var card = property.Cards.GetSpecificItem(deck);
                    if (card.ActionCategory == EnumActionCategory.House || card.ActionCategory == EnumActionCategory.Hotel)
                    {
                        SingleInfo!.BankedCards.Add(card);
                    }
                    else
                    {
                        player.SetData.RemoveCardFromPlayerSet(deck, property.Color);
                        SingleInfo!.AddSingleCardToPlayerPropertySet(card, property.Color); //hopefully this simple (?)
                    }
                }
            });
            player.Payments.Clear();
            SaveRoot.GameStatus = EnumGameStatus.None;
            await ContinueTurnAsync();
        }
    }
    private async Task AfterPaymentsAsync()
    {
        OtherTurn = 0;
        GetPlayerToContinueTurn(); //will finish processing.
        DealCardGameCardInformation card;
        BasicList<DealCardGameCardInformation> payments = [];
        _model.ReceivedPayments.ClearHand(); //just in case.
        foreach (var player in PlayerList)
        {
            player.Payments.ForEach(deck =>
            {
                card = _gameContainer.DeckList.GetSpecificItem(deck);
                card.IsSelected = false;
                _model.ReceivedPayments.HandList.Add(card);
                SingleInfo!.Money += card.ClaimedValue;
                player.Money -= card.ClaimedValue;
                if (player.BankedCards.ObjectExist(deck))
                {
                    player.BankedCards.RemoveObjectByDeck(deck);
                    SingleInfo.BankedCards.Add(card);
                }
                else
                {
                    var other = player.GetPropertyFromCard(deck);
                    other!.Cards.RemoveObjectByDeck(deck);
                    if (card.ActionCategory == EnumActionCategory.House || card.ActionCategory == EnumActionCategory.Hotel)
                    {
                        SingleInfo.BankedCards.Add(card);
                    }
                    else
                    {
                        SingleInfo.AddSingleCardToPlayerPropertySet(card, other.Color);
                    }
                }
            });
            player.Payments.Clear(); //i think it needs to clear out the payments afterwards.
        }
        SaveRoot.GameStatus = EnumGameStatus.ConfirmPayment;
        await ContinueTurnAsync();
    }
    public async Task ResumeAsync()
    {
        SaveRoot.GameStatus = EnumGameStatus.None; //i think.
        _model.ReceivedPayments.ClearHand();
        await ContinueTurnAsync();
    }
    private async Task FinishStealingSetAsync(int player, EnumColor color)
    {
        OtherTurn = 0; //to double check (for now)
        var chosen = PlayerList[player];
        _model.ChosenPlayer = chosen.NickName;
        var list = chosen.SetData.GetCards(color).ToRegularDeckDict();
        _model.StolenCards.HandList.ReplaceRange(list);
        _command.UpdateAll();
        await Delay!.DelayMilli(700);
        _model.ChosenPlayer = "";
        _model.StolenCards.ClearHand();
        int transferMoney = list.Sum(x => x.ClaimedValue);
        chosen.Money -= transferMoney;
        GetPlayerToContinueTurn();
        SingleInfo!.Money += transferMoney;
        chosen.ClearPlayerProperties(color);
        SingleInfo.AddSeveralCardsToPlayerPropertySet(list, color);
        await ContinueTurnAsync();
    }
    public async Task StartToStealSetAsync(int deck, int player, EnumColor color)
    {
        var card = GetPlayerSelectedSingleCard(deck);
        await AnimatePlayAsync(card);
        var chosen = PlayerList[player];
        if (chosen.MainHandList.Any(x => x.ActionCategory == EnumActionCategory.JustSayNo))
        {
            //this means you can decide to just say no.
            SaveRoot.OpponentColorChosen = color;
            UpdateToJustSayNo(chosen, card.Deck);
            //GetPlayerToContinueTurn(); //i think.
            await ContinueTurnAsync();
            return;
        }
        await FinishStealingSetAsync(player, color);
    }
    public async Task PossibleRentRequestAsync(RentModel rent)
    {
        var card = GetPlayerSelectedSingleCard(rent.Deck);
        await AnimatePlayAsync(card);
        OtherTurn = 0; //for now.
        GetPlayerToContinueTurn();
        int amountOwed = rent.RentOwed(SingleInfo!);
        int take = 0;
        if (rent.RentCategory == EnumRentCategory.SingleDouble)
        {
            take = 1;
        }
        else if (rent.RentCategory == EnumRentCategory.DoubleDouble)
        {
            take = 2;
        }
        if (take > 0)
        {
            var others = SingleInfo!.MainHandList.Where(x => x.ActionCategory == EnumActionCategory.DoubleRent).Take(take).ToBasicList();
            foreach (var item in others)
            {
                SingleInfo.MainHandList.RemoveSpecificItem(item);
                await AnimatePlayAsync(item); //these cards has to be removed because it was played.
            }
        }
        int player = _gameContainer.PersonalInformation.RentInfo.Player;
        EnumRentCategory category = rent.RentCategory;
        if (player == 0 && card.AnyColor)
        {
            player = PlayerList.Single(x => x.Id != WhoTurn).Id;
        }
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _gameContainer.PersonalInformation.RentInfo.RentCategory = EnumRentCategory.NA;
            _gameContainer.PersonalInformation.RentInfo.Color = EnumColor.None;
            _gameContainer.PersonalInformation.RentInfo.Player = -1;
            _gameContainer.PersonalInformation.RentInfo.Deck = 0;
            await _privateAutoResume.SaveStateAsync(_gameContainer);
        }
        StartPossiblePaymentProcesses();
        if (rent.Player == 0 && card.AnyColor == false) //for now.
        {
            await StartPossibleAllPlayersRentRequestAsync(amountOwed, category);
            //await StartFiguringOutPaymentsForAllPlayersAsync(amountOwed);
        }
        else
        {
            var chosen = PlayerList.Single(x => x.Id ==player);
            if (chosen.MainHandList.Any(x => x.ActionCategory == EnumActionCategory.JustSayNo) && chosen.Money > 0)
            {
                SaveRoot.RentCategory = category;
                SaveRoot.PaymentOwed = amountOwed;
                UpdateToJustSayNo(chosen, card.Deck);
                await ContinueTurnAsync();
                return;
            }
            await SelectSinglePlayerForPaymentAsync(player, amountOwed); //iffy.
        }
    }
    private async Task StartPossibleAllPlayersRentRequestAsync(int amountOwed, EnumRentCategory category)
    {
        if (category == EnumRentCategory.NeedChoice || category == EnumRentCategory.NA)
        {
            throw new CustomBasicException("No rent category was chosen");
        }
        //this is a placeholder.
        await StartFiguringOutPaymentsForAllPlayersAsync(amountOwed); //for now.
    }
    public async Task StartRentAsync(SetPlayerModel model, DealCardGameCardInformation card)
    {
        if (card.AnyColor && PlayerList.Count > 2)
        {
            _gameContainer.PersonalInformation.RentInfo.Player = -1; //means needs to choose a player.
        }
        else
        {
            _gameContainer.PersonalInformation.RentInfo.Player = 0; //means all players.
        }
        _gameContainer.PersonalInformation.RentInfo.Color = model.Color;
        _gameContainer.PersonalInformation.RentInfo.Deck = card.Deck;
        _gameContainer.PersonalInformation.RentInfo.RentCategory = EnumRentCategory.NeedChoice; //i think
        await _privateAutoResume.SaveStateAsync(_gameContainer);
        _command.UpdateAll();
    }
    public async Task StartStealingPropertyAsync(SetPlayerModel model, DealCardGameCardInformation card)
    {
        _gameContainer.PersonalInformation.StealInfo.StartStealing = true;
        _gameContainer.PersonalInformation.StealInfo.PlayerId = model.PlayerId;
        _gameContainer.PersonalInformation.StealInfo.Color = model.Color;
        _gameContainer.PersonalInformation.StealInfo.CardPlayed = card.Deck;
        _gameContainer.PersonalInformation.StealInfo.CardChosen = 0;
        await _privateAutoResume.SaveStateAsync(_gameContainer);
        _command.UpdateAll();
    }
    public async Task StartTradingPropertyAsync(SetPlayerModel model, DealCardGameCardInformation card)
    {
        _gameContainer.PersonalInformation.TradeInfo.StartTrading = true;
        _gameContainer.PersonalInformation.TradeInfo.PlayerId = model.PlayerId;
        _gameContainer.PersonalInformation.TradeInfo.OpponentColor = model.Color;
        _gameContainer.PersonalInformation.TradeInfo.CardPlayed = card.Deck;
        _gameContainer.PersonalInformation.TradeInfo.OpponentCard = 0;
        _gameContainer.PersonalInformation.TradeInfo.YourCard = 0;
        _gameContainer.PersonalInformation.TradeInfo.YourColor = EnumColor.None;
        await _privateAutoResume.SaveStateAsync(_gameContainer);
        _command.UpdateAll();
    }
    public async Task PossibleTradingPropertyAsync(TradePropertyModel trade)
    {
        trade.StartTrading = false;
        var cardPlayed = GetPlayerSelectedSingleCard(trade.CardPlayed);
        await AnimatePlayAsync(cardPlayed);
        var playerChosen = PlayerList.Single(x => x.Id == trade.PlayerId);

        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            if (trade.Equals(_gameContainer.PersonalInformation.TradeInfo))
            {
                //has to somehow clone this.
                trade = _gameContainer.PersonalInformation.TradeInfo.Clone(); //go ahead and clone this.
            }
            _gameContainer.PersonalInformation.TradeInfo.StartTrading = false;
            _gameContainer.PersonalInformation.TradeInfo.PlayerId = 0;
            _gameContainer.PersonalInformation.TradeInfo.OpponentColor = EnumColor.None;
            _gameContainer.PersonalInformation.TradeInfo.CardPlayed = 0;
            _gameContainer.PersonalInformation.TradeInfo.OpponentCard = 0;
            _gameContainer.PersonalInformation.TradeInfo.YourCard = 0;
            _gameContainer.PersonalInformation.TradeInfo.YourColor = EnumColor.None;
            await _privateAutoResume.SaveStateAsync(_gameContainer);
        }
        if (playerChosen.MainHandList.Any(x => x.ActionCategory == EnumActionCategory.JustSayNo))
        {
            //this means you can decide to just say no.
            SaveRoot.OpponentColorChosen = trade.OpponentColor;
            SaveRoot.YourColorChosen = trade.YourColor;
            SaveRoot.YourTrade = trade.YourCard; //i think.
            SaveRoot.OpponentTrade = trade.OpponentCard; //i think.
            UpdateToJustSayNo(playerChosen, trade.CardPlayed);
            //GetPlayerToContinueTurn(); //i think.
            await ContinueTurnAsync();
            return;
        }
        await FinishTradingPropertyAsync(trade);
    }
    public async Task FinishTradingPropertyAsync(TradePropertyModel trade)
    {
        var playerChosen = PlayerList.Single(x => x.Id == trade.PlayerId);
        _model.TradeDisplay = new();
        _model.TradeDisplay.TradePlayerName = playerChosen.NickName;
        _model.TradeDisplay.WhoPlayerName = SingleInfo!.NickName;
        DealCardGameCardInformation youReceive = _gameContainer.DeckList.GetSpecificItem(trade.OpponentCard);
        _model.TradeDisplay.WhoReceive = youReceive;
        DealCardGameCardInformation tradeReceive = _gameContainer.DeckList.GetSpecificItem(trade.YourCard);
        _model.TradeDisplay.TradeReceive = tradeReceive;
        _command.UpdateAll();
        await Delay!.DelayMilli(1500);
        _model.TradeDisplay = null;
        _command.UpdateAll();
        playerChosen.SetData.RemoveCardFromPlayerSet(trade.OpponentCard, trade.OpponentColor);
        int transferMoney;
        transferMoney = youReceive.ClaimedValue;
        playerChosen.Money -= transferMoney;
        SingleInfo.Money += transferMoney;
        SingleInfo.AddSingleCardToPlayerPropertySet(youReceive, trade.OpponentColor);
        transferMoney = tradeReceive.ClaimedValue;
        playerChosen.Money += transferMoney;
        SingleInfo.Money -= transferMoney;
        playerChosen.AddSingleCardToPlayerPropertySet(tradeReceive, trade.YourColor);
        SingleInfo.SetData.RemoveCardFromPlayerSet(trade.YourCard, trade.YourColor);

        await ContinueTurnAsync();
    }
    public async Task PossibleStealingPropertyAsync(StealPropertyModel steal)
    {
        steal.StartStealing = false;
        var cardPlayed = GetPlayerSelectedSingleCard(steal.CardPlayed);
        await AnimatePlayAsync(cardPlayed);
        var playerChosen = PlayerList.Single(x => x.Id == steal.PlayerId);
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            if (steal.Equals(_gameContainer.PersonalInformation.StealInfo))
            {
                //has to somehow clone this.
                steal = _gameContainer.PersonalInformation.StealInfo.Clone(); //go ahead and clone this.
            }
            _gameContainer!.PersonalInformation.StealInfo.StartStealing = false;
            _gameContainer.PersonalInformation.StealInfo.Color = EnumColor.None;
            _gameContainer.PersonalInformation.StealInfo.PlayerId = 0;
            _gameContainer.PersonalInformation.StealInfo.CardPlayed = 0;
            _gameContainer.PersonalInformation.StealInfo.CardChosen = 0;
            await _privateAutoResume.SaveStateAsync(_gameContainer);
        }
        if (playerChosen.MainHandList.Any(x => x.ActionCategory == EnumActionCategory.JustSayNo))
        {
            //this means they have the opportunity to just say no.
            SaveRoot.OpponentColorChosen = steal.Color;
            SaveRoot.CardStolen = steal.CardChosen; //i think.
            UpdateToJustSayNo(playerChosen, cardPlayed.Deck);
            await ContinueTurnAsync();
            return;
        }

        await FinishStealingPropertyAsync(steal); //i think.
    }
    private void UpdateToJustSayNo(DealCardGamePlayerItem player, int actionDeck)
    {
        SaveRoot.ActionCardUsed = actionDeck;
        _gameContainer.IsJustSayNoSelf = player.PlayerCategory == EnumPlayerCategory.Self;
        SaveRoot.PlayerUsedAgainst = player.Id;
        OtherTurn = player.Id; //hopefully this is also okay (?)
        SaveRoot.GameStatus = EnumGameStatus.ConsiderJustSayNo;
    }
    public async Task FinishStealingPropertyAsync(StealPropertyModel steal)
    {
        var playerChosen = PlayerList.Single(x => x.Id == steal.PlayerId);
        _model.ChosenPlayer = playerChosen.NickName; //this is the player who lost their card.
        var cardStolen = _gameContainer.DeckList.GetSpecificItem(steal.CardChosen);
        _model.ShownCard = cardStolen;
        _command.UpdateAll();
        await Delay!.DelayMilli(700);
        _model.ChosenPlayer = "";
        _model.ShownCard = null;

        playerChosen.SetData.RemoveCardFromPlayerSet(cardStolen.Deck, steal.Color);

        int transferMoney = cardStolen.ClaimedValue;
        playerChosen.Money -= transferMoney;
        OtherTurn = 0; //to double check (for now)
        GetPlayerToContinueTurn();
        SingleInfo!.Money += transferMoney;
        SingleInfo.AddSingleCardToPlayerPropertySet(cardStolen, steal.Color);

        await ContinueTurnAsync();
    }
}