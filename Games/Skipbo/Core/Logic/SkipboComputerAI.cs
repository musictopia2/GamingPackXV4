﻿namespace Skipbo.Core.Logic;
[SingletonGame]
[AutoReset]
public class SkipboComputerAI
{
    private readonly SkipboGameContainer _gameContainer;
    private readonly SkipboVMData _model;
    public SkipboComputerAI(SkipboGameContainer gameContainer, SkipboVMData model)
    {
        _gameContainer = gameContainer;
        _model = model;
    }
    private struct DiscardPileInfo
    {
        public int Number1; // most recent
        public int Number2; // one before
        public int Pile;
    }
    private struct MoveInfo
    {
        public int StartNum;
        public BasicList<ComputerData> MoveList;
        public int Value;
        public int NumberOfWilds;
        public bool GetMore; // if true, then this means after the move, can get more cards
    }
    public struct ComputerDiscardInfo
    {
        public int Deck;
        public int Pile;
    }
    private DiscardPilesVM<SkipboCardInformation>? _tempDiscards;
    private BasicList<DiscardPileInfo>? _tempDiscardList;
    private DeckRegularDict<SkipboCardInformation>? _tempHand; // i think
    private BasicList<ComputerData>? _startList;
    public BasicList<ComputerData> ComputerMoves()
    {
        BasicList<ComputerData> output = new();
        _startList = ValidMoveList();
        if (_startList.Count == 0)
        {
            return output;
        }
        int x = default;
        BasicList<MoveInfo> possibleList = new();
        MoveInfo tempMove;
        foreach (var thisMove in _startList)
        {
            x += 1;
            tempMove = ResultsOfMove(_startList, x);
            if (tempMove.MoveList.Count > 0)
            {
                possibleList.Add(tempMove);
            }
        }
        if (possibleList.Count == 0)
        {
            return output;
        }
        BasicList<MoveInfo> thisCol;
        thisCol = StockList(possibleList);
        if (thisCol.Count > 0)
        {
            return thisCol.OrderByDescending(Items => Items.Value).First().MoveList;
        }
        thisCol = GetMoreList(possibleList);
        if (thisCol.Count > 0)
        {
            return thisCol.OrderByDescending(Items => Items.Value).First().MoveList;
        }
        return possibleList.OrderByDescending(Items => Items.Value).First().MoveList;
    }
    private BasicList<ComputerData> ValidMoveList()
    {
        BasicList<ComputerData> thisList = new();
        ComputerData thisComputer;
        int x;
        if (_gameContainer.IsValidMove == null)
        {
            throw new CustomBasicException("Nobody is handling the isvalidmove.  Rethink");
        }
        foreach (var tempCard in _gameContainer.SingleInfo!.MainHandList)
        {
            for (x = 1; x <= 4; x++)
            {
                if (_gameContainer.IsValidMove(x - 1, tempCard.Deck) == true)
                {
                    thisComputer = new ();
                    thisComputer.ThisCard = tempCard;
                    thisComputer.Pile = x - 1;
                    thisComputer.Discard = -1;
                    thisComputer.WhichType = EnumCardType.MyCards;
                    thisList.Add(thisComputer);
                }
            }
        }
        SkipboCardInformation thisCard;
        int y;
        var thisDiscard = _model.DiscardPiles;
        DeckRegularDict<SkipboCardInformation> cardList;
        for (x = 1; x <= 4; x++)
        {
            cardList = thisDiscard!.PileList![x - 1].ObjectList;
            if (cardList.Count > 0)
            {
                thisCard = thisDiscard.GetLastCard(x - 1);
                for (y = 1; y <= 4; y++)
                {
                    if (_gameContainer.IsValidMove(y - 1, thisCard.Deck) == true)
                    {
                        thisComputer = new ();
                        thisComputer.ThisCard = thisCard;
                        thisComputer.Pile = y - 1;
                        thisComputer.Discard = x - 1;
                        thisComputer.WhichType = EnumCardType.Discard;
                        thisList.Add(thisComputer);
                    }
                }
            }
        }
        thisCard = _model.StockPile!.GetCard();
        for (x = 1; x <= 4; x++)
        {
            if (_gameContainer.IsValidMove(x - 1, thisCard.Deck) == true)
            {
                thisComputer = new ();
                thisComputer.ThisCard = thisCard;
                thisComputer.Pile = x - 1;
                thisComputer.WhichType = EnumCardType.Stock;
                thisComputer.Discard = -1;
                thisList.Add(thisComputer);
            }
        }
        return thisList;
    }
    private void CopyDiscards(bool alsoHand)
    {
        var thisDiscard = _model.DiscardPiles;
        thisDiscard!.CopyDiscards(out _tempDiscards);
        if (alsoHand == false)
        {
            return;
        }
        _tempHand = new DeckRegularDict<SkipboCardInformation>();
        foreach (var thisCard in _gameContainer.SingleInfo!.MainHandList)
        {
            _tempHand.Add(thisCard);
        }
    }
    private static BasicList<MoveInfo> StockList(BasicList<MoveInfo> possibleMoves)
    {
        BasicList<MoveInfo> output = new();
        foreach (var tempItem in possibleMoves)
        {
            foreach (var tempMove in tempItem.MoveList)
            {
                if ((int)tempMove.WhichType == (int)EnumCardType.Stock)
                {
                    output.Add(tempItem);
                }
            }
        }
        return output;
    }
    private static BasicList<MoveInfo> GetMoreList(BasicList<MoveInfo> possibleMoves)
    {
        return possibleMoves.Where(items => items.GetMore == true).ToBasicList();
    }
    private bool AllDiscardEmpty()
    {
        var thisDiscard = _model.DiscardPiles;
        int x;
        for (x = 1; x <= HowManyDiscards; x++)
        {
            if (thisDiscard!.PileList![x - 1].ObjectList.Count > 0)
            {
                return false;
            }
        }
        return true;
    }
    private int EmptyDiscardPile()
    {
        int x;
        var thisDiscard = _model.DiscardPiles;
        for (x = 1; x <= HowManyDiscards; x++)
        {
            if (thisDiscard!.PileList![x - 1].ObjectList.Count == 0)
            {
                return x - 1;
            }
        }
        return -1;
    }
    private void FillDiscardList()
    {
        _tempDiscardList = new();
        DiscardPileInfo thisTemp;
        var thisDiscard = _model.DiscardPiles;
        SkipboCardInformation thisCard;
        for (var x = 1; x <= HowManyDiscards; x++)
        {
            thisTemp = new ();
            thisTemp.Pile = x - 1;
            if (thisDiscard!.PileList![x - 1].ObjectList.Count == 1)
            {
                thisCard = thisDiscard.PileList[x - 1].ObjectList.Single(); // i think this one.
                thisTemp.Number1 = thisCard.Number;
                thisTemp.Number2 = 0;
            }
            else
            {
                thisCard = thisDiscard.PileList[x - 1].ObjectList.GetSpecificItem(thisDiscard.GetLastCard(x - 1).Deck);
                thisTemp.Number1 = thisCard.Number;
                thisCard = thisDiscard.PileList[x - 1].ObjectList.GetSpecificItem(thisDiscard.NextToLastDeck(x - 1));
                thisTemp.Number2 = thisCard.Number;
            }
            _tempDiscardList.Add(thisTemp);
        }
    }
    private ComputerDiscardInfo FindBestWild()
    {
        ComputerDiscardInfo output = new();
        output.Deck = 0;
        output.Pile = 0;
        SkipboCardInformation thisCard;
        foreach (var thisTemp in _tempDiscardList!)
        {
            if (thisTemp.Number1 == 50)
            {
                var wildList = _gameContainer.SingleInfo!.MainHandList.Where(Items => Items.IsWild == true).ToRegularDeckDict();
                if (wildList.Count == _gameContainer.SingleInfo.MainHandList.Count)
                {
                    // this means there are only wilds
                    thisCard = _gameContainer.SingleInfo.MainHandList.First();
                    output.Deck = thisCard.Deck;
                    output.Pile = thisTemp.Pile;
                    return output;
                }
            }
        }
        return output;
    }
    private ComputerDiscardInfo FindBestStack()
    {
        ComputerDiscardInfo output = new();
        output.Deck = 0;
        output.Pile = 0;
        ComputerDiscardInfo thisDiscard;
        int newNums = default;
        BasicList<ComputerDiscardInfo> newList = new();
        SkipboCardInformation thisCard;
        foreach (var thisTemp in _tempDiscardList!)
        {
            if (thisTemp.Number1 < 13 && thisTemp.Number1 > 1)
            {
                newNums = thisTemp.Number1 - 1;
                var Filters = _gameContainer.SingleInfo!.MainHandList.Where(Items => Items.Number == newNums).ToRegularDeckDict();
                if (Filters.Count > 0)
                {
                    thisCard = Filters.First();
                    thisDiscard = new ();
                    thisDiscard.Deck = thisCard.Deck;
                    thisDiscard.Pile = thisTemp.Pile;
                    newList.Add(thisDiscard);
                }
            }
        }
        if (newList.Count == 0)
        {
            return output;
        }
        return newList.GetRandomItem();
    }
    private ComputerDiscardInfo FindBestStack(BasicList<DiscardPileInfo> stacks)
    {
        ComputerDiscardInfo output = new();
        output.Deck = 0;
        output.Pile = 0;
        if (stacks.Count == 0)
        {
            return output;
        }
        BasicList<ComputerDiscardInfo> newList = new(); // i think
        int newNums = default;
        ComputerDiscardInfo thisDiscard;
        SkipboCardInformation thisCard;
        foreach (var thisTemp in stacks)
        {
            if (thisTemp.Number2 < 13 && thisTemp.Number2 > 1)
            {
                newNums = thisTemp.Number1 - 1;
                var Filters = _gameContainer.SingleInfo!.MainHandList.Where(Items => Items.Number == newNums).ToRegularDeckDict();
                if (Filters.Count > 0)
                {
                    thisCard = Filters.First();
                    thisDiscard = new ();
                    thisDiscard.Deck = thisCard.Deck;
                    thisDiscard.Pile = thisTemp.Pile;
                    newList.Add(thisDiscard);
                }
            }
        }
        if (newList.Count == 0)
        {
            return output;
        }
        return newList.GetRandomItem();
    }
    private ComputerDiscardInfo FindBestSame()
    {
        return FindBestSame(_tempDiscardList!);
    }
    private ComputerDiscardInfo FindBestSame(IBasicList<DiscardPileInfo> sames)
    {
        ComputerDiscardInfo output = new();
        output.Deck = 0;
        output.Pile = 0;
        if (sames.Count == 0)
        {
            return output;
        }
        ComputerDiscardInfo thisDiscard;
        BasicList<ComputerDiscardInfo> newList = new();
        SkipboCardInformation thisCard;
        foreach (var thisTemp in sames)
        {
            if (thisTemp.Number1 < 13)
            {
                var Filters = _gameContainer.SingleInfo!.MainHandList.Where(Items => Items.Number == thisTemp.Number1).ToRegularDeckDict();
                if (Filters.Count > 0)
                {
                    thisCard = Filters.First();
                    thisDiscard = new ();
                    thisDiscard.Deck = thisCard.Deck;
                    thisDiscard.Pile = thisTemp.Pile;
                    newList.Add(thisDiscard);
                }
            }
        }
        if (newList.Count == 0)
        {
            return output;
        }
        return newList.GetRandomItem();
    }
    private BasicList<DiscardPileInfo> StackList()
    {
        BasicList<DiscardPileInfo> output = new();
        foreach (var thisTemp in _tempDiscardList!)
        {
            if (thisTemp.Number1 == (thisTemp.Number2 - 1) && thisTemp.Number1 > 0 && thisTemp.Number2 > 0)
            {
                output.Add(thisTemp);
            }
        }
        return output;
    }
    private BasicList<DiscardPileInfo> NoStackNoSame()
    {
        BasicList<DiscardPileInfo> output = new();
        foreach (var thisTemp in _tempDiscardList!)
        {
            if (thisTemp.Number1 > 0 && thisTemp.Number2 > 0 && thisTemp.Number1 < 13 && thisTemp.Number2 < 13)
            {
                if (thisTemp.Number1 != thisTemp.Number2 && (thisTemp.Number1 + 1) != thisTemp.Number2)
                {
                    output.Add(thisTemp);
                }
            }
        }
        return output;
    }
    private BasicList<DiscardPileInfo> SameList()
    {
        BasicList<DiscardPileInfo> output = new();
        foreach (var thisTemp in _tempDiscardList!)
        {
            if (thisTemp.Number1 == thisTemp.Number2 && thisTemp.Number1 > 0 && thisTemp.Number2 > 0)
            {
                output.Add(thisTemp);
            }
        }
        return output;
    }
    private ComputerDiscardInfo FindDiscardBasedOnOne()
    {
        ComputerDiscardInfo output = new();
        output.Deck = 0;
        output.Pile = 0;
        BasicList<DiscardPileInfo> ones;
        ones = ListOnes();
        if (ones.Count == 0)
        {
            return output;
        }
        ComputerDiscardInfo thisDiscard;
        BasicList<ComputerDiscardInfo> newList = new();
        SkipboCardInformation thisCard;
        foreach (var thisTemp in ones)
        {
            var filters = _gameContainer.SingleInfo!.MainHandList.Where(Items => Items.Number == thisTemp.Number1).ToRegularDeckDict();
            if (filters.Count > 0)
            {
                thisCard = filters.First();
                thisDiscard = new ();
                thisDiscard.Deck = thisCard.Deck;
                thisDiscard.Pile = thisTemp.Pile;
                newList.Add(thisDiscard);
            }
        }
        if (newList.Count > 0)
        {
            return newList.GetRandomItem();
        }
        int newNums;
        foreach (var thisTemp in ones)
        {
            newNums = thisTemp.Number1 - 1;
            if (newNums > 0 && newNums < 12)
            {
                var Filters = _gameContainer.SingleInfo!.MainHandList.Where(xx => xx.Number == newNums).ToRegularDeckDict();
                if (Filters.Count > 0)
                {
                    thisCard = Filters.First();
                    thisDiscard = new ();
                    thisDiscard.Deck = thisCard.Deck;
                    thisDiscard.Pile = thisTemp.Pile;
                    newList.Add(thisDiscard);
                }
            }
        }
        if (newList.Count > 0)
        {
            return newList.GetRandomItem();
        }
        return output;
    }
    private BasicList<DiscardPileInfo> ListOnes()
    {
        return _tempDiscardList!.Where(items => items.Number1 > 0 && items.Number2 == 0).ToBasicList();
    }
    private static int MostOf(out int manys, IDeckDict<SkipboCardInformation> whatCol)
    {
        var temps = whatCol.GroupOrderDescending(Items => Items.Number);
        manys = temps.First().Count();
        return temps.First().Key;
    }
    private int BestCardForEmpty()
    {
        if (_gameContainer.SingleInfo!.MainHandList.Count == 1)
        {
            return _gameContainer.SingleInfo.MainHandList.First().Deck;
        }
        int wilds = _gameContainer.SingleInfo.MainHandList.Count(x => x.IsWild == true);
        if (wilds == _gameContainer.SingleInfo.MainHandList.Count)
        {
            return _gameContainer.SingleInfo.MainHandList.First().Deck;
        }
        DeckRegularDict<SkipboCardInformation> tempCol =
            _gameContainer.SingleInfo.MainHandList.Where(x => x.IsWild == false).ToRegularDeckDict();
        int whatNum = MostOf(out int howMany, tempCol);
        int ask1;
        if (howMany == 1)
        {
            do
            {
                ask1 = _gameContainer.Random.GetRandomNumber(12);
                howMany = tempCol.Count(x => x.Number == ask1);
                if (howMany > 0)
                {
                    whatNum = ask1;
                    break;
                }
            } while (true);
        }
        tempCol.KeepConditionalItems(x => x.Number == whatNum);
        return tempCol.First().Deck;
    }

