namespace MonopolyCardGame.Core.ViewModels;
[InstanceGame]
public partial class MonopolyCardGameMainViewModel : BasicCardGamesVM<MonopolyCardGameCardInformation>
{
    public readonly MonopolyCardGameMainGameClass MainGame;
    private readonly MonopolyCardGameVMData _model;
    private readonly TestOptions _test;
    private readonly IToast _toast;
    public EnumWhatStatus PreviousStatus { get; set; }
    public MonopolyCardGameMainViewModel(CommandContainer commandContainer,
        MonopolyCardGameMainGameClass mainGame,
        MonopolyCardGameVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        MainGame = mainGame;
        _model = viewModel;
        _test = test;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        var player = MainGame.PlayerList.GetSelf();
        player.DoInit();
        //CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        _model.TempSets1.SetClickedAsync = TempSets1_SetClickedAsync;
        HookUpEnableTradeProcesses();
        
        //_toast.ShowInfoToast("Initializing");
        //may need code for after choosing card for tempsets.
        //_model.TempSets1.
        _model.TempSets1.Init(this);
        CreateCommands(commandContainer);
    }

    private void HookUpEnableTradeProcesses()
    {
        foreach (var player in MainGame.PlayerList)
        {
            if (player.TradePile is not null)
            {
                player.TradePile!.SendEnableProcesses(this, () =>
                {
                    if (MainGame.SaveRoot!.GameStatus == EnumWhatStatus.LookOnly)
                    {
                        return false;
                    }
                    if (player.PlayerCategory == EnumPlayerCategory.Self && MainGame.SaveRoot.GameStatus != EnumWhatStatus.EndTurn)
                    {
                        return true;
                    }
                    return player.TradePile!.IsEnabled = MainGame.SingleInfo!.ObjectCount == 9;
                    //return false;
                });
            }
        }
    }

    private bool _isProcessing;
    private Task TempSets1_SetClickedAsync(int index)
    {
        if (_isProcessing == true)
        {
            return Task.CompletedTask;
        }
        _isProcessing = true;
        var tempList = _model.TempHand1!.ListSelectedObjects(false);
        if (tempList.Any(x => x.WhatCard == EnumCardType.IsGo || x.WhatCard == EnumCardType.IsMr))
        {
            _toast.ShowUserErrorToast("You cannot place gos or mr monopolies into the tempsets to make a monopoly");
            foreach (var item in tempList)
            {
                item.IsSelected = false; //unselect them automatically in this case.
            }
            _isProcessing = false;
            return Task.CompletedTask;
        }
        _model.TempSets1!.AddCards(index, tempList);
        foreach (var item in tempList)
        {
            MainGame.SingleInfo!.MainHandList.RemoveSpecificItem(item);
            //not sure about if additional cards are being account for this time (?)
        }
        _model.AdditionalInfo1.Clear(); //i think.
        _isProcessing = false;
        return Task.CompletedTask;
    }

