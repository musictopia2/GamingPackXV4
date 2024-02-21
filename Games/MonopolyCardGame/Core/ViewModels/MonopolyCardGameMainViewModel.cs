namespace MonopolyCardGame.Core.ViewModels;
[InstanceGame]
public partial class MonopolyCardGameMainViewModel : BasicCardGamesVM<MonopolyCardGameCardInformation>
{
    public readonly MonopolyCardGameMainGameClass MainGame;
    private readonly MonopolyCardGameVMData _model;
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
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        var player = MainGame.PlayerList.GetSelf();
        player.DoInit();
        CommandContainer.ExecutingChanged = CommandContainer_ExecutingChanged;
        _model.TempSets1.SetClickedAsync = TempSets1_SetClickedAsync;
        //may need code for after choosing card for tempsets.
        //_model.TempSets1.
        _model.TempSets1.Init(this);
        CreateCommands(commandContainer);
    }
    private bool _isProcessing;
    private Task TempSets1_SetClickedAsync(int index)
    {
        if (_isProcessing == true)
        {
            return Task.CompletedTask;
        }
        _isProcessing = true;
        var tempList = _model.TempHand1!.ListSelectedObjects(true);
        if (tempList.Any(x => x.WhatCard == EnumCardType.IsGo || x.WhatCard == EnumCardType.IsMr))
        {
            _toast.ShowUserErrorToast("You cannot place gos or mr monopolies into the tempsets to make a monopoly");
            //_model.TempHand1.HandList.AddRange(tempList);
            MainGame.SortTempHand(tempList);
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
        CommandContainer.ExecutingChanged -= CommandContainer_ExecutingChanged;
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
    private void CommandContainer_ExecutingChanged()
    {
        if (CommandContainer!.IsExecuting)
        {
            return;
        }
        MainGame!.PlayerList!.ForEach(thisPlayer =>
        {
            if (MainGame.SaveRoot!.GameStatus == EnumWhatStatus.LookOnly)
            {
                thisPlayer!.TradePile!.IsEnabled = false;
            }
            else if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisPlayer.TradePile!.IsEnabled = true;
            }
            else
            {
                thisPlayer.TradePile!.IsEnabled = MainGame.SaveRoot.GameStatus == EnumWhatStatus.Discard || MainGame.SingleInfo!.MainHandList.Count == 9;
            }
        });
    }
    public bool CanResume => MainGame!.SaveRoot!.GameStatus == EnumWhatStatus.LookOnly;
    [Command(EnumCommandCategory.Game)]
    public async Task ResumeAsync()
    {
        await MainGame!.EndTurnAsync();
    }
    public void OrganizeCards()
    {
        if (MainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Not Self.  Rethink");
        }
        PreviousStatus = MainGame.SaveRoot.GameStatus;
        MainGame.SaveRoot.GameStatus = EnumWhatStatus.Other;
        MainGame.SaveRoot.ManuelStatus = EnumManuelStatus.OrganizingCards;
        if (MainGame.OrganizedAtLeastsOnce == false)
        {
            MainGame.PopulateManuelCards();
        }
        else
        {
            MainGame.SingleInfo.TempHands.Clear();
            MainGame.SingleInfo.TempHands.AddRange(MainGame.SingleInfo.MainHandList); //hopefully this simple
        }
        //if (_model.TempHand1.)
    }
    public bool CanGoOut => CanEnableDeck();

    [Command(EnumCommandCategory.Game)]
    public void GoOut()
    {
        if (MainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Not Self.  Rethink");
        }
        _toast.ShowUserErrorToast("Has to rethink the processes for going out now");


        //PreviousStatus = MainGame.SaveRoot.GameStatus;
        //MainGame.SaveRoot.GameStatus = EnumWhatStatus.Other;
        //MainGame.SaveRoot.ManuelStatus = EnumManuelStatus.InitiallyGoingOut;
        //MainGame.PopulateManuelCards();
        //if i go out and go back again, choose again.
        //var newList = MainGame.SingleInfo.MainHandList.ToRegularDeckDict();
        //if (MainGame.CanGoOut(newList) == false)
        //{
        //    _toast.ShowUserErrorToast("Sorry, you cannot go out");
        //    return;
        //}
        //if (MainGame.BasicData!.MultiPlayer)
        //{
        //    await MainGame.Network!.SendAllAsync("goout");
        //}
        //await MainGame.ProcessGoingOutAsync();
    }
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
        MainGame.SortTempHand(thisList);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ManuallyPlaySetsAsync()
    {
        bool rets = MainGame.HasAllValidMonopolies();
        if (rets == false)
        {
            _toast.ShowUserErrorToast("You do not have valid monopolies");
            return;
        }
        if (_model.TempHand1.HandList.Any(x => x.WhatCard == EnumCardType.IsChance))
        {
            _toast.ShowUserErrorToast("You have to use up all the chances");
            return;
        }
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
}