    public ComputerDiscardInfo ComputerDiscard()
    {
        ComputerDiscardInfo output = new();
        if (AllDiscardEmpty() == true)
        {
            output.Pile = 0; // because 0 based.
            output.Deck = BestCardForEmpty();
            return output;
        }
        int piles;
        int deck;
        piles = EmptyDiscardPile();
        if (piles > -1)
        {
            deck = BestCardForEmpty();
            output.Pile = piles;
            output.Deck = deck;
            return output;
        }
        FillDiscardList();
        if (_tempDiscardList!.Count == 0)
        {
            throw new CustomBasicException("There is nothing in the discard list.  Find out what happened");
        }
        ComputerDiscardInfo finds;
        finds = FindDiscardBasedOnOne();
        if (finds.Deck > 0)
        {
            output.Pile = finds.Pile;
            output.Deck = finds.Deck;
            return output;
        }
        BasicList<DiscardPileInfo> sames;
        sames = SameList();
        finds = FindBestSame(sames);
        if (finds.Deck > 0)
        {
            output.Pile = finds.Pile;
            output.Deck = finds.Deck;
            return output;
        }
        BasicList<DiscardPileInfo> stacks;
        stacks = StackList();
        finds = FindBestStack(stacks);
        if (finds.Deck > 0)
        {
            output.Pile = finds.Pile;
            output.Deck = finds.Deck;
            return output;
        }
        BasicList<DiscardPileInfo> nones;
        nones = NoStackNoSame();
        if (nones.Count > 0)
        {
            DiscardPileInfo temps;
            temps = nones.GetRandomItem();
            output.Pile = temps.Pile;
            deck = BestCardForEmpty();
            output.Deck = deck;
            return output;
        }
        int empties;
        empties = EmptyDiscardPile();
        if (empties > 0)
        {
            output.Pile = empties;
            deck = BestCardForEmpty();
            output.Deck = deck;
            return output;
        }
        finds = FindBestSame();
        if (finds.Deck > 0)
        {
            output.Pile = finds.Pile;
            output.Deck = finds.Deck;
            return output;
        }
        finds = FindBestStack();
        if (finds.Deck > 0)
        {
            output.Pile = finds.Pile;
            output.Deck = finds.Deck;
            return output;
        }
        finds = FindBestWild();
        if (finds.Deck > 0)
        {
            output.Pile = finds.Pile;
            output.Deck = finds.Deck;
            return output;
        }
        DiscardPileInfo newFinds;
        newFinds = _tempDiscardList.GetRandomItem();
        output.Pile = newFinds.Pile;
        output.Deck = BestCardForEmpty();
        return output;
    }
    private static SkipboCardInformation? FindDiscard(DiscardPilesVM<SkipboCardInformation> thisDiscard, int pile)
    {
        if (thisDiscard.PileList![pile].ObjectList.Count == 0)
        {
            return null;
        }
        return thisDiscard.GetLastCard(pile);
    }

