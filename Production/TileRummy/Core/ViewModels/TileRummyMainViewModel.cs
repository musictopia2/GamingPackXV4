namespace TileRummy.Core.ViewModels;
[InstanceGame]
public partial class TileRummyMainViewModel(CommandContainer commandContainer,
        TileRummyMainGameClass mainGame,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        TileRummyVMData data,
        IToast toast) : BasicMultiplayerMainVM(commandContainer, mainGame, basicData, resolver, aggregator)
{
    public TileRummyVMData VMData { get; set; } = data;
    protected override void AdditionalInitialize()
    {
        VMData.TempSets.Init(this);
        VMData.TempSets.ClearBoard();
        VMData.Pool1.DrewTileAsync = DrewTileAsync;
        VMData.MainSetsClickedAsync = MainSets1_SetClickedAsync;
        VMData.TempSetsClickedAsync = TempSets_SetClickedAsync;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    public PoolCP GetPool => VMData.Pool1;
    public PlayerCollection<TileRummyPlayerItem> GetPlayers => mainGame.PlayerList;
    public TempSetsObservable<EnumColorType, EnumColorType, TileInfo> GetTempTiles => VMData.TempSets;
    public HandObservable<TileInfo> GetHand => VMData.PlayerHand1;
    public MainSets GetMainSets => VMData.MainSets1;
    public override bool CanEndTurn()
    {
        bool didPlay = VMData.MainSets1!.PlayedAtLeastOneFromHand();
        return didPlay == true || mainGame.SaveRoot.FirstInit || VMData.Pool1!.HasDrawn() || VMData.Pool1.HasTiles() == false;
    }
    public override async Task EndTurnAsync()
    {
        var thisCol = mainGame!.GetSelectedList();
        if (thisCol.Count > 0)
        {
            toast.ShowUserErrorToast("Must either use the tiles selected or unselect the tiles before ending turn");
            return;
        }
        bool didPlay;
        bool valids;
        didPlay = VMData.MainSets1!.PlayedAtLeastOneFromHand();
        valids = mainGame.ValidPlay();
        if (mainGame.BasicData!.MultiPlayer)
        {
            SendCustom thisEnd = new()
            {
                DidPlay = didPlay,
                ValidSets = valids
            };
            await mainGame.Network!.SendAllAsync("endcustom", thisEnd); //i think
        }
        await mainGame.EndTurnAsync(didPlay, valids);
    }
    private bool _isProcessing;
    private async Task DrewTileAsync(TileInfo thisTile)
    {
        if (mainGame.BasicData.MultiPlayer)
        {
            SendDraw thisDraw = new()
            {
                Deck = thisTile.Deck,
                FromEnd = false
            };
            await mainGame.Network!.SendAllAsync("drewtile", thisDraw);
        }
        await mainGame!.DrawTileAsync(thisTile, false);
    }
    private async Task MainSets1_SetClickedAsync(int setNumber, int section, int deck)
    {
        if (setNumber == 0)
        {
            throw new CustomBasicException("If the set is 0, rethinking is required");
        }
        var thisSet = VMData.MainSets1!.GetIndividualSet(setNumber);
        var thisCol = mainGame!.GetSelectedList();
        if (thisCol.Count == 0)
        {
            if (deck == 0 || thisSet.HandList.Count < 2)
            {
                if (mainGame.BasicData!.MultiPlayer)
                {
                    await mainGame.Network!.SendAllAsync("removeentireset", setNumber);
                }
                await mainGame.RemoveEntireSetAsync(setNumber);
                return;
            }
            if (mainGame.BasicData!.MultiPlayer)
            {
                SendSet thisSend = new()
                {
                    Index = setNumber,
                    Tile = deck
                };
                await mainGame.Network!.SendAllAsync("removeonefromset", thisSend);
            }
            await mainGame.RemoveTileFromSetAsync(setNumber, deck);
            return;
        }
        if (thisCol.Count > 1)
        {
            toast.ShowUserErrorToast("Can only add one tile to the set at a time");
            return;
        }
        var thisTile = thisCol.First();
        var newPos = thisSet.PositionToPlay(thisTile, section);
        if (mainGame.BasicData!.MultiPlayer)
        {
            SendSet finSend = new()
            {
                Index = setNumber,
                Position = newPos,
                Tile = thisTile.Deck
            };
            await mainGame.Network!.SendAllAsync("addtoset", finSend);
        }
        if (VMData.TempSets!.HasObject(thisTile.Deck))
        {
            VMData.TempSets.RemoveObject(thisTile.Deck);
        }
        else
        {
            mainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(thisTile.Deck);
        }
        await mainGame.AddToSetAsync(setNumber, thisTile, newPos);
    }
    private Task TempSets_SetClickedAsync(int index)
    {
        if (_isProcessing == true)
        {
            return Task.CompletedTask;
        }
        _isProcessing = true;
        var tempList = VMData.PlayerHand1!.ListSelectedObjects(true);
        //var player = mainGame.PlayerList.GetSelf();
        //var tt = player.MainHandList.GetSelectedItems();
        VMData.TempSets!.AddCards(index, tempList);
        _isProcessing = false;
        return Task.CompletedTask;
    }
    #region "Command Processes"
    public bool CanCreateFirstSets => !mainGame.SingleInfo!.InitCompleted;
    [Command(EnumCommandCategory.Game)]
    public async Task CreateFirstSetsAsync()
    {
        bool rets = mainGame!.HasInitialSets(out BasicList<TempInfo> thisList);
        if (rets == false)
        {
            toast.ShowUserErrorToast("Sorry, you do not have the initial set needed.  The point values has to be at least 30");
            return;
        }
        BasicList<string> newList = [];
        await thisList.ForEachAsync(async thisTemp =>
        {
            if (mainGame.BasicData!.MultiPlayer == true)
            {
                var tempList = thisTemp.CardList.GetDeckListFromObjectList();
                SendCreateSet thisSend = new()
                {
                    CardList = tempList,
                    WhatSet = thisTemp.WhatSet
                };
                var thisStr = await js1.SerializeObjectAsync(thisSend);
                newList.Add(thisStr);
            }
            VMData.TempSets.ClearBoard(thisTemp.TempSet);
            await mainGame.CreateNewSetAsync(thisTemp, true);
        });
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendSeveralSetsAsync(newList, "laiddowninitial");
        }
        await mainGame.InitialCompletedAsync();
    }
    public bool CanCreateNewSet => mainGame.SingleInfo!.InitCompleted;
    [Command(EnumCommandCategory.Game)]
    public async Task CreateNewSetAsync()
    {
        var thisCol = mainGame!.GetSelectedList();
        if (thisCol.Count < 3)
        {
            toast.ShowUserErrorToast("You must have at least 3 tiles in order to create a set");
            return;
        }
        mainGame.HasValidSet(thisCol, out int firstNumber, out int secondNumber);
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
        if (mainGame.BasicData!.MultiPlayer)
        {
            SendCreateSet thisSend = new()
            {
                CardList = thisCol.GetDeckListFromObjectList(),
                FirstNumber = thisTemp.FirstNumber,
                SecondNumber = thisTemp.SecondNumber,
                WhatSet = thisTemp.WhatSet
            };
            await mainGame.Network!.SendAllAsync("createnewset", thisSend);
        }
        thisCol.ForEach(thisTile =>
        {
            if (VMData.TempSets.HasObject(thisTile.Deck))
            {
                VMData.TempSets.RemoveObject(thisTile.Deck);
            }
            else
            {
                mainGame.SingleInfo!.MainHandList.RemoveObjectByDeck(thisTile.Deck);
            }
        });
        await mainGame.CreateNewSetAsync(thisTemp, false);
    }
    public bool CanUndoMove => mainGame.SaveRoot.DidExpand;
    [Command(EnumCommandCategory.Game)]
    public async Task UndoMoveAsync()
    {
        if (mainGame.BasicData!.MultiPlayer)
        {
            await mainGame.Network!.SendAllAsync("undomove");
        }
        await mainGame.UndoMoveAsync();
    }
    #endregion
}