namespace DealCardGame.Core.Logic;
[SingletonGame]
public class DealCardGameMainGameClass
    : CardGameClass<DealCardGameCardInformation, DealCardGamePlayerItem, DealCardGameSaveInfo>
    , IMiscDataNM, ISerializable
{
    private readonly DealCardGameVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly DealCardGameGameContainer _gameContainer; //if we don't need it, take it out.
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
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        //anything else needed is here.
        return base.FinishGetSavedAsync();
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
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
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
    }
    public async Task PlayPropertyAsync(int deck, EnumColor color)
    {
        var card = GetPlayerSelectedSingleCard(deck);
        if (card.CardType == EnumCardType.PropertyWild)
        {
            card.MainColor = color;
        }
        SingleInfo!.AddCardToPlayerPropertySet(card, color);
        SingleInfo!.Money += card.ClaimedValue; //because this can be used in order to pay other players.
        await ShowCardTemporarilyAsync(card);
        await ContinueTurnAsync();
    }
    private DealCardGameCardInformation GetPlayerSelectedSingleCard(int deck)
    {
        GetPlayerToContinueTurn();
        SingleInfo!.MainHandList.RemoveObjectByDeck(deck);
        DealCardGameCardInformation output = _gameContainer.DeckList!.GetSpecificItem(deck); //i think
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
}