    private int DiscardFromStack(int nextNumber, int stockNumber)
    {
        DiscardPilesVM<SkipboCardInformation> thisDiscard;
        thisDiscard = _tempDiscards!;
        SkipboCardInformation thisCard;
        int x;
        int y;
        int counts;
        for (x = 1; x <= HowManyDiscards; x++)
        {
            if (thisDiscard.PileList![x - 1].ObjectList.Count > 0)
            {
                thisCard = FindDiscard(thisDiscard, x - 1)!;
                if (thisCard.Number == nextNumber)
                {
                    if (thisCard.Number == nextNumber)
                    {
                        // now can consider
                        if (thisCard.Number == stockNumber)
                        {
                            return x - 1;
                        }
                        counts = thisDiscard.PileList[x - 1].ObjectList.Count - 1;
                        for (y = counts; y >= 1; y += -1)
                        {
                            thisCard = FindDiscard(_tempDiscards!, x - 1)!;
                            if (thisCard!.Number == stockNumber)
                            {
                                return x - 1;
                            }
                        }
                    }
                }
            }
        }
        return -1; // because 0 based
    }
    private ComputerData NextMove(int nextNumber, int pile)
    {
        ComputerData output = new();
        output.Pile = 0;
        output.WhichType = EnumCardType.IsNone;
        output.Discard = 0;
        string stockNumber;
        stockNumber = _gameContainer.SingleInfo!.InStock;
        if (stockNumber == "W" || int.Parse(stockNumber) == nextNumber)
        {
            output.Pile = pile;
            output.ThisCard = _model.StockPile!.GetCard();
            output.WhichType = EnumCardType.Stock;
            output.Discard = -1;
            return output;
        }
        int discards;
        discards = DiscardFromStack(nextNumber, int.Parse(stockNumber));
        if (discards > 0)
        {
            output.Pile = pile;
            output.WhichType = EnumCardType.Discard;
            output.Discard = discards;
            var ThisCard = FindDiscard(_tempDiscards!, discards);
            output.ThisCard = ThisCard;
            return output;
        }
        // now try from hand (non wilds)
        foreach (var thisCard in _tempHand!)
        {
            if (thisCard.Number == nextNumber && thisCard.IsWild == false)
            {
                output.Pile = pile;
                output.Discard = 0;
                output.ThisCard = thisCard;
                output.WhichType = EnumCardType.MyCards;
                return output;
            }
        }
        // now try including wilds
        foreach (var thisCard in _tempHand)
        {
            if (thisCard.IsWild == true)
            {
                output.Pile = pile;
                output.Discard = 0;
                output.ThisCard = thisCard;
                output.WhichType = EnumCardType.MyCards;
                return output;
            }
        }
        // now try the discard piles (not including wilds)
        int x;
        for (x = 1; x <= HowManyDiscards; x++)
        {
            var thisCard = FindDiscard(_tempDiscards!, x - 1);
            if (thisCard == null == false && thisCard!.IsWild == false && thisCard.Number == nextNumber)
            {
                output.Pile = pile;
                output.Discard = x - 1; // because 0 based
                output.ThisCard = thisCard;
                output.WhichType = EnumCardType.Discard;
                return output;
            }
        }
        for (x = 1; x <= HowManyDiscards; x++)
        {
            var thisCard = FindDiscard(_tempDiscards!, x - 1);
            if (thisCard != null && thisCard.IsWild == true)
            {
                output.Pile = pile;
                output.Discard = x - 1;
                output.ThisCard = thisCard;
                output.WhichType = EnumCardType.Discard;
                return output;
            }
        }
        return output;
    }
    private MoveInfo ResultsOfMove(BasicList<ComputerData> whatList, int whichOne)
    {
        MoveInfo output = new();
        output.MoveList = new();
        ComputerData thisMove;
        int numberNext;
        SkipboCardInformation thisCard;
        thisMove = whatList[whichOne - 1];
        MoveInfo tempMove;
        if (thisMove.WhichType == EnumCardType.Stock)
        {
            tempMove = new ();
            tempMove.MoveList = new();
            thisCard = thisMove.ThisCard!;
            if (thisCard.IsWild == true)
            {
                tempMove.NumberOfWilds = 1;
            }
            else
            {
                tempMove.NumberOfWilds = 0;
            }
            tempMove.Value = 500;
            tempMove.StartNum = whichOne;
            tempMove.MoveList = new()
            {
                thisMove
            };
            return tempMove;
        }
        numberNext = _model.PublicPiles!.NextNumberNeeded(thisMove.Pile);
        ComputerData newMove;
        output.MoveList.Add(thisMove);
        CopyDiscards(true);
        PopulateMove(ref output, thisMove);
        do
        {
            numberNext += 1;
            if (numberNext == 13)
            {
                numberNext = 1;
            }
            newMove = NextMove(numberNext, thisMove.Pile);
            if (newMove.WhichType == EnumCardType.IsNone)
            {
                if (output.NumberOfWilds > 0)
                {
                    BasicList<ComputerData> tempCol = new();
                    tempCol.AddRange(output.MoveList);
                    output.MoveList = new();
                    foreach (var newTempMove in tempCol)
                    {
                        thisCard = newTempMove.ThisCard!;
                        if (thisCard.IsWild == false)
                        {
                            output.MoveList.Add(newTempMove);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return output;
            }
            output.MoveList.Add(newMove);
            PopulateMove(ref output, newMove);
            if ((int)newMove.WhichType == (int)EnumCardType.Stock || output.GetMore == true)
            {
                return output;
            }
        }
        while (true);
    }
    private void PopulateMove(ref MoveInfo thisMove, ComputerData newMove)
    {
        int NewValue;
        NewValue = 0;
        int x;
        if (newMove.WhichType == EnumCardType.Stock)
        {
            NewValue += 400;
            if (thisMove.MoveList.Count > 1)
            {
                var loopTo = thisMove.MoveList.Count;
                for (x = 2; x <= loopTo; x++)
                {
                    NewValue -= 20;
                }
            }
        }
        SkipboCardInformation thisCard;
        thisCard = newMove.ThisCard!;
        if (thisCard.IsWild == true)
        {
            thisMove.NumberOfWilds += 1;
            NewValue -= 100;
        }
        if (newMove.WhichType == EnumCardType.MyCards)
        {
            _tempHand!.RemoveSpecificItem(thisCard);
            if (_tempHand.Count == 0)
            {
                thisMove.GetMore = true;
                NewValue += 60;
            }
        }
        if (newMove.WhichType == EnumCardType.Discard)
        {
            NewValue += 20;
            _tempDiscards!.RemoveCard(newMove.Discard, newMove.ThisCard!.Deck);
            if (_model.DiscardPiles!.PileList![newMove.Discard].ObjectList.Count == 0)
            {
                throw new CustomBasicException("Cannot be 0 for the discards");
            }
        }
        thisMove.Value += NewValue;
    }
}
