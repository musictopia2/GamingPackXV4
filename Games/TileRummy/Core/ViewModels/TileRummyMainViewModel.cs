namespace TileRummy.Core.ViewModels;
[InstanceGame]
public partial class TileRummyMainViewModel : BasicMultiplayerMainVM
{
    private readonly TileRummyMainGameClass _mainGame; //if we don't need, delete.
    private readonly IToast _toast;
    public TileRummyVMData VMData { get; set; }
    public TileRummyMainViewModel(CommandContainer commandContainer,
        TileRummyMainGameClass mainGame,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        TileRummyVMData data,
        IToast toast
        )
        : base(commandContainer, mainGame, basicData, test, resolver, aggregator)
    {
        _mainGame = mainGame;
        VMData = data;
        _toast = toast;
        VMData.TempSets.Init(this);
        VMData.TempSets.ClearBoard();
        VMData.Pool1.DrewTileAsync = DrewTileAsync;
        VMData.MainSetsClickedAsync = MainSets1_SetClickedAsync;
        VMData.TempSetsClickedAsync = TempSets_SetClickedAsync;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    public PoolCP GetPool => VMData.Pool1;
    public PlayerCollection<TileRummyPlayerItem> GetPlayers => _mainGame.SaveRoot.PlayerList;
    public TempSetsObservable<EnumColorType, EnumColorType, TileInfo> GetTempTiles => VMData.TempSets;
    public HandObservable<TileInfo> GetHand => VMData.PlayerHand1;
    public MainSets GetMainSets => VMData.MainSets1;
    public override bool CanEndTurn()
    {
        bool didPlay = VMData.MainSets1!.PlayedAtLeastOneFromHand();
        return didPlay == true || _mainGame.SaveRoot.FirstInit || VMData.Pool1!.HasDrawn() || VMData.Pool1.HasTiles() == false;
    }
    public override async Task EndTurnAsync()
    {
        var thisCol = _mainGame!.GetSelectedList();
        if (thisCol.Count > 0)
        {
            _toast.ShowUserErrorToast("Must either use the tiles selected or unselect the tiles before ending turn");
            return;
        }
        bool didPlay;
        bool valids;
        didPlay = VMData.MainSets1!.PlayedAtLeastOneFromHand();
        valids = _mainGame.ValidPlay();
        if (_mainGame.BasicData!.MultiPlayer)
        {
            SendCustom thisEnd = new()
            {
                DidPlay = didPlay,
                ValidSets = valids
            };
            await _mainGame.Network!.SendAllAsync("endcustom", thisEnd); //i think
        }
        await _mainGame.EndTurnAsync(didPlay, valids);
    }
    private bool _isProcessing;
    private async Task DrewTileAsync(TileInfo thisTile)
    {
        if (_mainGame.BasicData.MultiPlayer)
        {
            SendDraw thisDraw = new()
            {
                Deck = thisTile.Deck,
                FromEnd = false
            };
            await _mainGame.Network!.SendAllAsync("drewtile", thisDraw);
        }
        await _mainGame!.DrawTileAsync(thisTile, false);
    }
    private async Task MainSets1_SetClickedAsync(int setNumber, int section, int deck)
    {
        if (setNumber == 0)
        {
            throw new CustomBasicException("If the set is 0, rethinking is required");
        }
        var thisSet = VMData.MainSets1!.GetIndividualSet(setNumber);
        var thisCol = _mainGame!.GetSelectedList();
        if (thisCol.Count == 0)
        {
            if (deck == 0 || thisSet.HandList.Count < 2)
            {
                if (_mainGame.BasicData!.MultiPlayer)
                {
                    await _mainGame.Network!.SendAllAsync("removeentireset", setNumber);
                }
                await _mainGame.RemoveEntireSetAsync(setNumber);
                return;
            }
            if (_mainGame.BasicData!.MultiPlayer)
            {
                SendSet thisSend = new()
                {
                    Index = setNumber,
                    Tile = deck
                };
                await _mainGame.Network!.SendAllAsync("removeonefromset", thisSend);
            }
            await _mainGame.RemoveTileFromSetAsync(setNumber, deck);
            return;
        }
        if (thisCol.Count > 1)
        {
            _toast.ShowUserErrorToast("Can only add one tile to the set at a time");
            return;
        }
        var thisTile = thisCol.First();
        var newPos = thisSet.PositionToPlay(thisTile, section);
        if (_mainGame.BasicData!.MultiPlayer)
        {
            SendSet finSend = new()
            {
                Index = setNumber,
                Position = newPos,
                Tile = thisTile.Deck
            };
            await _mainGame.Network!.SendAllAsync("addtoset", finSend);
        }
        if (VMData.TempSets!.HasObject(thisTile.Deck))
        {
            VMData.TempSets.RemoveObject(thisTile.Deck);
        }
        else
        {
            _mainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(thisTile.Deck);
        }
        await _mainGame.AddToSetAsync(setNumber, thisTile, newPos);
    }
    private Task TempSets_SetClickedAsync(int index)
    {
        if (_isProcessing == true)
        {
            return Task.CompletedTask;
        }
        _isProcessing = true;
        var tempList = VMData.PlayerHand1!.ListSelectedObjects(true);
        //var player = _mainGame.PlayerList.GetSelf();
        //var tt = player.MainHandList.GetSelectedItems();
        VMData.TempSets!.AddCards(index, tempList);
        _isProcessing = false;
        CommandContainer.UpdateAll();
        return Task.CompletedTask;
    }
    #region "Command Processes"
    public bool CanCreateFirstSets => !_mainGame.SingleInfo!.InitCompleted;
    [Command(EnumCommandCategory.Game)]
    public async Task CreateFirstSetsAsync()
    {
        bool rets = _mainGame!.HasInitialSets(out BasicList<TempInfo> thisList);
        if (rets == false)
        {
            _toast.ShowUserErrorToast("Sorry, you do not have the initial set needed.  The point values has to be at least 30");
            return;
        }
        BasicList<string> newList = new();
        await thisList.ForEachAsync(async thisTemp =>
        {
            if (_mainGame.BasicData!.MultiPlayer == true)
            {
                var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                SendCreateSet thisSend = new()
                {
                    CardList = tempList,
                    WhatSet = thisTemp.WhatSet
                };
                var thisStr = await js.SerializeObjectAsync(thisSend);
                newList.Add(thisStr);
            }
            VMData.TempSets.ClearBoard(thisTemp.TempSet);
            await _mainGame.CreateNewSetAsync(thisTemp, true);
        });
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendSeveralSetsAsync(newList, "laiddowninitial");
        }
        await _mainGame.InitialCompletedAsync();
    }
    public bool CanCreateNewSet => _mainGame.SingleInfo!.InitCompleted;
    [Command(EnumCommandCategory.Game)]
    public async Task CreateNewSetAsync()
    {
        var thisCol = _mainGame!.GetSelectedList();
        if (thisCol.Count < 3)
        {
            _toast.ShowUserErrorToast("You must have at least 3 tiles in order to create a set");
            return;
        }
        _mainGame.HasValidSet(thisCol, out int firstNumber, out int secondNumber);
        TempInfo thisTemp = new()
        {
            CardList = thisCol,
            FirstNumber = firstNumber,
            SecondNumber = secondNumber
        };
        if (thisTemp.FirstNumber == -1)
        {
            thisTemp.WhatSet = EnumWhatSets.Kinds;
        }
        else
        {
            thisTemp.WhatSet = EnumWhatSets.Runs;
        }
        if (thisTemp.FirstNumber == -1)
        {
            thisTemp.WhatSet = EnumWhatSets.Kinds;
        }
        else
        {
            thisTemp.WhatSet = EnumWhatSets.Runs;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            SendCreateSet thisSend = new()
            {
                CardList = thisCol.GetDeckListFromObjectList(),
                FirstNumber = thisTemp.FirstNumber,
                SecondNumber = thisTemp.SecondNumber,
                WhatSet = thisTemp.WhatSet
            };
            await _mainGame.Network!.SendAllAsync("createnewset", thisSend);
        }
        thisCol.ForEach(thisTile =>
        {
            if (VMData.TempSets.HasObject(thisTile.Deck))
            {
                VMData.TempSets.RemoveObject(thisTile.Deck);
            }
            else
            {
                _mainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(thisTile.Deck);
            }
        });
        await _mainGame.CreateNewSetAsync(thisTemp, false);
    }
    public bool CanUndoMove => _mainGame.SaveRoot.DidExpand;
    [Command(EnumCommandCategory.Game)]
    public async Task UndoMoveAsync()
    {
        if (_mainGame.BasicData!.MultiPlayer)
        {
            await _mainGame.Network!.SendAllAsync("undomove");
        }
        await _mainGame.UndoMoveAsync();
    }
    #endregion
}