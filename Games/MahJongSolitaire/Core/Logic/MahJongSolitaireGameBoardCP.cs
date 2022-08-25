namespace MahJongSolitaire.Core.Logic;
[SingletonGame]
public class MahJongSolitaireGameBoardCP
{
    private readonly MahJongSolitaireSaveInfo _saveRoot;
    private readonly BaseMahjongGlobals _customGlobal;
    private readonly MahJongSolitaireModGlobal _mainGlobal;
    private readonly SizeF _sizeUsed;
    public BasicList<BoardInfo> GetPriorityBoards()
    {
        return (from items in _saveRoot.BoardList
                orderby items.Floor, items.BoardCategory, items.RowStart
                select items).ToBasicList();
    }
    public MahJongSolitaireGameBoardCP(MahJongSolitaireSaveInfo saveRoot, BaseMahjongGlobals customGlobal
        , MahJongSolitaireModGlobal mainGlobal)
    {
        _saveRoot = saveRoot;
        _customGlobal = customGlobal;
        _mainGlobal = mainGlobal;
        _sizeUsed = new SizeF(136, 176);
        if (saveRoot.BoardList.Count > 0)
        {
            throw new CustomBasicException("The saveroot should have cleared out every game.  Rethink");
        }
        FirstLoad();
    }
    private void FirstLoad()
    {
        // this will help on positioning.
        BoardInfo thisBoard;
        thisBoard = new ();
        thisBoard.Floor = 1;
        thisBoard.BoardCategory = BoardInfo.EnumBoardCategory.FarLeft;
        _saveRoot.BoardList.Add(thisBoard);
        thisBoard = new ();
        thisBoard.Floor = 1;
        thisBoard.BoardCategory = BoardInfo.EnumBoardCategory.FarRight; // always holding 2 tiles.
        _saveRoot.BoardList.Add(thisBoard);
        thisBoard = new ();
        thisBoard.Floor = 1;
        thisBoard.RowStart = 0; // should be 0 based.
        thisBoard.ColumnStart = 1; // first column is for holding the last one.
        thisBoard.HowManyColumns = 12;
        _saveRoot.BoardList.Add(thisBoard);
        thisBoard = new ();
        thisBoard.Floor = 1;
        thisBoard.RowStart = 1;
        thisBoard.ColumnStart = 3;
        thisBoard.HowManyColumns = 8;
        _saveRoot.BoardList.Add(thisBoard);
        thisBoard = new ();
        thisBoard.Floor = 1;
        thisBoard.RowStart = 2;
        thisBoard.ColumnStart = 2;
        thisBoard.HowManyColumns = 10;
        _saveRoot.BoardList.Add(thisBoard);
        int x;
        for (x = 3; x <= 4; x++)
        {
            thisBoard = new ();
            thisBoard.Floor = 1;
            thisBoard.RowStart = x;
            thisBoard.ColumnStart = 1;
            thisBoard.HowManyColumns = 12;
            _saveRoot.BoardList.Add(thisBoard);
        }
        thisBoard = new ();
        thisBoard.Floor = 1;
        thisBoard.RowStart = 5;
        thisBoard.ColumnStart = 2;
        thisBoard.HowManyColumns = 10;
        _saveRoot.BoardList.Add(thisBoard);
        thisBoard = new ();
        thisBoard.Floor = 1;
        thisBoard.RowStart = 6;
        thisBoard.ColumnStart = 3;
        thisBoard.HowManyColumns = 8;
        _saveRoot.BoardList.Add(thisBoard);
        thisBoard = new ();
        thisBoard.Floor = 1;
        thisBoard.RowStart = 7;
        thisBoard.ColumnStart = 1;
        thisBoard.HowManyColumns = 12;
        _saveRoot.BoardList.Add(thisBoard);
        int manys;
        manys = 6;
        int row;
        int column;
        row = 1;
        column = 4;
        int tempRow;
        int y;
        for (x = 2; x <= 4; x++)
        {
            tempRow = row;
            var loopTo = manys;
            for (y = 1; y <= loopTo; y++)
            {
                thisBoard = new ();
                thisBoard.RowStart = tempRow;
                thisBoard.ColumnStart = column;
                thisBoard.HowManyColumns = manys;
                thisBoard.Floor = x;
                _saveRoot.BoardList.Add(thisBoard);
                tempRow += 1;
            }
            row += 1;
            column += 1;
            manys -= 2;
        }
        thisBoard = new ();
        thisBoard.Floor = 5;
        thisBoard.BoardCategory = BoardInfo.EnumBoardCategory.VeryTop;
        _saveRoot.BoardList.Add(thisBoard);
    }
    public void ClearBoard()
    {
        _saveRoot.BoardList.ForEach(board =>
        {
            board.TileList.Clear();
            board.BackTaken = 0;
            board.FrontTaken = 0;
        });
    }
    public void PositionTiles()
    {
        float currentLeft;
        float currentTop;
        BoardInfo thisBoard;
        float adds;
        adds = 0;
        var thisList = (from x in _saveRoot.BoardList
                        where x.BoardCategory == BoardInfo.EnumBoardCategory.Regular
                        select x).ToBasicList();
        foreach (var tempBoard in thisList)
        {
            currentLeft = tempBoard.ColumnStart * (_sizeUsed.Width + adds);
            if (currentLeft < 0)
                currentLeft = 0;
            currentTop = tempBoard.RowStart * (_sizeUsed.Height + adds);
            if (currentTop < 0)
                currentTop = 0;
            foreach (var thisCard in tempBoard.TileList)
            {
                thisCard.Left = currentLeft;
                thisCard.Top = currentTop;
                currentLeft += _sizeUsed.Width; // because 3d style (well see)
            }
        }
        thisBoard = (from x in _saveRoot.BoardList
                     where x.BoardCategory == BoardInfo.EnumBoardCategory.FarRight
                     select x).Single();
        var otherBoard = (from Items in _saveRoot.BoardList
                          where Items.Floor == 1 && Items.RowStart == 3
                          select Items).Single();
        if (otherBoard.TileList.Count == 0)
        {
            throw new CustomBasicException("No cards to give any hints");
        }
        currentLeft = otherBoard.TileList.Last().Left + _sizeUsed.Width; // because 3d style
        currentTop = otherBoard.TileList.Last().Top + _sizeUsed.Height / 2;
        foreach (var thisCard in thisBoard.TileList)
        {
            thisCard.Left = currentLeft;
            thisCard.Top = currentTop;
            currentLeft += _sizeUsed.Width;
        }
        thisBoard = (from x in _saveRoot.BoardList
                     where x.BoardCategory == BoardInfo.EnumBoardCategory.FarLeft
                     select x).Single();
        if (thisBoard.TileList.Count > 0)
        {
            thisBoard.TileList.Single().Left = 0;
            thisBoard.TileList.Single().Top = currentTop;
        }

        thisBoard = (from xx in _saveRoot.BoardList
                     where xx.BoardCategory == BoardInfo.EnumBoardCategory.VeryTop
                     select xx).Single();
        otherBoard = (from xx in _saveRoot.BoardList
                      where xx.Floor == 4 && xx.RowStart == 3
                      select xx).Single();
        if (otherBoard.TileList.Count == 0)
        {
            throw new CustomBasicException("No cards to give any hints");
        }
        currentLeft = otherBoard.TileList.First().Left + (_sizeUsed.Width / 2);
        currentTop = otherBoard.TileList.First().Top + (_sizeUsed.Height / 2);
        thisBoard.TileList.Single().Left = currentLeft;
        thisBoard.TileList.Single().Top = currentTop;
        CheckForValidTiles();
        Check3DTiles();
    }
    internal DeckRegularDict<MahjongSolitaireTileInfo> ValidList { get; set; } = new ();
    private void UpdateTile(MahjongSolitaireTileInfo thisTile)
    {
        if (_customGlobal.CanShowTiles == true)
        {
            thisTile.IsEnabled = true;
        }
        if (ValidList.ObjectExist(thisTile.Deck) == false)
        {
            ValidList.Add(thisTile);
        }
    }
    public void CheckForValidTiles()
    {
        ValidList = new DeckRegularDict<MahjongSolitaireTileInfo>();
        var firstBoard = (from items in _saveRoot.BoardList
                          where items.BoardCategory == BoardInfo.EnumBoardCategory.FarLeft
                          select items).Single();
        bool hasFarLeft;
        hasFarLeft = false;
        foreach (var thisCard in firstBoard.TileList)
        {
            UpdateTile(thisCard);
            thisCard.IsEnabled = true;
            hasFarLeft = true;
        }
        firstBoard = (from items in _saveRoot.BoardList
                      where items.BoardCategory == BoardInfo.EnumBoardCategory.VeryTop
                      select items).Single();
        bool hasVeryTop;
        hasVeryTop = false;
        foreach (var thisCard in firstBoard.TileList)
        {
            UpdateTile(thisCard);
            hasVeryTop = true;
        }
        bool hasFarRight;
        hasFarRight = false;
        firstBoard = (from items in _saveRoot.BoardList
                      where items.BoardCategory == BoardInfo.EnumBoardCategory.FarRight
                      select items).Single();
        if (firstBoard.TileList.Count > 0)
        {
            hasFarRight = true;
            UpdateTile(firstBoard.TileList.Last());
        }
        BasicList<BoardInfo> tempList;
        if (hasVeryTop == false)
        {
            tempList = (from items in _saveRoot.BoardList
                        where items.Floor == 4
                        select items).ToBasicList();
            foreach (var tempBoard in tempList)
            {
                foreach (var ThisCard in tempBoard.TileList)
                {
                    UpdateTile(ThisCard);
                }
            }
        }
        tempList = (from items in _saveRoot.BoardList
                    where items.BoardCategory == BoardInfo.EnumBoardCategory.Regular && items.Floor == 1
                    select items).ToBasicList();
        foreach (var thisBoard in tempList)
        {
            if (thisBoard.TileList.Count > 0)
            {
                bool canUpdateLeftSoFar;
                bool canUpdateRightSoFar;
                canUpdateLeftSoFar = true;
                canUpdateRightSoFar = true;
                if (thisBoard.RowStart == 3 || thisBoard.RowStart == 4)
                {
                    if (hasFarLeft == true)
                    {
                        canUpdateLeftSoFar = false;
                    }
                    if (hasFarRight == true)
                    {
                        canUpdateRightSoFar = false;
                    }
                }
                if (thisBoard.FrontTaken == 0)
                {
                    if (canUpdateLeftSoFar == true)
                    {
                        UpdateTile(thisBoard.TileList.First());
                    }
                }
                if (thisBoard.BackTaken == 0)
                {
                    if (canUpdateRightSoFar == true)
                    {
                        UpdateTile(thisBoard.TileList.Last());
                    }
                }
                if (thisBoard.FrontTaken == 0 && thisBoard.BackTaken == 0)
                {
                    continue;
                }
                var tempBoard = (from items in _saveRoot.BoardList
                                 where items.Floor == thisBoard.Floor + 1 && items.RowStart == thisBoard.RowStart
                                 select items).SingleOrDefault();
                if (tempBoard == null || tempBoard.TileList.Count == 0)
                {
                    if (canUpdateLeftSoFar == true)
                    {
                        UpdateTile(thisBoard.TileList.First());
                    }
                    if (canUpdateRightSoFar == true)
                    {
                        UpdateTile(thisBoard.TileList.Last());
                    }
                    continue;
                }
                float realFirst;
                float realSecond;
                if (thisBoard.FrontTaken > 0)
                {
                    realFirst = thisBoard.ColumnStart + thisBoard.FrontTaken;
                    realSecond = tempBoard.ColumnStart + tempBoard.FrontTaken;
                    if (realFirst < realSecond)
                    {
                        if (canUpdateLeftSoFar == true)
                        {
                            UpdateTile(thisBoard.TileList.First());
                        }
                    }
                }
                if (thisBoard.BackTaken > 0)
                {
                    realFirst = thisBoard.ColumnStart + thisBoard.HowManyColumns - thisBoard.BackTaken;
                    realSecond = tempBoard.ColumnStart + tempBoard.HowManyColumns - tempBoard.BackTaken;
                    if (realFirst > realSecond)
                    {
                        if (canUpdateRightSoFar == true)
                        {
                            UpdateTile(thisBoard.TileList.Last());
                        }
                    }
                }
            }
        }
        tempList = (from items in _saveRoot.BoardList
                    where items.Floor == 2 || items.Floor == 3
                    select items).ToBasicList();
        foreach (var thisBoard in tempList)
        {
            if (thisBoard.TileList.Count > 0)
            {
                if (thisBoard.FrontTaken == 0)
                {
                    UpdateTile(thisBoard.TileList.First());
                }
                if (thisBoard.BackTaken == 0)
                {
                    UpdateTile(thisBoard.TileList.Last());
                }
                if (thisBoard.FrontTaken > 0 || thisBoard.BackTaken > 0)
                {
                    var tempBoard = (from items in _saveRoot.BoardList
                                     where items.Floor == thisBoard.Floor + 1 && items.RowStart == thisBoard.RowStart
                                     select items).SingleOrDefault();
                    if (tempBoard == null || tempBoard.TileList.Count == 0)
                    {
                        UpdateTile(thisBoard.TileList.First());
                        UpdateTile(thisBoard.TileList.Last());
                    }
                    else
                    {
                        float realFirst;
                        float realSecond;
                        if (thisBoard.FrontTaken > 0)
                        {
                            realFirst = thisBoard.ColumnStart + thisBoard.FrontTaken;
                            realSecond = tempBoard.ColumnStart + tempBoard.FrontTaken;
                            if (realFirst < realSecond)
                            {
                                UpdateTile(thisBoard.TileList.First());
                            }
                        }
                        if (thisBoard.BackTaken > 0)
                        {
                            realFirst = thisBoard.ColumnStart + thisBoard.HowManyColumns - thisBoard.BackTaken;
                            realSecond = tempBoard.ColumnStart + tempBoard.HowManyColumns - tempBoard.BackTaken;
                            if (realFirst > realSecond)
                            {
                                UpdateTile(thisBoard.TileList.Last());
                            }
                        }
                    }
                }
            }
        }
    }
    public void UnselectTiles()
    {
        _mainGlobal.TileList.UnselectAllObjects();
        _saveRoot.FirstSelected = 0;
        _mainGlobal.SecondSelected = 0;
    }
    public bool IsGameOver()
    {
        return !_saveRoot.BoardList.Any(items => items.TileList.Count > 0); //try this way.
    }
    private void RemoveSpecificTile(MahjongSolitaireTileInfo thisTile)
    {
        foreach (var thisBoard in _saveRoot.BoardList)
        {
            if (thisBoard.TileList.Count >= 2)
            {
                if (thisBoard.TileList.First().Deck == thisTile.Deck)
                {
                    thisBoard.FrontTaken += 1;
                    RemoveTile(thisBoard, thisBoard.TileList.First());
                    return;
                }
                if (thisBoard.TileList.Last().Deck == thisTile.Deck)
                {
                    thisBoard.BackTaken += 1;
                    RemoveTile(thisBoard, thisBoard.TileList.Last());
                    return;
                }
            }
            else if (thisBoard.TileList.Count == 1)
            {
                if (thisBoard.TileList.Single().Deck == thisTile.Deck)
                {
                    RemoveTile(thisBoard, thisBoard.TileList.Single());
                    return;
                }
            }
        }
        if (_customGlobal.CanShowTiles == true)
        {
            throw new CustomBasicException("No tile to remove");
        }
    }
    private void RemoveTile(BoardInfo thisBoard, MahjongSolitaireTileInfo thisTile)
    {
        thisBoard.TileList.RemoveSpecificItem(thisTile);
        if (_customGlobal.CanShowTiles == true)
        {
            thisTile.Visible = false;
        }
    }
    private DeckRegularDict<MahjongSolitaireTileInfo> GetPreviousTiles()
    {
        BoardInfo firstBoard;
        BoardInfo secondBoard;
        DeckRegularDict<MahjongSolitaireTileInfo> output = new();
        for (int x = 0; x < _saveRoot.PreviousList.Count; x++)
        {
            firstBoard = _saveRoot.PreviousList[x];
            secondBoard = _saveRoot.BoardList[x];
            if (firstBoard.TileList.Count != secondBoard.TileList.Count)
            {
                firstBoard.TileList.ForEach(tile =>
                {
                    if (secondBoard.TileList.ObjectExist(tile.Deck) == false)
                    {
                        tile.IsSelected = false;
                        output.Add(tile);
                    }
                });
            }
        }
        return output;
    }
    public void PopulateBoardFromUndo()
    {
        if (_saveRoot.PreviousList.Count != _saveRoot.BoardList.Count)
        {
            throw new CustomBasicException("Count don't reconcile");
        }
        var thisList = GetPreviousTiles();
        if (thisList.Count != 2)
        {
            throw new CustomBasicException("Must have 2 cards only because must be one pair at a time");
        }
        _saveRoot.BoardList = _saveRoot.PreviousList.ToBasicList();
        _mainGlobal.TileList.ClearObjects();
        _saveRoot.BoardList.ForEach(tempBoard =>
        {
            _mainGlobal.TileList.AddRelinkedTiles(tempBoard.TileList);
            tempBoard.TileList.ForEach(tempTile =>
            {
                tempTile.Visible = true;
                tempTile.IsEnabled = false;
            });
        });
        CheckForValidTiles();
        Check3DTiles();
    }
    public void ProcessPair(bool isAuto)
    {
        var firstTile = _mainGlobal.TileList.GetSpecificItem(_saveRoot.FirstSelected);
        var secondTile = _mainGlobal.TileList.GetSpecificItem(_mainGlobal.SecondSelected);
        if (isAuto == false)
        {
            firstTile.IsSelected = false;
            secondTile.IsSelected = false;
            _saveRoot.PreviousList = _saveRoot.BoardList.Clone();
            //looks like cloning is not working currently if you are cloning a list.  that can be a common use case.
            //CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions.ModelExtensions.Clone(_saveRoot.BoardList);
            //_saveRoot.BoardList.Clone();
            //_saveRoot.BoardList.Clone();
            //await MahJongSolitaireStaticFunctions.SaveMoveAsync(_saveRoot);
        }
        RemoveSpecificTile(firstTile);
        RemoveSpecificTile(secondTile);
        _saveRoot.FirstSelected = 0;
        _mainGlobal.SecondSelected = 0;
        if (isAuto == false)
        {
            _saveRoot.TilesGone += 2;
        }
        ValidList.Clear();
        CheckForValidTiles();
        if (isAuto == false)
        {
            Check3DTiles();
        }
    }
    public void Check3DTiles()
    {
        foreach (var thisTemp in _saveRoot.BoardList)
        {
            foreach (var tempCard in thisTemp.TileList)
            {
                tempCard.NeedsBottom = false;
                tempCard.NeedsTop = false;
                tempCard.NeedsRight = false;
                tempCard.NeedsLeft = false;
            }
        };
        var thisList = _saveRoot.BoardList.ToBasicList();
        var thisBoard = _saveRoot.BoardList.Where(x => x.BoardCategory == BoardInfo.EnumBoardCategory.VeryTop).Single();
        thisBoard.TileList.ForEach(item =>
        {
            item.NeedsBottom = true;
            item.NeedsLeft = true;
            item.NeedsRight = true;
            item.NeedsTop = true;
        });
        thisList.RemoveSpecificItem(thisBoard);
        thisList.KeepConditionalItems(xx => xx.Floor == 2);
        foreach (var firsts in thisList)
        {
            if (firsts.TileList.Count > 0)
            {
                firsts.TileList.First().NeedsLeft = true;
                firsts.TileList.Last().NeedsRight = true;
                foreach (var card in firsts.TileList)
                {
                    if (firsts.RowStart == 1)
                    {
                        card.NeedsTop = true;
                    }
                    else if (firsts.RowStart == 6)
                    {
                        card.NeedsBottom = true;
                    }
                }
            }
        }
        thisList = (from xx in _saveRoot.BoardList
                    where xx.Floor == 3
                    select xx).ToBasicList();
        foreach (var firsts in thisList)
        {
            if (firsts.TileList.Count > 0)
            {
                firsts.TileList.First().NeedsLeft = true;
                firsts.TileList.Last().NeedsRight = true;
                foreach (var thisCard in firsts.TileList)
                {
                    if (firsts.RowStart == 2)
                    {
                        thisCard.NeedsTop = true;
                    }
                    else if (firsts.RowStart == 5)
                    {
                        thisCard.NeedsBottom = true;
                    }
                }
            }
        }
        thisList = (from x in _saveRoot.BoardList
                    where x.Floor == 4
                    select x).ToBasicList();
        foreach (var firsts in thisList)
        {
            if (firsts.TileList.Count > 0)
            {
                firsts.TileList.First().NeedsLeft = true;
                firsts.TileList.Last().NeedsRight = true;
                foreach (var thisCard in firsts.TileList)
                {
                    if (firsts.RowStart == 3)
                    {
                        thisCard.NeedsTop = true;
                    }
                    else if (firsts.RowStart == 4)
                    {
                        thisCard.NeedsBottom = true;
                    }
                }
            }
        }
    }
}