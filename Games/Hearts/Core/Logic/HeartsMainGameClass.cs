using System.Runtime.InteropServices;

namespace Hearts.Core.Logic;
[SingletonGame]
public class HeartsMainGameClass
    : TrickGameClass<EnumSuitList, HeartsCardInformation, HeartsPlayerItem, HeartsSaveInfo>
        , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly HeartsVMData _model;
    private readonly CommandContainer _command; //most of the time, needs this.  if not needed, take out.
    private readonly HeartsGameContainer _gameContainer; //if we don't need it, take it out.
    private readonly IAdvancedTrickProcesses _aTrick;
    private readonly IToast _toast;
    private readonly IMessageBox _message;
    public HeartsMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        HeartsVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<HeartsCardInformation> cardInfo,
        CommandContainer command,
        HeartsGameContainer gameContainer,
        ITrickData trickData,
        ITrickPlay trickPlay,
        IAdvancedTrickProcesses aTrick,
        ISystemError error,
        IToast toast,
        IMessageBox message
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, trickData, trickPlay, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _aTrick = aTrick;
        _toast = toast;
        _message = message;
    }
    public override async Task FinishGetSavedAsync()
    {
        LoadControls();
        LoadVM();
        await base.FinishGetSavedAsync();
        _aTrick!.LoadGame();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true; //i think needs to be here.
    }
    protected override void LoadVM()
    {
        base.LoadVM();
        SaveRoot!.LoadMod(_model!);
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.75);
        }
        if (SaveRoot!.GameStatus == EnumStatus.ShootMoon)
        {
            throw new CustomBasicException("I doubt the computer would ever shoot the moon.  If so; rethinking is required");
        }
        if (SaveRoot.GameStatus == EnumStatus.Passing)
        {
            await ComputerPassCardsAsync();
            return;
            //throw new CustomBasicException("The computer should have already passed the cards");
        }
        BasicList<int> moveList;
        moveList = SingleInfo!.MainHandList.Where(x => IsValidMove(x.Deck)).Select(x => x.Deck).ToBasicList();
        int deck = moveList.First();
        if (BasicData!.MultiPlayer)
        {
            await Network!.SendAllAsync("trickplay", deck);
        }
        await PlayCardAsync(deck);
    }
    private async Task ComputerPassCardsAsync()
    {
        if (SingleInfo!.PlayerCategory != EnumPlayerCategory.Computer)
        {
            throw new CustomBasicException("Must be computer player when the computer is passing cards");
        }
        var thisList = SingleInfo.MainHandList.GetRandomList(false, 3);
        var fins = thisList.Select(x => x.Deck).ToBasicList();
        if (_gameContainer.BasicData.MultiPlayer == true)
        {
            //this means the computer has to send the passing of cards
            await Network!.SendAllAsync("passcards", fins);
        }
        await CardsPassedAsync(fins);
    }
    private void TransferToPassed(BasicList<int> thisList)
    {
        SingleInfo!.CardsPassed = thisList;
        thisList.ForEach(index => SingleInfo.MainHandList.RemoveObjectByDeck(index));
    }
    public async Task CardsPassedAsync(BasicList<int> thisList)
    {
        if (thisList.Count != 3)
        {
            throw new CustomBasicException("Must pass 3 cards");
        }
        SingleInfo = PlayerList!.GetWhoPlayer(); //has to be whoever turn it is.
        TransferToPassed(thisList);
        await EndTurnAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        LoadVM();
        SaveRoot!.RoundNumber++;
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.CurrentScore = 0;
            thisPlayer.TricksWon = 0;
            thisPlayer.HadPoints = false;
        });
        SaveRoot.TrickStatus = EnumTrickStatus.FirstTrick;
        SaveRoot.WhoLeadsTrick = 0; //hopefully the standard whostarts is okay (?)
        if (isBeginning)
        {
            SaveRoot.PassOption = EnumPassOption.Left;
        }
        else if (SaveRoot.PassOption == EnumPassOption.Left)
        {
            SaveRoot.PassOption = EnumPassOption.Right;
        }
        else if (SaveRoot.PassOption == EnumPassOption.Right)
        {
            SaveRoot.PassOption = EnumPassOption.Across;
        }
        else if (SaveRoot.PassOption == EnumPassOption.Across)
        {
            SaveRoot.PassOption = EnumPassOption.Keeper;
        }
        else if (SaveRoot.PassOption == EnumPassOption.Keeper)
        {
            SaveRoot.PassOption = EnumPassOption.Left;
        }
        if (SaveRoot.PassOption == EnumPassOption.Keeper)
        {
            SaveRoot.GameStatus = EnumStatus.Normal;
            _model.TrickArea1!.ClearBoard();
        }
        else
        {
            SaveRoot.GameStatus = EnumStatus.Passing;
        }
        return base.StartSetUpAsync(isBeginning);
    }
    protected override Task LastPartOfSetUpBeforeBindingsAsync()
    {
        if (SaveRoot.PassOption == EnumPassOption.Keeper)
        {
            SaveRoot!.WhoLeadsTrick = WhoLeadsFirstTrick(); //try this
            WhoTurn = SaveRoot!.WhoLeadsTrick; //has to be here i think.  try this next.
        }
        return base.LastPartOfSetUpBeforeBindingsAsync();
    }
    private int PassTurn(int turn)
    {
        if (SaveRoot.PassOption == EnumPassOption.Across)
        {
            if (turn == 1)
            {
                return 3;
            }
            if (turn == 2)
            {
                return 4;
            }
            if (turn == 3)
            {
                return 1;
            }
            if (turn == 4)
            {
                return 2;
            }
            throw new CustomBasicException("Unknown Pass");
        }
        if (SaveRoot.PassOption == EnumPassOption.Right)
        {
            if (turn == 4)
            {
                return 1;
            }
            return turn + 1;
        }
        if (SaveRoot.PassOption == EnumPassOption.Left)
        {
            if (turn == 1)
            {
                return 4;
            }
            return turn - 1;
        }
        throw new CustomBasicException("Unknown Pass Turn");
    }
    private string GetPlayerPassedTo()
    {
        if (SaveRoot.PassOption == EnumPassOption.Keeper)
        {
            throw new CustomBasicException("Since this is keepers, should not show who its being passed to");
        }
        int player = PassTurn(WhoTurn);
        return PlayerList[player].NickName; //i think.
    }
    public override async Task ContinueTurnAsync()
    {
        if (SaveRoot.GameStatus != EnumStatus.Passing)
        {
            _model.PassedPlayer = ""; //because its not passing anymore (?)
        }
        else
        {
            _model.PassedPlayer = GetPlayerPassedTo();
        }
        await base.ContinueTurnAsync();
        if (_model!.PlayerHand1!.IsEnabled == false && SingleInfo!.PlayerCategory == EnumPlayerCategory.Self)
        {
            _model.PlayerHand1.ReportCanExecuteChange();
        }
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "takepointsaway":
                await GiveSelfMinusPointsAsync();
                break;
            case "givepointseverybodyelse":
                await GiveEverybodyElsePointsAsync();
                break;
            case "passcards":
                var thisList = await content.GetSavedIntegerListAsync();
                await CardsPassedAsync(thisList);
                break;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    internal async Task GiveSelfMinusPointsAsync()
    {
        SingleInfo!.CurrentScore -= 52;
        SingleInfo.PreviousScore = SingleInfo.CurrentScore;
        _command.UpdateAll();
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
        {
            await _message.ShowMessageAsync($"{SingleInfo.NickName} chose to give him/herself -26 points");
        }
        await FinishEndAsync();
    }
    internal async Task GiveEverybodyElsePointsAsync()
    {
        int newPlayer;
        if (WhoTurn == 1)
        {
            newPlayer = 2;
        }
        else
        {
            newPlayer = 1;
        }
        var thisPlayer = PlayerList![newPlayer];
        SingleInfo!.CurrentScore -= 26;
        SingleInfo.PreviousScore = SingleInfo.CurrentScore;
        thisPlayer.CurrentScore += 26;
        thisPlayer.PreviousScore = thisPlayer.CurrentScore;
        _command.UpdateAll();
        if (SingleInfo.PlayerCategory == EnumPlayerCategory.OtherHuman)
        {
            await _message.ShowMessageAsync($"{SingleInfo.NickName} chose to give the other player 26 points");
        }
        await FinishEndAsync();
    }
    private async Task FinishEndAsync()
    {
        SaveRoot!.GameStatus = EnumStatus.EndRound;
        PlayerList!.ForEach(thisPlayer => thisPlayer.TotalScore += thisPlayer.CurrentScore);
        if (CanEndGame() == true)
        {
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    private void GetWinningPlayer()
    {
        SingleInfo = PlayerList.OrderBy(items => items.TotalScore).Take(1).Single();
    }
    private bool CanEndGame()
    {
        if (SaveRoot!.RoundNumber >= 15) //15 rounds the most no matter what.
        {
            GetWinningPlayer();
            return true;
        }
        if (PlayerList.Any(items => items.TotalScore >= 100))
        {
            GetWinningPlayer();
            return true;
        }
        return false;
    }
    public override async Task StartNewTurnAsync() //okay.
    {
        await base.StartNewTurnAsync();
        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    private bool NeedsPass()
    {
        foreach (var player in PlayerList)
        {
            if (player.CardsPassed.Count == 0)
            {
                return true;
            }
        }
        return false;
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        SingleInfo = PlayerList.GetWhoPlayer();
        if (SaveRoot.GameStatus == EnumStatus.Passing)
        {
            if (NeedsPass() == false)
            {
                await AfterCardsPassedAsync();
                return;
            }
        }
        this.ShowTurn();
        await SaveStateAsync();
        await ContinueTurnAsync();
    }
    private async Task AfterCardsPassedAsync()
    {
        if (PlayerList.Any(xx => xx.CardsPassed.Count != 3))
        {
            throw new CustomBasicException("All players must have passed 3 cards");
        }
        HeartsCardInformation card;
        foreach (var player in PlayerList)
        {
            int turn = player.Id; //i think.
            int passTo = PassTurn(turn);
            var others = PlayerList[passTo];
            BasicList<HeartsCardInformation> list = new();
            player.CardsPassed.ForEach(deck =>
            {
                card = _gameContainer.DeckList!.GetSpecificItem(deck);
                card.IsSelected = false;
                card.Drew = true;
                list.Add(card);
            });
            var fins = PlayerList[passTo];
            fins.MainHandList.AddRange(list);
        }
        SortCards();
        foreach (var player in PlayerList)
        {
            player.CardsPassed.Clear();
        }
        SaveRoot!.WhoLeadsTrick = WhoLeadsFirstTrick();
        SaveRoot.GameStatus = EnumStatus.Normal;
        _model.TrickArea1!.ClearBoard();
        WhoTurn = SaveRoot.WhoLeadsTrick;
        SingleInfo = PlayerList!.GetWhoPlayer();
        await StartNewTurnAsync();
    }
    private int WhoLeadsFirstTrick()
    {
        int deck = _gameContainer.DeckList.Where(x => x.Suit == EnumSuitList.Clubs && x.Value == EnumRegularCardValueList.Two).Single().Deck;
        return PlayerList.WhoHasCardFromDeck<HeartsCardInformation, HeartsPlayerItem>(deck);
    }
    private static int WhoWonTrick(DeckRegularDict<HeartsCardInformation> thisCol)
    {



        var leadCard = thisCol.First();
        var highCard = thisCol.Where(x => x.Suit == leadCard.Suit).OrderByDescending(x => x.Value).First();
        return highCard.Player; //try this (?)


        //var tempList = thisCol.ToRegularDeckDict();
        //tempList.RemoveSpecificItem(leadCard);
        //if (tempList.Any(x => x.Suit == leadCard.Suit && x.Value > leadCard.Value) == false)
        //{
        //    return leadCard.Player;
        //}
        //return WhoTurn;
    }
    public override async Task EndTrickAsync() //done i think.
    {
        var trickList = SaveRoot!.TrickList;
        if (trickList.Any(x => x.Suit == EnumSuitList.Hearts))
        {
            SaveRoot.TrickStatus = EnumTrickStatus.SuitBroken;
        }
        else if (SaveRoot.TrickStatus == EnumTrickStatus.FirstTrick)
        {
            SaveRoot.TrickStatus = EnumTrickStatus.NoSuit;
        }
        int wins = WhoWonTrick(trickList);
        HeartsPlayerItem thisPlayer = PlayerList![wins];
        thisPlayer.TricksWon++;
        WhoTurn = wins;
        SingleInfo = PlayerList.GetWhoPlayer();
        int points = trickList.Sum(xx => xx.HeartPoints);
        if (SingleInfo.HadPoints == false)
        {
            SingleInfo.HadPoints = trickList.Any(xx => xx.ContainPoints == true);
        }
        if (points != 0)
        {
            SingleInfo.CurrentScore += points;
        }
        await _aTrick!.AnimateWinAsync(wins);
        if (SingleInfo!.MainHandList.Count == 0)
        {
            await EndRoundAsync();
            return; //most of the time its in rounds.
        }
        _model!.PlayerHand1!.EndTurn();
        SaveRoot.WhoLeadsTrick = WhoTurn;
        await StartNewTrickAsync();
    }
    private async Task StartNewTrickAsync() //done
    {
        _aTrick!.ClearBoard();
        _command!.ManuelFinish = true; //because it could be somebody else's turn.
        await StartNewTurnAsync(); //hopefully this simple.
    }
    //decided to not have separate for shooting moon or even passing cards for game class.
    public override async Task EndRoundAsync()
    {
        foreach (var player in PlayerList)
        {
            player.PreviousScore = player.CurrentScore;
        }
        int Shoots = PlayerList!.WhoShotMoon;
        if (Shoots == 0)
        {
            await FinishEndAsync();
            return;
        }
        WhoTurn = Shoots;
        SaveRoot.GameStatus = EnumStatus.ShootMoon;
        SingleInfo = PlayerList!.GetWhoPlayer();
        _toast.ShowInfoToast($"{SingleInfo.NickName}  has shot the moon.  The player needs to choose to either give 26 points to the other player or take 26 points off their own score");
        await StartNewTurnAsync();
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.TotalScore = 0;
            thisPlayer.PreviousScore = 0;
        });
        SaveRoot!.RoundNumber = 0;
        return Task.CompletedTask;
    }
}