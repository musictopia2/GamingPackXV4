namespace MonopolyDicedGame.Core.Logic;
[SingletonGame]
public class MonopolyDicedGameMainGameClass : BasicGameClass<MonopolyDicedGamePlayerItem, MonopolyDicedGameSaveInfo>
    , ICommonMultiplayer<MonopolyDicedGamePlayerItem, MonopolyDicedGameSaveInfo>
    , IMiscDataNM, ISerializable
{
    public MonopolyDicedGameMainGameClass(IGamePackageResolver resolver,
        IEventAggregator aggregator,
        BasicData basic,
        TestOptions test,
        MonopolyDicedGameVMData model,
        IMultiplayerSaveState state,
        IAsyncDelayer delay,
        CommandContainer command,
        IRandomGenerator rs,
        MonopolyDicedGameGameContainer gameContainer,
        ISystemError error,
        IToast toast,
        MonopolyDiceSet monopolyDice,
        HouseDice houseDice
        ) : base(resolver, aggregator, basic, test, model, state, delay, command, gameContainer, error, toast)
    {
        _model = model;
        _rs = rs;
        _gameContainer = gameContainer;
        _monopolyDice = monopolyDice;
        _houseDice = houseDice;
        gameContainer.SelectOneMainAsync = SelectOneMainAsync;
    }
    private readonly MonopolyDicedGameVMData _model;
    private readonly IRandomGenerator _rs; //if we don't need, take out.
    private readonly MonopolyDicedGameGameContainer _gameContainer;
    private readonly MonopolyDiceSet _monopolyDice;
    private readonly HouseDice _houseDice;

    public override Task FinishGetSavedAsync()
    {
        LoadControls();
        _model.OtherActions.Clear(); //has to clear out here too obviously.
        //anything else needed is here.
        return Task.CompletedTask;
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
    public override async Task SetUpGameAsync(bool isBeginning)
    {
        LoadControls();
        SaveRoot.RollNumber = 1;
        if (FinishUpAsync == null)
        {
            throw new CustomBasicException("The loader never set the finish up code.  Rethink");
        }
        await FinishUpAsync(isBeginning);
    }
    async Task IMiscDataNM.MiscDataReceived(string status, string content)
    {
        switch (status) //can't do switch because we don't know what the cases are ahead of time.
        {
            //put in cases here.
            case "diceclicked":
                await SelectOneMainAsync(int.Parse(content));
                return;
            case "maindicelist":
                var dice = await _monopolyDice.GetDiceList(content);
                await ShowRollingRegularAsync(dice);
                Network!.IsEnabled = true;
                return;
            case "possiblecops":
                var cops = await js1.DeserializeObjectAsync<BasicList<EnumMiscType>>(content);
                ShowCopResults(cops);
                Network!.IsEnabled = true;
                return;
            case "housedice":
                var houses = await js1.DeserializeObjectAsync<BasicList<EnumMiscType>>(content);
                await ShowRollingHouseAsync(houses);
                return;
            case "utilitychosen":
                var utility = await js1.DeserializeObjectAsync<EnumUtilityType>(content);
                await ChoseUtilityAsync(utility);
                return;
            case "trainchosen":
                await ChoseTrainAsync();
                return;
            case "propertychosen":
                await ChosePropertyAsync(int.Parse(content));
                return;
            case "finishroll":
                await FinishRollingAsync();
                return;
            default:
                throw new CustomBasicException($"Nothing for status {status}  with the message of {content}");
        }
    }
    public override async Task EndTurnAsync()
    {
        //for now, nothing else.  later will do extra stuff for ending turn.
        //return base.EndTurnAsync();

        //do other things here.

        SingleInfo = PlayerList.GetWhoPlayer();
        int score = SaveRoot.GetTotalScoreInRound();
        SingleInfo.CurrentScore = score;
        SingleInfo.TotalScore += score;
        _houseDice.Value = EnumMiscType.None; //reset to none again.
        if (SingleInfo.TotalScore >= 20000)
        {
            //since this is the first player to reach 20000, the game is over and that player wins period.
            await ShowWinAsync();
            return;
        }
        WhoTurn = await PlayerList.CalculateWhoTurnAsync();
        await StartNewTurnAsync();
    }
    public override async Task StartNewTurnAsync()
    {
        PrepStartTurn(); //anything else is below.
        SaveRoot.RollNumber = 1;
        SaveRoot.NumberOfCops = 0;
        SaveRoot.TotalGos = 0;
        SaveRoot.NumberOfHouses = 0;
        SaveRoot.HasAtLeastOnePropertyMonopoly = false;
        SaveRoot.HasHotel = false;
        SaveRoot.CurrentScore = 0;
        SaveRoot.DiceList.Clear();
        SaveRoot.Owns.Clear();
        _model.OtherActions.Clear();
        await ContinueTurnAsync(); //most of the time, continue turn.  can change to what is needed
    }
    public async Task SelectOneMainAsync(int whichOne)
    {
        _monopolyDice.SelectDice(whichOne);
        await ContinueTurnAsync();
    }
    public async Task RollDiceAsync() //whoever turn it is will call this.
    {
        //the host will call this one.
        var firsts = _monopolyDice.RollDice();
        if (BasicData.MultiPlayer)
        {
            await _monopolyDice.SendMessageAsync("maindicelist", firsts);
        }
        await ShowRollingRegularAsync(firsts);
        //the host will continue.
        await StartCopRollAsync();
        await PossibleHouseRollAsync();
    }
    private async Task PossibleHouseRollAsync()
    {
        if (SaveRoot.HasAtLeastOnePropertyMonopoly)
        {
            var list = _houseDice.RollDice();
            if (BasicData.MultiPlayer)
            {
                await Network!.SendAllAsync("housedice", list);
            }
            await ShowRollingHouseAsync(list);
            return;
        }
        if (BasicData.MultiPlayer)
        {
            //this tells other players to finish roll.
            await Network!.SendAllAsync("finishroll");
        }
        await FinishRollingAsync();
    }
    private async Task StartCopRollAsync()
    {
        var list = SaveRoot.GetMiscResults(_rs);
        if (BasicData.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync("possiblecops", list);
        }
        ShowCopResults(list);
        //the player who goes has to decide what happens next.
        
    }
    private async Task FinishRollingAsync()
    {
        //not sure if there is anything else that is needed here.
        if (SaveRoot.NumberOfCops > 2)
        {
            SaveRoot.CurrentScore = 0;
        }
        await ContinueTurnAsync();
    }
    private void ShowCopResults(BasicList<EnumMiscType> cops)
    {
        _model.OtherActions = cops;
        if (_model.OtherActions.Any(x => x == EnumMiscType.Free))
        {
            SaveRoot.NumberOfCops--;
        }
        if (_model.OtherActions.Any(x => x == EnumMiscType.Go))
        {
            SaveRoot.CurrentScore += 200;
            SaveRoot.TotalGos++;
        }
        SaveRoot.NumberOfCops += _model.OtherActions.Count(x => x == EnumMiscType.Police);
    }
    private async Task ShowRollingRegularAsync(BasicList<BasicList<BasicDiceModel>> thisList)
    {
        await _monopolyDice.ShowRollingAsync(thisList);
        SaveRoot.RollNumber++;
    }
    private async Task ShowRollingHouseAsync(BasicList<EnumMiscType> list)
    {
        await _houseDice.ShowRollingAsync(list);
        if (_houseDice.Value == EnumMiscType.Free && SaveRoot.NumberOfCops > 0)
        {
            SaveRoot.NumberOfCops--;
        }
        if (_houseDice.Value == EnumMiscType.BrokenHouse)
        {
            if (SaveRoot.NumberOfHouses > 0)
            {
                SaveRoot.NumberOfHouses--;
            }
        }
        if (_houseDice.Value == EnumMiscType.RegularHouse)
        {
            if (SaveRoot.NumberOfHouses < 4 && SaveRoot.HasHotel == false)
            {
                SaveRoot.NumberOfHouses++;
            }
        }
        if (_houseDice.Value == EnumMiscType.Hotel)
        {
            if (SaveRoot.NumberOfHouses == 4)
            {
                SaveRoot.HasHotel = true;
                SaveRoot.NumberOfHouses = 0; //because you now have hotel.
            }
        }
        SaveRoot.CurrentScore = SaveRoot.GetTotalScoreInRound(); //because you have new houses.
        //both call this.
        //after this, finish roll no matter what.
        await FinishRollingAsync();
    }
    public async Task ChoseUtilityAsync(EnumUtilityType utility)
    {
        //has to assume the one selected dice will be the utility.
        var dice = SaveRoot.DiceList.GetSelectedItems().Single(); //let it blow up if it needs to.
        dice.IsSelected = false;
        OwnedModel own = new();
        own.UsedOn = EnumBasicType.Utility;
        if (dice.WhatDice == EnumBasicType.Chance)
        {
            own.WasChance = true;
        }
        own.Utility = utility;
        SaveRoot.Owns.Add(own);
        SaveRoot.DiceList.RemoveSpecificItem(dice);
        SaveRoot.DiceList.Sort();
        SaveRoot.CurrentScore = SaveRoot.GetTotalScoreInRound();
        await ContinueTurnAsync();
    }
    public async Task ChoseTrainAsync()
    {
        var list = SaveRoot.DiceList.GetSelectedItems();
        foreach (var item in list)
        {
            OwnedModel own = new();
            own.UsedOn = EnumBasicType.Railroad;
            if (item.WhatDice == EnumBasicType.Chance)
            {
                own.WasChance = true;
            }
            SaveRoot.Owns.Add(own);
            item.IsSelected = false;
            SaveRoot!.DiceList.RemoveSpecificItem(item);
        }
        SaveRoot!.DiceList.Sort();
        SaveRoot.CurrentScore = SaveRoot.GetTotalScoreInRound();
        await ContinueTurnAsync();
    }
    public async Task ChosePropertyAsync(int group)
    {
        var list = SaveRoot.DiceList.GetSelectedItems();
        foreach (var item in list)
        {
            OwnedModel own = new();
            own.Group = group;
            if (item.WhatDice == EnumBasicType.Chance)
            {
                own.WasChance = true;
            }
            SaveRoot.Owns.Add(own);
            item.IsSelected = false;
            SaveRoot!.DiceList.RemoveSpecificItem(item);
        }
        SaveRoot!.DiceList.Sort();
        SaveRoot.CurrentScore = SaveRoot.GetTotalScoreInRound();
        await ContinueTurnAsync();
    }
}