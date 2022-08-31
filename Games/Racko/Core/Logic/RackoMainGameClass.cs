using BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfacesForHelpers;
using BasicGameFrameworkLibrary.Core.SpecializedGameTypes.RummyClasses;

namespace Racko.Core.Logic;
[SingletonGame]
public class RackoMainGameClass
    : CardGameClass<RackoCardInformation, RackoPlayerItem, RackoSaveInfo>
    , IMiscDataNM, IStartNewGame, ISerializable
{
    private readonly RackoVMData _model;
    private readonly CommandContainer _command;
    private readonly RackoGameContainer _gameContainer;
    private readonly IToast _toast;
    private int _previousUse; //for computer ai.
    public RackoMainGameClass(IGamePackageResolver mainContainer,
        IEventAggregator aggregator,
        BasicData basicData,
        TestOptions test,
        RackoVMData currentMod,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        ICardInfo<RackoCardInformation> cardInfo,
        CommandContainer command,
        RackoGameContainer gameContainer,
        RackoDelegates delegates,
        ISystemError error,
        IToast toast
        ) : base(mainContainer, aggregator, basicData, test, currentMod, state, delay, cardInfo, command, gameContainer, error, toast)
    {
        _model = currentMod;
        _command = command;
        _gameContainer = gameContainer;
        _toast = toast;
        delegates.PlayerCount = () => SaveRoot.PlayerList.Count;
    }
    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisPlayer.CanShowValues = true;
            }
            else
            {
                thisPlayer.CanShowValues = false;
            }
        });
        return base.FinishGetSavedAsync();
    }
    private void LoadControls()
    {
        if (IsLoaded == true)
        {
            return;
        }
        IsLoaded = true;
    }
    protected override async Task ComputerTurnAsync()
    {
        if (Test!.NoAnimations == false)
        {
            await Delay!.DelaySeconds(.5);
        }
        if (HasRacko() == true)
        {
            if (BasicData!.MultiPlayer == true)
            {
                await Network!.SendAllAsync("racko");
            }
            await EndRoundAsync();
            return;
        }
        int nums;
        if (_gameContainer.AlreadyDrew == false)
        {
            nums = ComputerAI.PickUp(this, _model);
            if (nums > 0)
            {
                _previousUse = nums;
                await PickupFromDiscardAsync();
                return;
            }
            await DrawAsync();
            return;
        }
        RackoCardInformation thisCard;
        if (_previousUse > 0)
        {
            thisCard = SingleInfo!.MainHandList[_previousUse - 1];
            thisCard.WillKeep = true;
            if (BasicData!.MultiPlayer == true)
            {
                await _gameContainer.SendDiscardMessageAsync(thisCard.Deck);
            }
            await DiscardAsync(thisCard.Deck);
            return;
        }
        nums = ComputerAI.CardToPlay(this, _model);
        if (nums > 0)
        {
            thisCard = SingleInfo!.MainHandList[nums - 1];
            thisCard.WillKeep = true;
            if (BasicData!.MultiPlayer == true)
            {
                await _gameContainer.SendDiscardMessageAsync(thisCard.Deck);
            }
            await DiscardAsync(thisCard.Deck);
            return;
        }
        if (BasicData!.MultiPlayer == true)
        {
            await Network!.SendAllAsync("discardcurrent");
        }
        await DiscardCurrentAsync();
    }
    protected override Task StartSetUpAsync(bool isBeginning)
    {
        LoadControls();
        PlayerList!.ForEach(thisPlayer =>
        {
            if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisPlayer.CanShowValues = true;
            }
            else
            {
                thisPlayer.CanShowValues = false;
            }
        });
        return base.StartSetUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status)
        {
            case "racko":
                await EndRoundAsync();
                return;
            case "discardcurrent":
                await DiscardCurrentAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task StartNewTurnAsync()
    {
        await base.StartNewTurnAsync();
        _previousUse = 0;
        _gameContainer.AlreadyDrew = false;
        await ContinueTurnAsync();
    }
    public override async Task EndTurnAsync()
    {
        SingleInfo = PlayerList!.GetWhoPlayer();
        SingleInfo.MainHandList.UnhighlightObjects();
        _command.ManuelFinish = true; //because it could be somebody else's turn.
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public override async Task EndRoundAsync()
    {
        PlayerList!.ForEach(thisPlayer => thisPlayer.CanShowValues = true);
        PlayerList.ForEach(thisPlayer =>
        {
            thisPlayer.ScoreRound = WhatScore(thisPlayer);
            if (thisPlayer.ScoreRound == 0)
            {
                throw new CustomBasicException("Cannot be 0 points");
            }
            thisPlayer.TotalScore += thisPlayer.ScoreRound;
        });
        PossibleWinProcess();
        if (SingleInfo != null)
        {
            await ShowWinAsync();
            return;
        }
        await this.RoundOverNextAsync();
    }
    private void PossibleWinProcess()
    {
        SingleInfo = PlayerList.Where(items => items.TotalScore >= 300).FirstOrDefault();
    }
    private static int HowManyHigher(RackoPlayerItem thisPlayer)
    {
        int x = 0;
        int thisNum = 0;
        foreach (var thisCard in thisPlayer.MainHandList)
        {
            if (thisCard.Value > thisNum)
            {
                x++;
                thisNum = thisCard.Value;
            }
            else
            {
                return x;
            }
        }
        return x;
    }
    private int WhatScore(RackoPlayerItem thisPlayer)
    {
        if (thisPlayer.Id != WhoTurn)
        {
            return HowManyHigher(thisPlayer) * 5;
        }
        return 75 + BonusPoints(thisPlayer);
    }
    private int BonusPoints(RackoPlayerItem thisPlayer)
    {
        RummyProcesses<EnumSuitList, EnumRegularColorList, RackoCardInformation> thisInfo = new();
        thisInfo.HasSecond = false;
        thisInfo.HasWild = false;
        thisInfo.NeedMatch = false; //try this too.
        thisInfo.LowNumber = 1;
        RackoDeckCount temps = MainContainer.Resolve<RackoDeckCount>();
        thisInfo.HighNumber = temps.GetDeckCount();
        DeckRegularDict<RackoCardInformation> tempCol = thisPlayer.MainHandList.ToRegularDeckDict();
        var newColls = thisInfo.WhatNewRummy(tempCol, 6, EnumRummyType.Runs, true);
        if (newColls.Count > 0)
        {
            return 400;
        }
        newColls = thisInfo.WhatNewRummy(tempCol, 5, EnumRummyType.Runs, true);
        if (newColls.Count > 0)
        {
            return 200;
        }
        newColls = thisInfo.WhatNewRummy(tempCol, 4, EnumRummyType.Runs, true);
        if (newColls.Count > 0)
        {
            return 100;
        }
        newColls = thisInfo.WhatNewRummy(tempCol, 3, EnumRummyType.Runs, true);
        if (newColls.Count > 0)
        {
            return 50;
        }
        return 0;
    }
    public override async Task DiscardAsync(RackoCardInformation thisCard)
    {
        var newCard = _model!.OtherPile!.GetCardInfo();
        newCard.WillKeep = thisCard.WillKeep;
        if (thisCard.Deck == newCard.Deck)
        {
            throw new CustomBasicException("Cannot be duplicate for discard.  Rethink");
        }
        if (SingleInfo!.MainHandList.Contains(thisCard) == true)
        {
            thisCard = SingleInfo.MainHandList.GetSpecificItem(thisCard.Deck);
        }
        else
        {
            if (SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
            {
                _toast.ShowUserErrorToast("Card not found.  Rethinking may be required.  Not sure though");
            }
            await ContinueTurnAsync();
            return;
        }
        SingleInfo.MainHandList.ReplaceItem(thisCard, newCard);
        if (SingleInfo.MainHandList.ObjectExist(thisCard.Deck))
        {
            throw new CustomBasicException("Failed To Replace Card");
        }
        _model.OtherPile.ClearCards();
        await AnimatePlayAsync(thisCard);
        await EndTurnAsync();
    }
    public bool HasRacko()
    {
        int thisNum = 0;
        foreach (var thisCard in SingleInfo!.MainHandList)
        {
            if (thisCard.Value < thisNum)
            {
                return false;
            }
            thisNum = thisCard.Value;
        }
        return true;
    }
    public async Task DiscardCurrentAsync()
    {
        var thisCard = _model!.OtherPile!.GetCardInfo();
        _model.OtherPile.ClearCards();
        await AnimatePlayAsync(thisCard);
        await EndTurnAsync();
    }
    Task IStartNewGame.ResetAsync()
    {
        PlayerList!.ForEach(thisPlayer =>
        {
            thisPlayer.ScoreRound = 0;
            thisPlayer.TotalScore = 0;
        });
        return Task.CompletedTask;
    }
    public override Task ContinueTurnAsync()
    {
        PlayerList.ForEach(player => player.UpdateAllValues());
        return base.ContinueTurnAsync();
    }
}