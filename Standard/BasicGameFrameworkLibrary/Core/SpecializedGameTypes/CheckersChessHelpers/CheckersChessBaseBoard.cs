namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.CheckersChessHelpers;
public abstract class CheckersChessBaseBoard<E, S>
    where E : IFastEnumColorSimple
    where S : CheckersChessSpace<E>, new()
{
    public static SizeF OriginalSize { get; set; } = new(500, 500);
    public static BasicList<S> PrivateSpaceList { get; set; } = new(); // looks like blazor needs to reference this as well.
    private static string _firstColor = "";
    private static string _secondColor = "";
    public static bool HasGreen { get; set; }
    public CheckersChessBaseBoard()
    {
        PrivateSpaceList.Clear();
        _thisGame = GetGame();
        SetUpColors();
        CreateSpaces();
    }
    private void SetUpColors()
    {
        if (_thisGame == EnumCheckerChessGame.Checkers)
        {
            _firstColor = cs.White;
            _secondColor = cs.Black;
            HasGreen = false;
        }
        else
        {
            _firstColor = cs.WhiteSmoke;
            _secondColor = cs.Tan;
            HasGreen = true;
        }
    }
    public PointF SuggestedOffLocation(bool isReversed)
    {
        if (isReversed == true)
        {
            return new(0, 0);
        }
        return new(0, OriginalSize.Height);
    }
    public static int GetIndexByPoint(int row, int column)
    {
        int x;
        int y;
        int z = 0;
        for (x = 1; x <= 8; x++)
        {
            for (y = 1; y <= 8; y++)
            {
                z += 1;
                if (x == row && y == column)
                {
                    return z;
                }
            }
        }
        throw new CustomBasicException("Nothing found");
    }
    public static int GetRealIndex(int originalIndex, bool isReversed)
    {
        if (originalIndex == 0)
        {
            return 0;
        }
        if (isReversed == false)
        {
            return originalIndex;
        }
        var thisSpace = (from xx in PrivateSpaceList
                         where xx.MainIndex == originalIndex
                         select xx).Single();
        return thisSpace.ReversedIndex;
    }
    public static S GetSpace(int originalIndex, bool isReversed)
    {
        if (isReversed == false)
        {
            return (from xx in PrivateSpaceList
                    where xx.MainIndex == originalIndex
                    select xx).Single();
        }
        return (from xx in PrivateSpaceList
                where xx.ReversedIndex == originalIndex
                select xx).Single();
    }
    public static S? GetSpace(int row, int column)
    {
        if (row < 1 || row > 8)
        {
            return null;
        }
        if (column < 1 || column > 8)
        {
            return null;
        }
        return (from xx in PrivateSpaceList
                where xx.Row == row && xx.Column == column
                select xx).Single();
    }
    protected RectangleF GetBounds = new(0, 0, 500, 500);
    public static int LongestSize => 500 / 8;
    private void CreateSpaces()
    {
        int x;
        int y;
        string rowPaint;
        string thisPaint;
        float locX;
        float locY;
        var thisBounds = GetBounds;
        var diffs = thisBounds.Width / 8;
        locY = 0;
        rowPaint = _firstColor!;
        for (x = 1; x <= 8; x++)
        {
            thisPaint = rowPaint!;
            locX = 0;
            for (y = 1; y <= 8; y++)
            {
                S thisSpace = new();
                thisSpace.ThisRect = new(locX, locY, diffs, diffs);
                thisSpace.Column = y;
                thisSpace.Row = x;
                thisSpace.MainIndex = GetIndexByPoint(x, y);
                thisSpace.ReversedIndex = GetIndexByPoint(9 - x, 9 - y);
                thisSpace.Color = thisPaint;
                if (thisPaint!.Equals(_firstColor) == true)
                {
                    thisPaint = _secondColor!;
                }
                else
                {
                    thisPaint = _firstColor!;
                }
                PrivateSpaceList.Add(thisSpace);
                locX += diffs;
            }
            if (rowPaint!.Equals(_firstColor) == true)
            {
                rowPaint = _secondColor!;
            }
            else
            {
                rowPaint = _firstColor!;
            }
            locY += diffs;
        }
    }
    public static BasicList<int> GetBlackStartingSpaces()
    {
        BasicList<int> newList = new();
        for (var x = 6; x <= 8; x++)
        {
            for (var y = 1; y <= 8; y++)
            {
                var thisSpace = GetSpace(x, y);
                if (thisSpace!.Color!.Equals(_secondColor) == true)
                {
                    newList.Add(thisSpace.MainIndex);
                }
            }
        }
        return newList;
    }
    public abstract bool CanHighlight(); // so if you can't won't even both doing that part.
    private readonly EnumCheckerChessGame _thisGame;
    public abstract EnumCheckerChessGame GetGame();
    public static int SpaceSelected { get; set; } // this is used so it can do something different.
    public void ClearBoard()
    {
        foreach (var thisSpace in PrivateSpaceList)
        {
            thisSpace.ClearSpace();
        }
        AfterClearBoard();
    }
    protected abstract void AfterClearBoard();
}