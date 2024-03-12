using System.IO;

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
                await StealSetAsync(stealSet.Deck, stealSet.PlayerId, stealSet.Color);
                return;
            case "rentrequest":
                RentModel rent = await js1.DeserializeObjectAsync<RentModel>(content);
                await RentRequestAsync(rent);
                return;
            case "stealproperty":
                StealPropertyModel stealProperty = await js1.DeserializeObjectAsync<StealPropertyModel>(content);
                await FinishStealingPropertyAsync(stealProperty);
                return;
            case "playerchosenfordebt":
                await ChosePlayerForDebtAsync(int.Parse(content));
                return;
            case "trade":
                TradePropertyModel tradeProperty = await js1.DeserializeObjectAsync<TradePropertyModel>(content);
                await FinishTradingPropertyAsync(tradeProperty);
                return;
            default:
                _toast.ShowUserErrorToast($"Nothing for status {status}");
                return;
                //throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
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
            LoadPlayerPicker();
            await ContinueTurnAsync();
            return;
        }
        await StartFiguringOutPaymentsForAllPlayersAsync(5);
    }
    public async Task ChosePlayerForDebtAsync(int id)
    {
        var player = PlayerList.Single(x => x.Id == id);
        SaveRoot.GameStatus = EnumGameStatus.None;
        await SelectSinglePlayerForPaymentAsync(id, 5);
    }
    public void LoadPlayerPicker()
    {
        var list = PlayerList.AllPlayersExceptForCurrent().Select(x => x.NickName).ToBasicList();
        _model.PlayerPicker.SelectedIndex = 0; //because its one based this time.
        _model.PlayerPicker.LoadTextList(list);
    }
    private async Task StartBirthdayAsync(DealCardGameCardInformation card)
    {
        await AnimatePlayAsync(card);
        StartPossiblePaymentProcesses();
        await StartFiguringOutPaymentsForAllPlayersAsync(2);
    }
    public async Task SelectSinglePlayerForPaymentAsync(int player, int owed)
    {
        OtherTurn = player;
        SingleInfo = PlayerList.GetOtherPlayer();
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
        if (PlayerList.All(x => x.Debt == 0 && x.Payments.Count == 0))
        {
            SaveRoot.GameStatus = EnumGameStatus.None; //i think.
            await ContinueTurnAsync();
            return;
        }

        //var temp = PlayerList.FirstOrDefault(x => x.Debt > 0 && x.Payments.Count == 0);
        if (PlayerList.Any(x => x.Debt > 0))
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
            return;
        }
        if (currentPlayer.Money <= owed && currentPlayer.Money > 0)
        {
            currentPlayer.Debt = 0;
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
    public async Task StealSetAsync(int deck, int player, EnumColor color)
    {
        var card = GetPlayerSelectedSingleCard(deck);
        await AnimatePlayAsync(card);
        var chosen = PlayerList[player];
        _model.ChosenPlayer = chosen.NickName;
        var list = chosen.SetData.GetCards(color).ToRegularDeckDict();
        _model.StolenCards.HandList.ReplaceRange(list);
        _command.UpdateAll();
        await Delay!.DelayMilli(700);
        _model.StolenCards.ClearHand();
        int transferMoney = list.Sum(x => x.ClaimedValue);
        chosen.Money -= transferMoney;
        OtherTurn = 0; //to double check (for now)
        GetPlayerToContinueTurn();
        SingleInfo!.Money += transferMoney;
        chosen.ClearPlayerProperties(color);
        SingleInfo.AddSeveralCardsToPlayerPropertySet(list, color);
        await ContinueTurnAsync();
    }
    public async Task RentRequestAsync(RentModel rent)
    {
        //if (rent.Player > 0)
        //{
        //    _toast.ShowUserErrorToast("Unable to process other players for now");
        //    return;
        //}
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
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _gameContainer.PersonalInformation.RentInfo.RentCategory = EnumRentCategory.NA;
            _gameContainer.PersonalInformation.RentInfo.Color = EnumColor.None;
            _gameContainer.PersonalInformation.RentInfo.Player = -1;
            _gameContainer.PersonalInformation.RentInfo.Deck = 0;
            await _privateAutoResume.SaveStateAsync(_gameContainer);
        }
        StartPossiblePaymentProcesses();
        if (rent.Player == 0)
        {
            await StartFiguringOutPaymentsForAllPlayersAsync(amountOwed);
        }
        else
        {
            await SelectSinglePlayerForPaymentAsync(player, amountOwed); //iffy.
        }
    }
    public async Task StartRentAsync(SetPlayerModel model, DealCardGameCardInformation card)
    {
        if (card.AnyColor && PlayerList.Count > 2)
        {
            _gameContainer.PersonalInformation.RentInfo.Player = -1; //means needs to choose a player.
        }
        else
        {
            _gameContainer.PersonalInformation.RentInfo.Player = 0; //means you have to choose a player.
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
    public async Task FinishTradingPropertyAsync(TradePropertyModel trade)
    {
        trade.StartTrading = false;
        var cardPlayed = GetPlayerSelectedSingleCard(trade.CardPlayed);
        await AnimatePlayAsync(cardPlayed);
        var playerChosen = PlayerList.Single(x => x.Id == trade.PlayerId);
        //needs to show details temporarily.
        //show display trade.
        OtherTurn = 0; //i think
        GetPlayerToContinueTurn();
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
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _gameContainer.PersonalInformation.TradeInfo.StartTrading = false;
            _gameContainer.PersonalInformation.TradeInfo.PlayerId = 0;
            _gameContainer.PersonalInformation.TradeInfo.OpponentColor = EnumColor.None;
            _gameContainer.PersonalInformation.TradeInfo.CardPlayed = 0;
            _gameContainer.PersonalInformation.TradeInfo.OpponentCard = 0;
            _gameContainer.PersonalInformation.TradeInfo.YourCard = 0;
            _gameContainer.PersonalInformation.TradeInfo.YourColor = EnumColor.None;
            await _privateAutoResume.SaveStateAsync(_gameContainer);
        }
        await ContinueTurnAsync();
    }
    public async Task FinishStealingPropertyAsync(StealPropertyModel steal)
    {
        steal.StartStealing = false;
        var cardPlayed = GetPlayerSelectedSingleCard(steal.CardPlayed);
        await AnimatePlayAsync(cardPlayed);
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
        if (SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _gameContainer!.PersonalInformation.StealInfo.StartStealing = false;
            _gameContainer.PersonalInformation.StealInfo.Color = EnumColor.None;
            _gameContainer.PersonalInformation.StealInfo.PlayerId = 0;
            _gameContainer.PersonalInformation.StealInfo.CardPlayed = 0;
            _gameContainer.PersonalInformation.StealInfo.CardChosen = 0;
            await _privateAutoResume.SaveStateAsync(_gameContainer);
        }
        await ContinueTurnAsync();
    }
}