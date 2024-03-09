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
    public DealCardGameMainGameClass(IGamePackageResolver mainContainer,
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
    protected override async Task ShowHumanCanPlayAsync()
    {
        await base.ShowHumanCanPlayAsync();
        if (_gameContainer.PersonalInformation.NeedsPayment)
        {
            _command.UpdateSpecificAction("processpayment");
        }
        
        //_command.UpdateAll();
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
                await SelectSinglePlayerForPaymentAsync(int.Parse(content));
                return;
            case "resume":
                await ResumeAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
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
            SaveRoot.GameStatus = EnumGameStatus.StartDebtCollector;
            await ContinueTurnAsync();
            return;
        }
        await StartFiguringOutPaymentsForAllPlayersAsync(5);
    }
    private async Task StartBirthdayAsync(DealCardGameCardInformation card)
    {
        await AnimatePlayAsync(card);
        StartPossiblePaymentProcesses();
        await StartFiguringOutPaymentsForAllPlayersAsync(2);
    }
    public async Task SelectSinglePlayerForPaymentAsync(int player)
    {
        OtherTurn = player;
        SingleInfo = PlayerList.GetOtherPlayer();
        _model.ChosenPlayer = SingleInfo.NickName;
        _command.UpdateAll();
        await Delay!.DelayMilli(700);
        _model.ChosenPlayer = "";
        if (SaveRoot.GameStatus == EnumGameStatus.StartDebtCollector)
        {
            AttemptToAutomatePayment(SingleInfo, 5);
        }
        else
        {
            throw new CustomBasicException("Needs to figure out the payment owed now");
        }
        if (SingleInfo.Debt == 0)
        {
            SaveRoot.GameStatus = EnumGameStatus.None; //i think.
            await ContinueTurnAsync();
            return;
        }
        await StartPaymentProcessesForSelectedPlayerAsync();
        await ContinueTurnAsync(); //i think.
    }
    private async Task StartFiguringOutPaymentsForAllPlayersAsync(int owed)
    {
        foreach (var player in PlayerList)
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
        foreach (var player in PlayerList)
        {
            AttemptToAutomatePayment(player, owed);
        }
        if (PlayerList.All(x => x.Debt == 0))
        {
            SaveRoot.GameStatus = EnumGameStatus.None; //i think.
            await ContinueTurnAsync();
            return;
        }
        await StartPaymentProcessesForSelectedPlayerAsync();
        await ContinueTurnAsync(); //i think.
    }
    private async Task StartPaymentProcessesForSelectedPlayerAsync()
    {
        SingleInfo = PlayerList.First(x => x.Debt > 0);
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
        _gameContainer.PersonalInformation.State.SetData = SingleInfo.SetData.ToBasicList();
        _gameContainer.PersonalInformation.State.BankedCards = SingleInfo.BankedCards.ToRegularDeckDict();
        await _privateAutoResume.SaveStateAsync(_gameContainer);
    }

    private void AttemptToAutomatePayment(DealCardGamePlayerItem currentPlayer, int owed)
    {
        DealCardGamePlayerItem realPlayer = PlayerList.GetWhoPlayer();
        if (currentPlayer.Money <= owed && currentPlayer.Money > 0)
        {
            realPlayer.Money += currentPlayer.Money;
            currentPlayer.Money = 0;
            foreach (var item in currentPlayer.SetData)
            {
                var list = item.Cards.ToRegularDeckDict();
                list.RemoveAllOnly(x => x.ClaimedValue == 0);
                var others = list.RemoveAllAndObtain(x => x.ActionCategory != EnumActionCategory.None);
                list.RemoveGivenList(others);
                realPlayer.AddSeveralCardsToPlayerPropertySet(list, item.Color);
                realPlayer.BankedCards.AddRange(others);
            }
            realPlayer.BankedCards.AddRange(currentPlayer.BankedCards.ToBasicList());
            currentPlayer.BankedCards.Clear();
        }
        if (currentPlayer.Money == 0)
        {
            currentPlayer.Debt = 0; //to double check.
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
        if (_gameContainer.CanSendMessage())
        {
            await _gameContainer.Network!.SendAllAsync("finishpayment", cards);
        }
        var player = PlayerList.GetOtherPlayer();
        player.Payments.ReplaceRange(cards);
        player.Debt = 0; //because you no longer owe.
        if (PlayerList.All(x => x.Debt == 0))
        {
            await AfterPaymentsAsync();
            return;
        }
        OtherTurn = player.Id;
        SingleInfo = PlayerList.GetOtherPlayer();
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
        {
            _gameContainer.PersonalInformation.State = new();
            _gameContainer.PersonalInformation.Payments.Clear();
        }
        _gameContainer.PersonalInformation.NeedsPayment = true; //for everybody, will record that we need payment.
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
        _model.Payments.ClearHand();
        foreach (var player in PlayerList)
        {
            player.Payments.ForEach(deck =>
            {
                card =_gameContainer.DeckList.GetSpecificItem(deck);
                card.IsSelected = false;
                _model.Payments.HandList.Add(card);
                SingleInfo!.Money += card.ClaimedValue;
                player.Money -= card.ClaimedValue;
                if (player.BankedCards.ObjectExist(deck))
                {
                    player.BankedCards.RemoveObjectByDeck(deck);
                    SingleInfo.BankedCards.Add(card);
                }
            });
        }
        SaveRoot.GameStatus = EnumGameStatus.ConfirmPayment;
        await ContinueTurnAsync();
    }
    public async Task ResumeAsync()
    {
        SaveRoot.GameStatus = EnumGameStatus.None; //i think.
        _model.Payments.ClearHand();
        await ContinueTurnAsync();
    }
}