    partial void CreateCommands(CommandContainer command);
    protected override Task TryCloseAsync()
    {
        //CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
        _model.TempSets1.SetClickedAsync -= TempSets1_SetClickedAsync;
        return base.TryCloseAsync();
    }
    protected override bool CanEnableDeck()
    {
        return MainGame!.SaveRoot!.GameStatus == EnumWhatStatus.DrawOrTrade || MainGame.SaveRoot.GameStatus == EnumWhatStatus.Either;
    }
    protected override bool CanEnablePile1()
    {
        return false;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    protected override Task OnConsiderSelectOneCardAsync(MonopolyCardGameCardInformation payLoad)
    {
        if (payLoad.Deck == _model.AdditionalInfo1!.CurrentCard.Deck)
        {
            _model.AdditionalInfo1.Clear();
        }
        else
        {
            _model.AdditionalInfo1.AdditionalInfo(payLoad.Deck);
        }
        return Task.CompletedTask;
    }
    public override bool CanEndTurn() => MainGame.SaveRoot.GameStatus == EnumWhatStatus.EndTurn;
    public override async Task EndTurnAsync()
    {
        await MainGame.EndTurnAsync();
    }
    //private void CommandContainer_ExecutingChanged()
    //{
    //    if (CommandContainer!.IsExecuting)
    //    {
    //        return;
    //    }
    //    CheckTradePileStatus();
    //}
    //public void CheckTradePileStatus()
    //{
    //    MainGame!.PlayerList!.ForEach(thisPlayer =>
    //    {
    //        if (MainGame.SaveRoot!.GameStatus == EnumWhatStatus.LookOnly)
    //        {
    //            thisPlayer!.TradePile!.IsEnabled = false;
    //        }
    //        else if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
    //        {
    //            thisPlayer.TradePile!.IsEnabled = true;
    //        }
    //        else
    //        {
    //            thisPlayer.TradePile!.IsEnabled = MainGame.SingleInfo!.ObjectCount == 9;
    //        }
    //    });
    //}
    public bool CanResume => MainGame!.SaveRoot!.GameStatus == EnumWhatStatus.LookOnly;
    [Command(EnumCommandCategory.Game)]
    public async Task ResumeAsync()
    {
        await MainGame!.EndTurnAsync();
    }
    public bool CanOrganizeCards
    {
        get
        {
            if (MainGame.SaveRoot.GameStatus == EnumWhatStatus.LookOnly)
            {
                return false;
            }
            return true; //i think the only case you cannot organize cards from that screen is if its look only.
        }
    }
    [Command(EnumCommandCategory.Game)]
    public void OrganizeCards()
    {
        if (MainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Not Self.  Rethink");
        }
        PreviousStatus = MainGame.SaveRoot.GameStatus;
        MainGame.SaveRoot.GameStatus = EnumWhatStatus.Other;
        MainGame.SaveRoot.ManuelStatus = EnumManuelStatus.OrganizingCards;
        //if (MainGame.OrganizedAtLeastsOnce == false)
        //{
        //    MainGame.PopulateManuelCards();
        //}
        //else
        //{
        //    MainGame.SingleInfo.TempHands.Clear();
        //    MainGame.SingleInfo.TempHands.AddRange(MainGame.SingleInfo.MainHandList); //hopefully this simple
        //}
        //if (_model.TempHand1.)
    }
    public bool CanGoOut => CanEnableDeck();
    [Command(EnumCommandCategory.Game)]
    public async Task GoOutAsync()
    {
        if (MainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Not Self.  Rethink");
        }
        //_toast.ShowUserErrorToast("Has to rethink the processes for going out now");
        if (_test.EndRoundEarly == false)
        {
            int left = MainGame.SingleInfo.MainHandList.Count(x => x.WhatCard != EnumCardType.IsGo && x.WhatCard != EnumCardType.IsMr);
            if (left > 0)
            {
                _toast.ShowUserErrorToast("You cannot go out because you have cards left in your hand that is not a go or mr. monopoly");
                return;
            }
            if (_model.HasAllValidMonopolies == false)
            {
                _toast.ShowUserErrorToast("You cannot go out one of the sets was either not a valid monopoly or improperly placed houses or hotels");
                return;
            }
        }
        
        //at this stage, can't calculate scores because you get 5 more cards.
        if (MainGame.BasicData!.MultiPlayer)
        {
            await MainGame.Network!.SendAllAsync("goout");
        }
        await MainGame.ProcessGoingOutAsync();
    }
    //[Command(EnumCommandCategory.Game)]
    //public void OrganizeCards()
    //{

    //}
    [Command(EnumCommandCategory.Game)]
    public void PutBack()
    {
        var thisList = _model.TempSets1!.ListSelectedObjects();
        thisList.ForEach(thisCard =>
        {
            thisCard.IsSelected = false;
            _model.TempSets1.RemoveObject(thisCard.Deck);
            MainGame.SingleInfo!.AdditionalCards.RemoveSpecificItem(thisCard); //this may be needed (?)
            MainGame.SingleInfo.MainHandList.Add(thisCard);

            //not sure about cards going into your hand though (?)
        });
        _model.AdditionalInfo1.Clear(); //i think
        MainGame.SortCards();
        //MainGame.SortTempHand(thisList);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ManuallyPlaySetsAsync()
    {
        bool rets;
        if (_test.EndRoundEarly == false)
        {
            if (MainGame.SingleInfo!.HasMonopolyInHand)
            {
                _toast.ShowUserErrorToast("You have at least one monopoly in hand to play.  You must play them manually");
                return;
            }
            rets = MainGame.HasAllValidMonopolies();
            if (rets == false)
            {
                //even if you are not the player going out, if there is something not valid, you must put back to the hand first.
                _toast.ShowUserErrorToast("You do not have valid monopolies");
                return;
            }
        }
        if (MainGame.SaveRoot.ManuelStatus == EnumManuelStatus.Final)
        {
            if (_test.EndRoundEarly == false)
            {
                if (_model.TempHand1.HandList.Any(x => x.WhatCard == EnumCardType.IsChance))
                {
                    _toast.ShowUserErrorToast("You have to use up all the chances");
                    return;
                }
                if (_model.TempHand1.HandList.Any(x => x.WhatCard == EnumCardType.IsToken))
                {
                    _toast.ShowUserErrorToast("You have to use up all your tokens");
                    return;
                }
            }
            await FinishManuelSetsAsync();
            return;
        }
        if (_test.EndRoundEarly == false)
        {
            rets = _model.HasAnyMonopolyPlayed;
            if (rets)
            {
                if (_model.TempHand1.HandList.Any(x => x.WhatCard == EnumCardType.IsToken))
                {
                    _toast.ShowUserErrorToast("You have to use up all your tokens because you played at least one monopoly");
                    return;
                }
            }
        }
        await FinishManuelSetsAsync();
    }
    private async Task FinishManuelSetsAsync()
    {
        var firsts = MainGame.ListValidSets();
        var list = MonopolyCardGameMainGameClass.GetSetInfo(firsts);
        if (MainGame.BasicData!.MultiPlayer == true)
        {
            BasicList<string> newList = [];
            await firsts.ForEachAsync(async thisTemp =>
            {
                if (MainGame.BasicData!.MultiPlayer == true)
                {
                    var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                    var thisStr = await js1.SerializeObjectAsync(tempList);
                    newList.Add(thisStr);
                }
            });
            await MainGame.Network!.SendSeveralSetsAsync(newList, "finishedsets");
        }
        await MainGame.FinishManualProcessingAsync(list);
    }
    public async Task TradeAsync(TradeModel trade)
    {
        if (MainGame.BasicData!.MultiPlayer)
        {
            SendTrade sent = new()
            {
                Player = trade.OpponentPlayer,
                CardList = trade.OpponentReceive.ToRegularDeckDict().GetDeckListFromObjectList()
                //WhoTurnReceive = trade.YouReceive.ToRegularDeckDict().GetDeckListFromObjectList(),
                //OtherReceive = trade.OpponentReceive.ToRegularDeckDict().GetDeckListFromObjectList()
            };
            await MainGame.Network!.SendAllAsync("trade2", sent);
        }
        var yourList = trade.OpponentReceive.ToRegularDeckDict();
        MonopolyCardGamePlayerItem opponent = MainGame.PlayerList[trade.OpponentPlayer];
        MainGame.ProcessTrade(opponent.TradePile!, yourList, MainGame.SingleInfo!.TradePile!);
        if (MainGame.SingleInfo.ObjectCount == 10)
        {
            MainGame.SaveRoot.GameStatus = EnumWhatStatus.EndTurn;
            //await MainGame.EndTurnAsync(); //try this way (?)
            //await EndTurnAsync();
            //return;
        }
        await MainGame.ContinueTurnAsync();
    }
}