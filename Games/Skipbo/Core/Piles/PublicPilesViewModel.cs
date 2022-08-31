﻿namespace Skipbo.Core.Piles;
public class PublicPilesViewModel : BasicMultiplePilesCP<SkipboCardInformation>
{
    public void UnselectAllPiles()
    {
        int x;
        var loopTo = PileList!.Count;
        for (x = 1; x <= loopTo; x++)
        {
            UnselectPile(x - 1);// because 0 based
        }
    }
    public DeckRegularDict<SkipboCardInformation> CardsFromPile(int pile)
    {
        var output = new DeckRegularDict<SkipboCardInformation>();
        var thisPile = PileList![pile];
        output.AddRange(thisPile.ObjectList);
        thisPile.ObjectList.Clear();
        if (output.Count == 0)
        {
            throw new CustomBasicException("Can't clear the list out");
        }
        return output;
    }
    public int NextNumberNeeded(int whichOne) // has to send in 0 based because inherited now.
    {
        var thisPile = PileList![whichOne];
        if (thisPile.ObjectList.Count > 12)
        {
            throw new CustomBasicException("Should have cleared out the piles");
        }
        return thisPile.ObjectList.Count + 1;
    }
    public bool NeedToRemovePile(int whichOne)
    {
        var thisPile = PileList![whichOne];
        if (thisPile.ObjectList.Count < 12)
        {
            return false;
        }
        return true;
    }
    public DeckRegularDict<SkipboCardInformation> EmptyPileList(int whichOne)
    {
        var thisPile = PileList![whichOne];
        if (thisPile.ObjectList.Count != 12)
        {
            throw new CustomBasicException($"Must have 12 cards to empty a pile; not {thisPile.ObjectList.Count}");
        }
        var tempList = thisPile.ObjectList.ToRegularDeckDict();
        thisPile.ObjectList.Clear(); // i think this is fine
        if (tempList.Count != 12)
        {
            throw new CustomBasicException($"Must have 12 cards in the list, not {tempList.Count}");
        }
        return tempList;
    }
    public PublicPilesViewModel(CommandContainer command) : base(command)
    {
        Style = EnumMultiplePilesStyleList.HasList;
        Rows = 1;
        HasFrame = true;
        HasText = true;
        Columns = 4;
        LoadBoard();
        int x = 0;
        if (PileList!.Count != 4)
        {
            throw new CustomBasicException("Should have had 4 piles");
        }
        foreach (var thisPile in PileList)
        {
            x += 1;
            thisPile.Text = "Pile " + x;
        }
    }
}