namespace MonopolyCardGame.Core.ViewModels;
[InstanceGame]
public partial class MonopolyCardGameMainViewModel : BasicCardGamesVM<MonopolyCardGameCardInformation>
{
    private readonly MonopolyCardGameMainGameClass _mainGame;
    private readonly MonopolyCardGameVMData _model;
    private readonly IToast _toast;
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
        _mainGame = mainGame;
        _model = viewModel;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        CommandContainer.ExecutingChanged += CommandContainer_ExecutingChanged;
        _model.TempSets1.SetClickedAsync += TempSets1_SetClickedAsync;
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
            _mainGame.SortTempHand(tempList);
            _isProcessing = false;
            return Task.CompletedTask;
        }
        _model.TempSets1!.AddCards(index, tempList);
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
        return _mainGame!.SaveRoot!.GameStatus == EnumWhatStatus.DrawOrTrade || _mainGame.SaveRoot.GameStatus == EnumWhatStatus.Either;
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
        _mainGame!.PlayerList!.ForEach(thisPlayer =>
        {
            if (_mainGame.SaveRoot!.GameStatus == EnumWhatStatus.LookOnly)
            {
                thisPlayer!.TradePile!.IsEnabled = false;
            }
            else if (thisPlayer.PlayerCategory == EnumPlayerCategory.Self)
            {
                thisPlayer.TradePile!.IsEnabled = true;
            }
            else
            {
                thisPlayer.TradePile!.IsEnabled = _mainGame.SaveRoot.GameStatus == EnumWhatStatus.Discard || _mainGame.SingleInfo!.MainHandList.Count == 9;
            }
        });
    }
    public bool CanResume => _mainGame!.SaveRoot!.GameStatus == EnumWhatStatus.LookOnly;
    [Command(EnumCommandCategory.Game)]
    public async Task ResumeAsync()
    {
        await _mainGame!.EndTurnAsync();
    }
    public bool CanGoOut => CanEnableDeck();

    [Command(EnumCommandCategory.Game)]
    public async Task GoOutAsync()
    {
        if (_mainGame!.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Not Self.  Rethink");
        }
        var newList = _mainGame.SingleInfo.MainHandList.ToRegularDeckDict();
        if (_mainGame.CanGoOut(newList) == false)
        {
            _toast.ShowUserErrorToast("Sorry, you cannot go out");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("goout");
        }
        await _mainGame.ProcessGoingOutAsync();
    }
    [Command(EnumCommandCategory.Game)]
    public void PutBack()
    {
        var thisList = _model.TempSets1!.ListSelectedObjects();
        thisList.ForEach(thisCard =>
        {
            thisCard.IsSelected = false;
            _model.TempSets1.RemoveObject(thisCard.Deck);
        });
        _mainGame.SortTempHand(thisList);
    }
    [Command(EnumCommandCategory.Game)]
    public async Task ManuallyPlaySetsAsync()
    {
        bool rets = _mainGame.HasAllValidMonopolies();
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
        var firsts = _mainGame.ListValidSets();
        var list = MonopolyCardGameMainGameClass.GetSetInfo(firsts);
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            BasicList<string> newList = new();
            await firsts.ForEachAsync(async thisTemp =>
            {
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                    var thisStr = await js.SerializeObjectAsync(tempList);
                    newList.Add(thisStr);
                }
            });
            await _mainGame.Network!.SendSeveralSetsAsync(newList, "finishedsets");
        }
        await _mainGame.FinishManualProcessingAsync(list);
    }
}