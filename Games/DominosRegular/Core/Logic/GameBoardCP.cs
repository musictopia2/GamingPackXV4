﻿namespace DominosRegular.Core.Logic;
public partial class GameBoardCP : SimpleControlObservable
{

    public DeckRegularDict<SimpleDominoInfo> DominoList = new();
    public ControlCommand? DominoCommand { get; set; }
    private DominosRegularSaveInfo? _saveRoot;
    [Command(EnumCommandCategory.Control)]
    private async Task PrivateDominoClicked(SimpleDominoInfo domino)
    {
        if (DominoPileClicked == null)
        {
            throw new CustomBasicException("No domino pile function was created.  Rethink");
        }
        if (domino.Deck == CenterDomino.Deck)
        {
            throw new CustomBasicException("Should never click on center domino.");
        }
        if (domino.Deck == FirstDomino.Deck)
        {
            await DominoPileClicked.Invoke(1);
            return;
        }
        if (domino.Deck == SecondDomino.Deck)
        {
            await DominoPileClicked.Invoke(2);
            return;
        }
        throw new CustomBasicException("Problem");
    }
    internal Func<int, Task>? DominoPileClicked { get; set; }
    public GameBoardCP(CommandContainer container) : base(container)
    {
        CreateCommands();
    }
    partial void CreateCommands();
    public SimpleDominoInfo CenterDomino
    {
        get
        {
            return _saveRoot!.CenterDomino!;
        }
        set
        {
            _saveRoot!.CenterDomino = value;
        }
    }
    public SimpleDominoInfo FirstDomino
    {
        get
        {
            return _saveRoot!.FirstDomino!;
        }
        set
        {
            _saveRoot!.FirstDomino = value;
        }
    }
    public SimpleDominoInfo SecondDomino
    {
        get
        {
            return _saveRoot!.SecondDomino!;
        }
        set
        {
            _saveRoot!.SecondDomino = value;
        }
    }
    public void LoadSavedGame(DominosRegularSaveInfo saveRoot)
    {
        _saveRoot = saveRoot;
        DominoList.ReplaceRange(new BasicList<SimpleDominoInfo> { FirstDomino, CenterDomino, SecondDomino });
    }
    public void ClearBoard(DominosRegularSaveInfo saveRoot)
    {
        _saveRoot = saveRoot;
        FirstDomino = new ();
        FirstDomino.Deck = -1;
        SecondDomino = new ();
        SecondDomino.Deck = -2;
        CenterDomino = new ();
        CenterDomino.Deck = -3;
        DominoList.ReplaceRange(new BasicList<SimpleDominoInfo> { FirstDomino, CenterDomino, SecondDomino });
    }
    public void PopulateCenter(SimpleDominoInfo thisDomino)
    {
        if (DominoList.Count != 3)
        {
            throw new CustomBasicException("Must have 3 dominos before populating center");
        }
        DominoList.ReplaceItem(CenterDomino, thisDomino);
        CenterDomino = thisDomino; //just in case.
    }
    protected override void EnableChange()
    {
        DominoCommand!.ReportCanExecuteChange();
    }
    protected override void PrivateEnableAlways() { }
    public bool IsValidMove(int whichOne, SimpleDominoInfo thisDomino)
    {
        if (FirstDomino.Deck <= 0 && whichOne == 1 || SecondDomino.Deck <= 0 && whichOne == 2)
        {
            if (CenterDomino.FirstNum == CenterDomino.SecondNum)
            {
                if (thisDomino.FirstNum == CenterDomino.FirstNum | thisDomino.SecondNum == CenterDomino.FirstNum)
                {
                    return true;
                }
                if (thisDomino.FirstNum == CenterDomino.FirstNum && whichOne == 1)
                {
                    return true;
                }
                if (thisDomino.SecondNum == CenterDomino.FirstNum && whichOne == 1)
                {
                    return true;
                }
                if (thisDomino.FirstNum == CenterDomino.SecondNum && whichOne == 2)
                {
                    return true;
                }
                if (thisDomino.SecondNum == CenterDomino.SecondNum && whichOne == 2)
                {
                    return true;
                }
            }
            return false;
        }
        if (whichOne == 1 & FirstDomino.CurrentFirst == thisDomino.FirstNum)
        {
            return true;
        }
        if (whichOne == 1 & FirstDomino.CurrentFirst == thisDomino.SecondNum)
        {
            return true;
        }
        if (whichOne == 1)
        {
            return false;
        }
        if (whichOne != 2)
        {
            throw new CustomBasicException("Must be 1 or 2; not " + whichOne);
        }
        if (SecondDomino.CurrentSecond == thisDomino.FirstNum)
        {
            return true;
        }
        if (SecondDomino.CurrentSecond == thisDomino.SecondNum)
        {
            return true;
        }
        return false;
    }
    public void MakeMove(int whichOne, SimpleDominoInfo thisDomino)
    {
        if (whichOne != 1 && whichOne != 2)
        {
            throw new CustomBasicException("Must be 1 or 2 for makemove");
        }
        thisDomino.IsEnabled = true;
        thisDomino.IsSelected = false;
        thisDomino.Drew = false;
        thisDomino.CurrentFirst = thisDomino.FirstNum;
        thisDomino.CurrentSecond = thisDomino.SecondNum; // make sure its not rotated
        if (SecondDomino.Deck <= 0 && whichOne == 2)
        {
            SecondDomino.CurrentSecond = CenterDomino.FirstNum;
        }
        if (FirstDomino.Deck <= 0 && whichOne == 1)
        {
            FirstDomino.CurrentFirst = CenterDomino.FirstNum;
        }
        if (whichOne == 1)
        {
            if (FirstDomino.CurrentFirst == thisDomino.FirstNum & thisDomino.FirstNum != thisDomino.SecondNum)
            {
                thisDomino.CurrentFirst = thisDomino.SecondNum;
                thisDomino.CurrentSecond = thisDomino.FirstNum;
            }
            DominoList.ReplaceItem(FirstDomino, thisDomino);
            FirstDomino = thisDomino;
        }
        else
        {
            if (SecondDomino.CurrentSecond == thisDomino.SecondNum & thisDomino.FirstNum != thisDomino.SecondNum)
            {
                thisDomino.CurrentFirst = thisDomino.SecondNum;
                thisDomino.CurrentSecond = thisDomino.FirstNum;
            }
            DominoList.ReplaceItem(SecondDomino, thisDomino);
            SecondDomino = thisDomino;
        }
    }
}