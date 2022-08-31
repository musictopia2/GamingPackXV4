﻿namespace Flinch.Core.Logic;
[SingletonGame]
[AutoReset]
public class FlinchComputerAI
{
    private readonly FlinchGameContainer _gameContainer;
    private readonly FlinchVMData _model;
    public FlinchComputerAI(FlinchGameContainer gameContainer, FlinchVMData model)
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
        public bool GetMore; // if true, then this means after the move, can get more cards
    }
    public struct ComputerDiscardInfo
    {
        public int Deck;
        public int Pile;
    }
    private DiscardPilesVM<FlinchCardInformation>? _tempDiscards;
    private BasicList<DiscardPileInfo>? _tempDiscardList;
    private DeckRegularDict<FlinchCardInformation>? _tempHand;
    private BasicList<ComputerData>? _startList;
    public int MaxPiles { get; set; }
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
            for (x = 1; x <= MaxPiles; x++)
            {
                if (_gameContainer.IsValidMove(x - 1, tempCard.Deck) == true)
                {
                    thisComputer = new ();
                    thisComputer.CardToPlay = tempCard;
                    thisComputer.Pile = x - 1;
                    thisComputer.Discard = -1;
                    thisComputer.WhichType = EnumCardType.MyCards;
                    thisList.Add(thisComputer);
                }
            }
        }
        FlinchCardInformation thisCard;
        int y;
        var thisDiscard = _model.DiscardPiles;
        DeckRegularDict<FlinchCardInformation> cardList;
        for (x = 1; x <= HowManyDiscards; x++)
        {
            cardList = thisDiscard!.PileList![x - 1].ObjectList;
            if (cardList.Count > 0)
            {
                thisCard = thisDiscard.GetLastCard(x - 1);
                for (y = 1; y <= MaxPiles; y++)
                {
                    if (_gameContainer.IsValidMove(y - 1, thisCard.Deck) == true)
                    {
                        thisComputer = new ();
                        thisComputer.CardToPlay = thisCard;
                        thisComputer.Pile = y - 1;
                        thisComputer.Discard = x - 1; // forgot -1 here too.
                        thisComputer.WhichType = EnumCardType.Discard;
                        thisList.Add(thisComputer);
                    }
                }
            }
        }
        thisCard = _model.StockPile!.GetCard();
        for (x = 1; x <= MaxPiles; x++)
        {
            if (_gameContainer.IsValidMove(x - 1, thisCard.Deck) == true)
            {
                thisComputer = new ();
                thisComputer.CardToPlay = thisCard;
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
        _tempHand = new DeckRegularDict<FlinchCardInformation>();
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
        FlinchCardInformation thisCard;
        for (var x = 1; x <= HowManyDiscards; x++)
        {
            thisTemp = new ();
            thisTemp.Pile = x - 1; // because 0 based
            if (thisDiscard!.PileList![x - 1].ObjectList.Count == 1)
            {
                thisCard = thisDiscard.PileList[x - 1].ObjectList.Single();
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

    private ComputerDiscardInfo FindBestStack()
    {
        ComputerDiscardInfo output = new();
        output.Deck = 0;
        output.Pile = 0;
        ComputerDiscardInfo thisDiscard;
        int newNums = default;
        BasicList<ComputerDiscardInfo> newList = new();
        FlinchCardInformation thisCard;
        foreach (var thisTemp in _tempDiscardList!)
        {
            if (thisTemp.Number1 < 13 && thisTemp.Number1 > 1)
            {
                newNums = thisTemp.Number1 - 1;
                var Filters = _gameContainer.SingleInfo!.MainHandList.Where(x => x.Number == newNums).ToRegularDeckDict();
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
        BasicList<ComputerDiscardInfo> newList = new();
        int newNums = default;
        ComputerDiscardInfo thisDiscard;
        FlinchCardInformation thisCard;
        foreach (var thisTemp in stacks)
        {
            if (thisTemp.Number2 < 13 && thisTemp.Number2 > 1)
            {
                newNums = thisTemp.Number1 - 1;
                var Filters = _gameContainer.SingleInfo!.MainHandList.Where(x => x.Number == newNums).ToRegularDeckDict();
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
        FlinchCardInformation thisCard;
        foreach (var thisTemp in sames)
        {
            if (thisTemp.Number1 < 13)
            {
                var filters = _gameContainer.SingleInfo!.MainHandList.Where(x => x.Number == thisTemp.Number1).ToRegularDeckDict();
                if (filters.Count > 0)
                {
                    thisCard = filters.First();
                    thisDiscard = new ();
                    thisDiscard.Deck = thisCard.Deck;
                    thisDiscard.Pile = thisTemp.Pile;
                    newList.Add(thisDiscard);
                }
            }
        }
        if (newList.Count == 0)
            return output;
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
        FlinchCardInformation thisCard;
        foreach (var thisTemp in ones)
        {
            var filters = _gameContainer.SingleInfo!.MainHandList.Where(x => x.Number == thisTemp.Number1).ToRegularDeckDict();
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
            return newList.GetRandomItem();
        int newNums;
        foreach (var thisTemp in ones)
        {
            newNums = thisTemp.Number1 - 1;
            if (newNums > 0 && newNums < 12)
            {
                var filters = _gameContainer.SingleInfo!.MainHandList.Where(x => x.Number == newNums).ToRegularDeckDict();
                if (filters.Count > 0)
                {
                    thisCard = filters.First();
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
    private int BestCardForEmpty()
    {
        if (_gameContainer.SingleInfo!.MainHandList.Count == 1)
        {
            return _gameContainer.SingleInfo.MainHandList.First().Deck;
        }
        return _gameContainer.SingleInfo.MainHandList.GetRandomItem().Deck;
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
        DiscardPileInfo newFinds;
        newFinds = _tempDiscardList.GetRandomItem();
        output.Pile = newFinds.Pile;
        output.Deck = BestCardForEmpty();
        return output;
    }
    private static FlinchCardInformation? FindDiscard(DiscardPilesVM<FlinchCardInformation> thisDiscard, int pile)
    {
        if (thisDiscard.PileList![pile].ObjectList.Count == 0)
        {
            return null;
        }
        return thisDiscard.GetLastCard(pile);
    }

    private int DiscardFromStack(int nextNumber, int stockNumber)
    {
        DiscardPilesVM<FlinchCardInformation> thisDiscard;
        thisDiscard = _tempDiscards!;
        FlinchCardInformation thisCard;
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
        if (int.Parse(stockNumber) == nextNumber)
        {
            output.Pile = pile;
            output.CardToPlay = _model.StockPile!.GetCard();
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
            output.CardToPlay = ThisCard;
            return output;
        }
        foreach (var thisCard in _tempHand!)
        {
            if (thisCard.Number == nextNumber)
            {
                output.Pile = pile;
                output.Discard = 0;
                output.CardToPlay = thisCard;
                output.WhichType = EnumCardType.MyCards;
                return output;
            }
        }
        int x;
        for (x = 1; x <= HowManyDiscards; x++)
        {
            var thisCard = FindDiscard(_tempDiscards!, x - 1);
            if (thisCard == null == false && thisCard!.Number == nextNumber)
            {
                output.Pile = pile;
                output.Discard = x - 1; // because 0 based
                output.CardToPlay = thisCard;
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
        thisMove = whatList[whichOne - 1];
        MoveInfo tempMove;
        if (thisMove.WhichType == EnumCardType.Stock)
        {
            tempMove = new ();
            tempMove.MoveList = new();
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
            if (numberNext == 16)
            {
                numberNext = 1;
            }
            newMove = NextMove(numberNext, thisMove.Pile);
            if (newMove.WhichType == EnumCardType.IsNone)
            {
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
        int newValue;
        newValue = 0;
        int x;
        if (newMove.WhichType == EnumCardType.Stock)
        {
            newValue += 400;
            if (thisMove.MoveList.Count > 1)
            {
                var loopTo = thisMove.MoveList.Count;
                for (x = 2; x <= loopTo; x++)
                {
                    newValue -= 20;
                }
            }
        }
        FlinchCardInformation thisCard;
        thisCard = newMove.CardToPlay!;
        if (newMove.WhichType == EnumCardType.MyCards)
        {
            _tempHand!.RemoveSpecificItem(thisCard);
            if (_tempHand.Count == 0)
            {
                thisMove.GetMore = true;
                newValue += 60;
            }
        }
        if (newMove.WhichType == EnumCardType.Discard)
        {
            newValue += 20;
            _tempDiscards!.RemoveCard(newMove.Discard, newMove.CardToPlay!.Deck);
            if (_model.DiscardPiles!.PileList![newMove.Discard].ObjectList.Count == 0)
            {
                throw new CustomBasicException("Cannot be 0 for the discards");
            }
        }
        thisMove.Value += newValue;
    }
}
