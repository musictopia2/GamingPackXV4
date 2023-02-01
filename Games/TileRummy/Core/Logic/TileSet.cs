﻿
namespace TileRummy.Core.Logic;
public class TileSet : SetInfo<EnumColorType, EnumColorType, TileInfo, SavedSet>
{
    private bool _isNew;
    private EnumWhatSets _setType;
    private readonly RummyProcesses<EnumColorType, EnumColorType, TileInfo> _rummys;
    public TileSet(CommandContainer command, RummyProcesses<EnumColorType, EnumColorType, TileInfo> rummys) : base(command)
    {
        _rummys = rummys;
    }
    public override void LoadSet(SavedSet payLoad)
    {
        HandList.ReplaceRange(payLoad.TileList);
        _isNew = payLoad.IsNew;
        _setType = payLoad.SetType;
    }
    public override SavedSet SavedSet()
    {
        SavedSet output = new();
        output.TileList = HandList.ToRegularDeckDict();
        output.IsNew = _isNew;
        output.SetType = _setType;
        return output;
    }
    protected override bool CanClickMainBoard()
    {
        return false;
    }
    public override bool CanClickSingleObject()
    {
        return true;
    }
    public override void EndTurn()
    {
        _isNew = false;
        HandList.ForEach(thisTile => thisTile.WhatDraw = EnumDrawType.IsNone);
        base.EndTurn();
    }
    private static DeckRegularDict<TileInfo> GetWildList(IDeckDict<TileInfo> tempList) => tempList.Where(items => items.IsJoker).ToRegularDeckDict();
    public int PositionToPlay(TileInfo thisTile, int position)
    {
        if (_setType == EnumWhatSets.Kinds)
        {
            return position;
        }
        if (thisTile.IsJoker == true)
        {
            return position;
        }
        int newPos;
        if (position == 1)
        {
            newPos = 2;
        }
        else
        {
            newPos = 1;
        }
        TileInfo newTile;
        if (newPos == 1)
        {
            newTile = HandList.First();
        }
        else
        {
            newTile = HandList.Last();
        }
        if (newTile.Number == 13)
        {
            return 1;
        }
        if (newTile.Number == 1)
        {
            return 2;
        }
        if (((newTile.Number + 1) == thisTile.Number) & (newPos == 2))
        {
            return newPos;
        }
        if (newTile.Number == 1)
        {
            return 2;
        }
        if (((newTile.Number - 1) == thisTile.Number) & (newPos == 1))
        {
            return newPos;
        }
        return position;
    }
    public void CreateSet(IDeckDict<TileInfo> thisCol, EnumWhatSets whatType)
    {
        _setType = whatType;
        _isNew = true;
        TileRummySaveInfo saveRoot = aa1.Resolver!.Resolve<TileRummySaveInfo>();
        if (thisCol.Count == 0)
        {
            throw new CustomBasicException("There must be at least one item to create a new set");
        }
        foreach (var tempTile in thisCol)
        {
            saveRoot.TilesFromField.RemoveSpecificItem(tempTile.Deck);// if not there, ignore
        }
        if (_setType == EnumWhatSets.Kinds)
        {
            HandList.ReplaceRange(thisCol);
            return;
        }
        var wildCol = GetWildList(thisCol);
        int VeryFirst;
        VeryFirst = thisCol.First().Number;
        int veryLast;
        veryLast = thisCol.Last().Number;
        int firstNum;
        int lastNum;
        firstNum = VeryFirst;
        lastNum = veryLast;
        int x;
        int y;
        int WildNum = default;
        y = 1;
        var loopTo = thisCol.Count;
        for (x = 2; x <= loopTo; x++)
        {
            y += 1;
            firstNum += 1;
            var thisTile = thisCol[y - 1];
            if (thisTile.Number != firstNum)
            {
                WildNum += 1;
                thisTile = wildCol[WildNum - 1];
                thisTile.Number = firstNum;
                if (thisTile.Number == 14)
                {
                    thisTile.Number = VeryFirst - 1;
                }
                y -= 1;
            }
        }
        var Temps = (from items in thisCol
                     orderby items.Number
                     select items).ToList();
        HandList.ReplaceRange(Temps);
    }
    public void AddTile(TileInfo thisTile, int position)
    {
        TileRummySaveInfo saveRoot = aa1.Resolver!.Resolve<TileRummySaveInfo>();
        thisTile.Drew = true;
        thisTile.IsSelected = false;
        saveRoot.TilesFromField.RemoveSpecificItem(thisTile.Deck);
        if (_setType.Value == EnumWhatSets.Runs.Value && thisTile.IsJoker == true)
        {
            TileInfo newTile;
            if (position == 1)
            {
                newTile = HandList.First();
                thisTile.Number = newTile.Number - 1;
                HandList.InsertBeginning(thisTile);
            }
            else
            {
                newTile = HandList.Last();
                thisTile.Number = newTile.Number + 1;
                HandList.Add(thisTile);
            }
        }
        else
        {
            HandList.Add(thisTile);
        }
        if (_setType.Value == EnumWhatSets.Runs.Value)
        {
            var TempList = (from xx in HandList
                            orderby xx.Number
                            select xx).ToList();
            HandList.ReplaceRange(TempList);
        }
    }
    //if i can't ask for it to begin with, do here for rummys.
    public bool IsAcceptableSet()
    {
        if (HandList.Count < 3)
        {
            return false;
        }
        var thisList = HandList.ToRegularDeckDict();
        if (thisList.Count(items => items.IsJoker) > 1)
        {
            return false;
        }
        var wildList = GetWildList(thisList);
        if (wildList.Count == 1)
        {
            if (_isNew)
            {
                if (thisList.Count(items => items.IsJoker == false && items.WhatDraw == EnumDrawType.FromHand) < 2)
                {
                    return false; //because needs 2 from hand.
                }
            }
        }
        var newRummy = HandList.ToRegularDeckDict();
        if (_setType == EnumWhatSets.Runs)
        {
            return _rummys.IsNewRummy(newRummy, newRummy.Count, EnumRummyType.Runs);
        }
        return _rummys.IsNewRummy(newRummy, newRummy.Count, EnumRummyType.Sets);
    }
}