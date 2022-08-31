namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.RummyClasses;
public class RummyProcesses<S, C, R>
    where S : IFastEnumSimple
    where C : IFastEnumColorSimple
    where R : IRummmyObject<S, C>, new()
{
    public bool HasSecond { get; set; }
    struct TempObject
    {
        public int ObjectNumber;
        public S Suit;
        public int IndexinCollection;
    }
    public bool NeedMatch { get; set; } = true;
    public bool HasWild { get; set; }
    private int _maxStraight;
    public bool UseAll { get; set; }
    public int LowNumber { get; set; } = 1;
    public int HighNumber { get; set; } = 13; // from 1 to 13 because the regular deck of cards
    private BasicList<R>? _tempList;
    public bool UseSecond { get; private set; }
    public int FirstUsed { get; private set; }
    private void CheckErrors()
    {
        if (HighNumber == 0 | LowNumber == 0)
        {
            throw new Exception("Need to know the low and high numbers");
        }
        if (HighNumber < LowNumber)
        {
            throw new Exception("The highest number must be higher than the low number");
        }
        _maxStraight = HighNumber - LowNumber + 2;
    }
    private void GetTempList(IBasicList<R> objectList)
    {
        _tempList = objectList.ToBasicList();
    }
    public BasicList<S> ListSuits(IBasicList<R> objectList)
    {
        GetTempList(objectList);
        var thisTemp = _tempList!.Where(Items => Items.IsObjectIgnored == false && Items.IsObjectWild == false).GroupBy(Items => Items.GetSuit);
        BasicList<S> output = new();
        int MaxNum = 0;
        int Count;
        foreach (var newTemp in thisTemp)
        {
            Count = newTemp.Count();
            if (Count > MaxNum)
            {
                output = new();
                MaxNum = Count;
            }
            output.Add(newTemp.Key);
        }
        return output;
    }
    public int StraightDistance(ref IBasicList<R> objectList, int whatNum)
    {
        int output = default;
        GetTempList(objectList);
        int highAmount = default;
        int lowAmount = default;
        int firstInfo = default;
        int secondInfo = default;
        if (NeedMatch == true)
        {
            _tempList = _tempList!.OrderBy(xx => xx.GetSuit).ThenBy(xx => xx.ReadMainValue).ToBasicList();
        }
        else
        {
            _tempList = _tempList!.OrderBy(xx => xx.ReadMainValue).ToBasicList();
        }
        int x = 0;
        int newNum = 0;
        do
        {
            if (x > _tempList.Count)
            {
                break;
            }
            newNum = _tempList[x].ReadMainValue;
            if (newNum == whatNum)
            {
                if (x > 0)
                {
                    lowAmount = _tempList[x].ReadMainValue;
                    firstInfo = lowAmount;
                    lowAmount = whatNum - lowAmount;
                }
                else
                {
                    lowAmount = 0;
                    firstInfo = 0;
                }
                if (x + 1 < objectList.Count)
                {
                    highAmount = _tempList[x + 1].ReadMainValue;
                    secondInfo = highAmount;
                    if (highAmount > 15)
                    {
                        highAmount = 0;
                        secondInfo = 0;
                    }
                    else
                    {
                        highAmount -= whatNum;
                    }
                }
                else
                {
                    highAmount = 0;
                }
                break;
            }
            x += 1;
        }
        while (true);
        if (highAmount == 0)
        {
            output = lowAmount;
            FirstUsed = firstInfo;
        }
        else if (lowAmount == 0)
        {
            output = highAmount;
            FirstUsed = secondInfo;
        }
        else if (highAmount < lowAmount)
        {
            output = highAmount;
            FirstUsed = secondInfo;
        }
        else
        {
            output = lowAmount;
            FirstUsed = secondInfo;
        }
        if (HasSecond == false)
        {
            return output;
        }
        GetTempList(objectList);
        if (NeedMatch == true)
        {
            _tempList = _tempList.OrderBy(xx => xx.GetSuit).ThenBy(xx => xx.GetSecondNumber).ToBasicList();
        }
        else
        {
            _tempList = _tempList.OrderBy(xx => xx.GetSecondNumber).ToBasicList(); //this was the best way to handle sorting.
        }
        x = 0;
        do
        {
            if (x > _tempList.Count)
            {
                break;
            }
            newNum = _tempList[x].GetSecondNumber;
            if (newNum == whatNum)
            {
                if (x > 0)
                {
                    lowAmount = _tempList[x].GetSecondNumber;
                    firstInfo = lowAmount;
                    lowAmount = whatNum - lowAmount;
                }
                else
                {
                    lowAmount = 0;
                    firstInfo = 0;
                }

                if (x + 1 < objectList.Count)
                {
                    highAmount = _tempList[x + 1].GetSecondNumber;
                    secondInfo = highAmount;
                    if (highAmount > 15)
                    {
                        highAmount = 0;
                        secondInfo = 0;
                    }
                    else
                    {
                        highAmount -= whatNum;
                    }
                }
                else
                {
                    highAmount = 0;
                }
                break;
            }
            x += 1;
        }
        while (true);
        if (output == 0 & highAmount == 0 & lowAmount > 0)
        {
            output = lowAmount;
            FirstUsed = firstInfo;
        }
        else if (output == 0 & lowAmount == 0 & highAmount > 0)
        {
            output = highAmount;
            FirstUsed = secondInfo;
        }
        else if (highAmount == 0 & lowAmount < output & lowAmount > 0)
        {
            output = lowAmount;
            FirstUsed = firstInfo;
        }
        else if (lowAmount == 0 & highAmount < output & highAmount > 0)
        {
            output = highAmount;
            FirstUsed = secondInfo;
        }
        else if (highAmount < lowAmount & highAmount < output & highAmount > 0)
        {
            output = highAmount;
            FirstUsed = secondInfo;
        }
        else if (lowAmount < highAmount & lowAmount < output & highAmount > 0)
        {
            output = lowAmount;
            FirstUsed = firstInfo;
        }
        return output;
    }
    private void HighLow(ref BasicList<R> firstList, IBasicList<R> colObj, int intHowMany, bool minonly, int lngUnUsedWild, ref int intHigh, ref int intLow)
    {
        int intObjectNumber;
        int intAvailableObject;
        int intAvailableWild;
        intHigh = 0;
        intLow = 0;
        if (colObj.Count <= 0)
        {
            return;
        }
        if (HasWild)
        {
            intAvailableWild = lngUnUsedWild;
        }
        else
        {
            intAvailableWild = 0;
        }
        intAvailableObject = colObj.Count;
        if (UseSecond == false)
            firstList = (from xx in colObj
                         orderby xx.ReadMainValue
                         select xx).ToBasicList();
        else
            firstList = (from xx in colObj
                         orderby xx.GetSecondNumber
                         select xx).ToBasicList();
        if (UseSecond == false)
        {
            intObjectNumber = firstList.Last().ReadMainValue;
        }
        else
        {
            intObjectNumber = firstList.Last().GetSecondNumber;
        }
        if (intObjectNumber < HighNumber)
        {
            if (intAvailableWild > 0)
            {
                if (minonly)
                {
                    if (intHowMany > intAvailableObject)
                        if (intHowMany - intAvailableObject > HighNumber - intObjectNumber)
                        {
                            intHigh = HighNumber;
                            intAvailableWild -= HighNumber - intObjectNumber;
                            intAvailableObject += HighNumber - intObjectNumber;
                        }
                        else
                        {
                            intHigh = intObjectNumber + intHowMany - intAvailableObject;
                            intAvailableWild -= intHowMany - intAvailableObject;
                            intAvailableObject = intHowMany;
                        }
                    else if (HighNumber - intObjectNumber > intAvailableWild)
                    {
                        intHigh = intObjectNumber + intAvailableWild;
                        intAvailableWild = 0;
                    }
                    else
                    {
                        intHigh = HighNumber;
                        intAvailableWild -= HighNumber - intObjectNumber;
                    }
                }
                else
                {
                    intHigh = intObjectNumber;
                }
            }
        }
        if (UseSecond == false)
        {
            intObjectNumber = colObj.First().ReadMainValue;
        }
        else
        {
            intObjectNumber = colObj.First().GetSecondNumber;
        }
        if (intObjectNumber >= LowNumber)
        {
            if (intAvailableWild > 0)
            {
                if (minonly)
                {
                    if (intHowMany > intAvailableObject)
                    {
                        if (intHowMany - intAvailableObject > intObjectNumber - LowNumber)
                        {
                            intLow = LowNumber;
                            intAvailableWild -= intObjectNumber - LowNumber;
                            intAvailableObject += intObjectNumber - LowNumber;
                        }
                        else
                        {
                            intLow = intObjectNumber - (intHowMany - intAvailableObject);
                            intAvailableWild -= intHowMany - intAvailableObject;
                            intAvailableObject = intHowMany;
                        }
                    }
                    else if (intObjectNumber - LowNumber > intAvailableWild)
                    {
                        intLow = intObjectNumber - intAvailableWild;
                        intAvailableWild = 0;
                    }
                    else
                    {
                        intLow = LowNumber;
                        intAvailableWild -= intObjectNumber - LowNumber;
                    }
                }
                else
                {
                    intLow = intObjectNumber;
                }
            }
        }
    }
    private bool HasValidStraight(IBasicList<R> firstList, IBasicList<R> wildList, bool bUseSecond, int lngHowMany, bool minonly, ref int[]? aStraightObject, ref int lngUnUsedWild, ref int startAt)
    {
        BasicList<TempObject> aObject = new();
        TempObject tempObject;
        int lngObjectIndex = default;
        int lngIndex;
        int lngAvailableWild;
        int lngTotalWild;
        int lngObjectInStraight;
        int lngEnd;
        bool output = true; //this time it was set to true.
        int intObjectNumber;
        if (firstList.Count + wildList.Count < lngHowMany)
        {
            return false;
        }
        if (lngHowMany == 1)
        {
            return true;
        }
        if (HasWild)
        {
            lngTotalWild = wildList.Count;
            lngAvailableWild = lngTotalWild; // 'initally available wild will be same as total wild
            lngUnUsedWild = lngTotalWild;
        }
        else
        {
            lngTotalWild = 0;
            lngAvailableWild = 0;
            lngUnUsedWild = 0;
        }
        S CurrentSuit;
        var loopTo = firstList.Count - 1;
        for (lngIndex = 0; lngIndex <= loopTo; lngIndex++)
        {
            tempObject = new();
            if (NeedMatch == true)
            {
                CurrentSuit = firstList[lngIndex].GetSuit;
                tempObject.Suit = CurrentSuit;
            }
            intObjectNumber = firstList[lngIndex].ReadMainValue;
            if (bUseSecond)
            {
                intObjectNumber = firstList[lngIndex].GetSecondNumber;
            }
            tempObject.ObjectNumber = intObjectNumber;
            tempObject.IndexinCollection = lngIndex;
            aObject.Add(tempObject);
            lngObjectIndex++;
        }

        if (output == false)
        {
            return false;
        }
        if (lngObjectIndex + lngTotalWild < lngHowMany)
        {
            return false;
        }
        lngObjectInStraight = 1;
        startAt = 0;
        lngEnd = 0;
        var loopTo1 = lngObjectIndex - (long)2;
        for (lngIndex = 0; lngIndex <= loopTo1; lngIndex++)
        {
            if (Math.Abs(aObject[lngIndex].ObjectNumber - aObject[lngIndex + 1].ObjectNumber) == 1 & (NeedMatch == false | NeedMatch == true & aObject[lngIndex].Suit.Equals(aObject[lngIndex + 1].Suit)))
            {
                lngObjectInStraight += 1;
                lngEnd = lngIndex + 1;
            }
            else if (HasWild == true)
            {
                if ((long)(Math.Abs(aObject[lngIndex].ObjectNumber - aObject[lngIndex + 1].ObjectNumber) - 1) <= lngAvailableWild & lngAvailableWild > 0)
                    if (minonly == true)
                    {
                        if (lngObjectInStraight < lngHowMany)
                        {
                            lngAvailableWild -= lngHowMany - lngObjectInStraight;
                            lngObjectInStraight = lngObjectInStraight + lngHowMany - lngObjectInStraight;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        lngObjectInStraight++;
                        lngObjectInStraight = lngObjectInStraight + Math.Abs(aObject[lngIndex].ObjectNumber - aObject[lngIndex + 1].ObjectNumber) - 1;
                        lngAvailableWild -= Math.Abs(aObject[lngIndex].ObjectNumber - aObject[lngIndex + 1].ObjectNumber) - 1;
                        lngEnd = lngIndex + 1;
                    }
                else if (lngObjectInStraight >= lngHowMany)
                {
                    lngEnd = lngIndex;
                    break;
                }
                else
                {
                    if (lngObjectIndex - lngIndex - (long)1 + lngTotalWild >= lngHowMany)
                    {
                        startAt = lngIndex + 1;
                        lngAvailableWild = lngTotalWild;
                        lngObjectInStraight = 1;
                        lngEnd = startAt;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (lngObjectInStraight >= lngHowMany)
            {
                lngEnd = lngIndex;
                break;
            }
            else
            {
                startAt = lngIndex + 1;
                lngAvailableWild = lngTotalWild; // 'Make all wild available
                lngObjectInStraight = 1;
                lngEnd = startAt;
            }
            if (lngObjectInStraight >= lngHowMany & minonly == true)
            {
                break;
            }
        }
        if (lngObjectInStraight + lngAvailableWild >= lngHowMany)
        {
            aStraightObject = new int[lngObjectInStraight - (lngTotalWild - lngAvailableWild) + 1];
            long lngPos;
            lngPos = 0;
            var loopTo2 = lngEnd;
            for (lngIndex = startAt; lngIndex <= loopTo2; lngIndex++)
            {
                aStraightObject[lngPos] = aObject[lngIndex].IndexinCollection;
                lngPos++;
            }
            output = true;
            lngUnUsedWild = lngAvailableWild;
        }
        else
        {
            output = false;
        }
        return output;
    }
    private IBasicList<R> StraightSet(IBasicList<R> objectList, int howMany, bool minOnly, IBasicList<R> wildList, bool noWilds = false)
    {
        IBasicList<R> output = new BasicList<R>();
        bool tempwilds = HasWild;
        if (noWilds == true)
        {
            HasWild = false;
        }
        else if (HasWild == false)
        {
            HasWild = false;
        }
        else
        {
            HasWild = true;
        }
        IEnumerable<R> firstLinq;
        if (UseSecond == false)
            firstLinq = from xx in _tempList
                        where xx.IsObjectIgnored == false && xx.IsObjectWild == false
                        orderby xx.ReadMainValue
                        select xx;
        else
            firstLinq = from xx in _tempList
                        where xx.IsObjectIgnored == false && xx.IsObjectWild == false
                        select xx;
        BasicList<R> firstList = new();
        firstList.AddRange(firstLinq);
        var exps = firstLinq.GroupBy(xx => xx.ReadMainValue).ToBasicList();
        BasicList<R> temps;
        if (NeedMatch == false)
        {
            temps = firstList.GroupBy(xx => xx.ReadMainValue).Select(xx => xx.First()).ToBasicList();
        }
        else
        {
            temps = firstList.GroupBy(xx => new { xx.ReadMainValue, xx.GetSuit }).Select(xx => xx.First()).ToBasicList();
        }
        firstList.ReplaceRange(temps);
        if (NeedMatch == true)
        {
            firstList = firstList.OrderBy(xx => xx.GetSuit).ThenBy(xx => xx.ReadMainValue).ToBasicList();
        }
        else
        {
            firstList = firstList.OrderBy(xx => xx.ReadMainValue).ToBasicList();
        }
        int[]? aObjectIndex;
        aObjectIndex = new int[1];
        bool bStraightFound = default;
        int lngUnUsedWild = default;
        int Start = default;
        if (HasValidStraight(firstList, wildList, false, howMany, minOnly, ref aObjectIndex, ref lngUnUsedWild, ref Start))
        {
            bStraightFound = true;
        }
        else
        {
            aObjectIndex = null;
            if (HasSecond == true)
            {
                firstList = new();
                firstList.AddRange(firstLinq);
                if (NeedMatch == true)
                {
                    firstList = (from xx in firstList
                                 orderby xx.GetSuit ascending, xx.GetSecondNumber ascending
                                 select xx).ToBasicList();
                }
                else
                {
                    firstList = (from xx in firstList
                                 orderby xx.GetSecondNumber
                                 select xx).ToBasicList();
                }
            }
            if (HasValidStraight(firstList, wildList, true, howMany, minOnly, ref aObjectIndex, ref lngUnUsedWild, ref Start))
            {
                UseSecond = true;
                bStraightFound = true;
            }
        }
        int lngObjectInStraight;
        int lngIndex;
        if (bStraightFound == true)
        {
            lngObjectInStraight = aObjectIndex!.GetUpperBound(0) - 1; // i think
            var loopTo = (long)Start + lngObjectInStraight;
            for (lngIndex = Start; lngIndex <= loopTo; lngIndex++)
            {
                output.Add(firstList[lngIndex]);
                if (output.Count == _maxStraight)
                {
                    break;
                }
            }
            int intHigh = default;
            int intLow = default;
            HighLow(ref firstList, output, howMany, minOnly, lngUnUsedWild, ref intHigh, ref intLow);
            FirstUsed = intLow;
            if (HasWild == true)
            {
                var loopTo1 = wildList.Count - 1;
                for (lngIndex = 0; lngIndex <= loopTo1; lngIndex++)
                {
                    if (lngIndex + 1 <= wildList.Count)
                    {
                        if (output.Count < howMany | minOnly == false)
                        {
                            output.Add(wildList[lngIndex]);
                        }
                    }
                    if (output.Count == _maxStraight)
                    {
                        return output;
                    }
                }
            }
        }
        if (output.Count == objectList.Count & UseAll == false)
        {
            firstLinq = from xx in output
                        where xx.ReadMainValue == FirstUsed
                        select xx;
            if (firstLinq.Any())
            {
                output.RemoveSpecificItem(firstLinq.First());
            }
            else
            {
                output.RemoveLastItem();
            }
        }
        aObjectIndex = null;
        int newnum;
        newnum = FirstUsed + output.Count - 1;
        newnum = HighNumber - newnum;
        if (FirstUsed > 0 & newnum < 0)
        {
            FirstUsed += newnum;
        }
        HasWild = tempwilds;
        return output;
    }
    private bool IsStraight()
    {
        bool output = default;
        bool bStraightFound;
        int[]? aObjectIndex;
        int lngUnUsedWild = default;
        UseSecond = false;
        int currentnum = default;
        int thismany = default;
        int x = default;
        BasicList<R> filteredList;
        filteredList = (from xx in _tempList
                        where xx.IsObjectIgnored == false && xx.IsObjectWild == false
                        select xx).ToBasicList();
        var loopTo = filteredList.Count - 1;
        for (x = 0; x <= loopTo; x++)
        {
            currentnum = filteredList[x].ReadMainValue;
            thismany = filteredList.Count(Items => Items.ReadMainValue == currentnum);
            if (thismany > 1)
            {
                return false;
            }
        }
        BasicList<R> newList = new();
        newList.AddRange(_tempList!);
        var wildList = newList.Where(Items => Items.IsObjectWild == true).ToBasicList();
        _tempList = filteredList.ToBasicList();
        if (NeedMatch == true)
        {
            _tempList = (from xx in _tempList
                         orderby xx.GetSuit ascending, xx.ReadMainValue ascending
                         select xx).ToBasicList();
        }
        else
        {
            _tempList = (from xx in _tempList
                         orderby xx.ReadMainValue
                         select xx).ToBasicList();
        }
        if (NeedMatch == true)
        {
            bool rets;
            rets = _tempList.HasOnlyOne(items => items.GetSuit);
            if (rets == false)
            {
                return false;
            }
        }
        bStraightFound = false;
        FirstUsed = 0;
        aObjectIndex = new int[1];
        var argStartAt1 = 0;
        if (HasValidStraight(_tempList, wildList, false, _tempList.Count + wildList.Count, false, ref aObjectIndex, ref lngUnUsedWild, ref argStartAt1))
        {
            bStraightFound = true;
        }
        else
        {
            aObjectIndex = null;
            if (HasSecond == true)
            {
                _tempList = _tempList.OrderBy(items => items.GetSecondNumber).ToBasicList();
                var argStartAt = 0;
                if (HasValidStraight(_tempList, wildList, true, _tempList.Count + wildList.Count, false, ref aObjectIndex, ref lngUnUsedWild, ref argStartAt))
                {
                    bStraightFound = true;
                    UseSecond = true;
                }
            }
        }
        if (bStraightFound == true)
        {
            int lngCardInStraight;
            int lngIndex;
            BasicList<R> straightset = new();
            lngCardInStraight = aObjectIndex!.GetUpperBound(0) - 1; // because its 0 based.
            var loopTo1 = lngCardInStraight;
            for (lngIndex = 0; lngIndex <= loopTo1; lngIndex++)
            {
                straightset.Add(_tempList[lngIndex]);
                if (straightset.Count == _maxStraight)
                {
                    break;
                }
            }
            int intHigh = default;
            int intLow = default;
            HighLow(ref _tempList, straightset, _tempList.Count, false, lngUnUsedWild, ref intHigh, ref intLow);
            FirstUsed = intLow;
        }
        aObjectIndex = null;
        output = bStraightFound;
        return output;
    }
    public BasicList<R> WhatNewRummy(IBasicList<R> objectList, int howMany, EnumRummyType whatRummy, bool minOnly, bool noWilds = false, bool minWilds = false)
    {
        BasicList<R> output = new();
        PrivateNewRummy(output, objectList, howMany, whatRummy, minOnly, noWilds, minWilds);
        return output;
    }
    public DeckRegularDict<D> WhatNewRummy<D>(DeckRegularDict<D> cardList, int howMany, EnumRummyType whatRummy, bool minOnly, bool noWilds = false, bool minWilds = false)
        where D : IRummmyObject<S, C>, IDeckObject, new()
    {
        DeckRegularDict<D> output = new();
        PrivateNewRummy(output, cardList, howMany, whatRummy, minOnly, noWilds, minWilds);
        return output;
    }
    private void PrivateNewRummy<RR>(IBasicList<RR> output, IBasicList<RR> objectList, int howMany, EnumRummyType whatRummy, bool minOnly, bool noWilds = false, bool minWilds = false)
        where RR : IRummmyObject<S, C>, new()
    {
        BasicList<R> firstTemp = objectList.Cast<R>().ToBasicList();
        GetTempList(firstTemp);
        CheckErrors();
        UseSecond = false;
        int wildsUsed = default;
        int maxWildsUsed;
        maxWildsUsed = 100;
        BasicList<R> wildList;
        BasicList<R> mainList = new();
        BasicList<R> thisList;
        wildList = _tempList!.Where(xx => xx.IsObjectWild == true).ToBasicList();
        int WildsNeeded;
        int x;
        switch (whatRummy)
        {
            case EnumRummyType.Colors:
                var colorList = _tempList!.Where(xx => xx.IsObjectIgnored == false && xx.IsObjectWild == false).GroupBy(xx => xx.GetColor).ToBasicList();
                colorList.ForEach(thisColor =>
                {
                    thisList = new();
                    int count = thisColor.Count();
                    if (count >= howMany)
                    {
                        thisList.AddRange(thisColor);
                        if (minOnly == false)
                        {
                            thisList.AddRange(wildList);
                        }
                    }
                    else if (minOnly == false & count + wildList.Count >= howMany & HasWild == true)
                    {
                        thisList.AddRange(thisColor);
                        thisList.AddRange(wildList);
                    }
                    else if (minOnly == true & HasWild == true & count + wildList.Count > howMany)
                    {
                        thisList.AddRange(thisColor);
                        WildsNeeded = howMany - count;
                        var loopTo = WildsNeeded - 1;
                        for (x = 0; x <= loopTo; x++)
                        {
                            thisList.Add(wildList[x]);
                        }
                    }
                    if (thisList.Count > mainList.Count)
                    {
                        mainList = thisList;
                    }
                });
                output.ReplaceRange(mainList.Cast<RR>());
                if (output.Count == objectList.Count & UseAll == false)
                {
                    output.RemoveAt(0);
                }
                return;
            case EnumRummyType.Sets:
                int y;
                var setList = _tempList!.Where(xx => xx.IsObjectIgnored == false && xx.IsObjectWild == false).GroupBy(xx => xx.ReadMainValue).ToBasicList();
                setList.ForEach(thisSet =>
                {
                    thisList = new();
                    int Count = thisSet.Count();
                    if (Count >= howMany)
                    {
                        thisList.AddRange(thisSet);
                        if (minOnly == true & thisList.Count > howMany)
                        {
                            y = thisList.Count - 1;
                            var loopTo1 = y;
                            for (x = howMany; x <= loopTo1; x++)
                            {
                                thisList.RemoveAt(0);
                            }
                        }
                        if (minOnly == false & wildList.Count > 0 & minWilds == false) //they did not do else if here.  hopefully still okay
                        {
                            thisList.AddRange(wildList);
                            wildsUsed = 0;
                        }
                    }
                    else if ((minOnly == false | minWilds == true) & Count + wildList.Count >= howMany & HasWild == true)
                    {
                        thisList.AddRange(thisSet);
                        thisList.AddRange(wildList);
                        wildsUsed = wildList.Count;
                    }
                    else if (minOnly == true & HasWild == true & Count + wildList.Count > howMany)
                    {
                        thisList.AddRange(thisSet);
                        WildsNeeded = howMany - Count;
                        var loopTo2 = WildsNeeded;
                        for (x = 1; x <= loopTo2; x++)
                        {
                            thisList.Add(wildList[x - 1]);
                        }
                    }
                    if (wildsUsed < maxWildsUsed & HasWild == true & setList.Count == mainList.Count & (minOnly == true | minWilds == true))
                    {
                        mainList = thisList;
                    }
                    else if (thisList.Count > mainList.Count & (HasWild == false |
                    wildsUsed < maxWildsUsed & HasWild == true & (minOnly == true |
                    minWilds == true) |
                    HasWild == true & minOnly == false))
                    {
                        mainList = thisList;
                    }
                });
                output.ReplaceRange(mainList.Cast<RR>());
                if (output.Count == objectList.Count & UseAll == false)
                {
                    output.RemoveAt(0);
                }
                return;
            case EnumRummyType.Runs:
                var ourObjects = objectList.Cast<R>().ToBasicList();
                var finTemp = StraightSet(ourObjects, howMany, minOnly, wildList, noWilds);
                output.ReplaceRange(finTemp.Cast<RR>());
                if (FirstUsed == 2 & HasSecond == true)
                {
                    S suit;
                    var suitList = _tempList!.Where(xx => xx.IsObjectIgnored == false
                    && xx.IsObjectWild == false)
                        .GroupBy(Items => Items.GetSuit).ToBasicList();
                    if (suitList.Count == 0)
                    {
                        throw new CustomBasicException("There are no suits available.  If there are really no suits; then fix this");
                    }
                    suit = suitList.First().Key;
                    var runTemp = objectList.Where(xx => xx.GetSuit.Equals(suit) && xx.GetSecondNumber == 1).FirstOrDefault();
                    if (runTemp != null)
                    {
                        output.InsertBeginning(runTemp);
                    }
                }
                return;
            default:
                {
                    throw new CustomBasicException("Cannot figure out what rummy type to do");
                }
        }
    }
    public bool IsNewRummy(IBasicList<R> objectList, int howMany, EnumRummyType whatRummy)
    {
        UseSecond = false;
        CheckErrors();
        if (objectList.Count < howMany)
        {
            return false;
        }
        GetTempList(objectList);
        BasicList<R> ignoreLinq = objectList.Where(Items => Items.IsObjectIgnored == true).ToBasicList();
        if (ignoreLinq.Count > 0)
        {
            return false;
        }
        switch (whatRummy)
        {
            case EnumRummyType.Sets:
                _tempList!.RemoveAllOnly(xx => xx.IsObjectWild == true);
                return _tempList.HasOnlyOne(xx => xx.ReadMainValue);
            case EnumRummyType.Colors:
                _tempList!.RemoveAllOnly(xx => xx.IsObjectWild == true);
                return _tempList.HasOnlyOne(xx => xx.GetColor);
            case EnumRummyType.Runs:
                return IsStraight();
            default:
                throw new CustomBasicException("Not Supported");
        }
    }
    public bool CanCardUsed(IBasicList<R> objectList, int whatElement, EnumRummyType whatRummy, int howMany = 3)
    {
        bool output = default;
        GetTempList(objectList);
        CheckErrors();
        if (objectList.Count < howMany)
        {
            output = false;
            return output;
        }
        int thisnum;
        S thissuit;
        thisnum = _tempList![whatElement].ReadMainValue;
        thissuit = _tempList[whatElement].GetSuit;
        int num1;
        int num2;
        S suit1;
        S suit2;
        BasicList<R> newCols = new();
        _tempList = newCols.ToBasicList();
        BasicList<R> tempRummy;
        IEnumerable<R> firstLinq;
        IEnumerable<R> secondLinq;
        int x = 0;
        do
        {
            tempRummy = WhatNewRummy(newCols, howMany, whatRummy, false);
            if (tempRummy.Count == 0)
            {
                return false;
            }
            firstLinq = from xx in tempRummy
                        where xx.ReadMainValue == thisnum & xx.GetSuit.Equals(thissuit)
                        select xx;
            if (firstLinq.Any())
            {
                return true;
            }
            secondLinq = from xx in tempRummy
                         where xx.IsObjectIgnored == true && xx.IsObjectWild == false
                         select xx;
            foreach (R obj in secondLinq)
            {
                suit1 = obj.GetSuit;
                num1 = obj.ReadMainValue;
                var loopTo = _tempList.Count - 1;
                for (x = 0; x <= loopTo; x++)
                {
                    num2 = _tempList[x].ReadMainValue;
                    suit2 = _tempList[x].GetSuit;
                    if (num1 == num2 & suit1.Equals(suit2))
                    {
                        _tempList.RemoveAt(x);
                    }
                }
            }
            newCols = new();
            _tempList = newCols.ToBasicList();
        }
        while (true);
    }
}