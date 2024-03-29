﻿namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.StockClasses;
public class DiscardPilesVM<D>(CommandContainer command) : BasicMultiplePilesCP<D>(command)
    where D : IDeckObject, new()
{
    private readonly CommandContainer _command = command;
    public void AddToEmptyDiscard(D thisCard)
    {
        foreach (var thisPile in PileList!)
        {
            if (thisPile.ObjectList.Count == 0)
            {
                thisPile.ObjectList.Add(thisCard);
                return;
            }
        }
        throw new CustomBasicException("There was no empty piles left.  Find out what happened");
    }
    public void AddToDiscard(int pile, D thisCard)
    {
        PileList![pile - 1].ObjectList.Add(thisCard);
    }
    public void ClearCards()
    {
        foreach (var pile in PileList!)
        {
            pile.ObjectList.Clear();
        }
    }
    public int CardSelected(out int whichPile)
    {
        int x = 0;
        D thisCard;
        foreach (var thisPile in PileList!)
        {
            if (thisPile.ObjectList.Count > 0)
            {
                thisCard = thisPile.ObjectList.Last();
                if (thisCard.IsSelected == true)
                {
                    whichPile = x;
                    return thisCard.Deck;
                }
            }
            x += 1;
        }
        whichPile = -1;
        return -1;
    }
    public bool CanAddToDiscardFlinch(int pile)
    {
        BasicPileInfo<D> thisPile;
        thisPile = PileList![pile];
        if (thisPile.ObjectList.Count == 0)
        {
            return true;
        }
        if (HasEmptyPile() == true)
        {
            return false;
        }
        return true;
    }
    private bool HasEmptyPile()
    {
        foreach (var thisPile in PileList!)
        {
            if (thisPile.ObjectList.Count == 0)
            {
                return true;
            }
        }
        return false;
    }
    public void RemoveCard(int pile, int deck)
    {
        BasicPileInfo<D> thisPile;
        thisPile = PileList![pile];
        thisPile.ObjectList.RemoveObjectByDeck(deck);
        thisPile.IsSelected = false;
    }
    public void Init(int howManyPiles)
    {
        Columns = howManyPiles;
        Rows = 1;
        Style = EnumMultiplePilesStyleList.HasList;
        HasFrame = true;
        HasText = true;
        LoadBoard();
        foreach (var thisPile in PileList!)
        {
            thisPile.Text = $"Discard {PileList.IndexOf(thisPile) + 1}";
        }
    }
    public void CopyDiscards(out DiscardPilesVM<D> newDiscard)
    {
        newDiscard = new DiscardPilesVM<D>(_command);
        newDiscard.Init(Columns);
        int x = 0;
        foreach (var thisPile in PileList!)
        {
            x += 1;
            var newPile = newDiscard.PileList![x - 1];
            foreach (var thisCard in thisPile.ObjectList)
            {
                newPile.ObjectList.Add(thisCard);
            }
        }
    }
    public int NextToLastDeck(int pile)
    {
        var thisPile = PileList![pile];
        D thisCard;
        thisCard = thisPile.ObjectList[thisPile.ObjectList.Count - 1];
        return thisCard.Deck;
    }
    public int FirstDeck(int pile) 
    {
        D thisCard;
        var thisPile = PileList![pile - 1];
        thisCard = thisPile.ObjectList.First();
        return thisCard.Deck;
    }
    public void UnselectAllCards()
    {
        int x = 0;
        foreach (var thisPile in PileList!)
        {
            x += 1;
            UnselectPile(x - 1);
        }
    }
}