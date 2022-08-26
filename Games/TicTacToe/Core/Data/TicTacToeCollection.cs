

namespace TicTacToe.Core.Data;
public class TicTacToeCollection : IEnumerable<SpaceInfoCP>, IBoardCollection<SpaceInfoCP>
{
    private readonly BoardCollection<SpaceInfoCP> _privateBoard;
    private BasicList<BasicList<SpaceInfoCP>>? _winList;
    public TicTacToeCollection()
    {
        _privateBoard = new BoardCollection<SpaceInfoCP>(3, 3);
        FinishInit();
    }
    private void FinishInit()
    {
        _winList = _privateBoard.GetPossibleCombinations(3);
        _privateBoard.MainObjectSelector = items => items.Status;
        _privateBoard.BoardResultSelector = items =>
        {
            if (items.Status == EnumSpaceType.O)
            {
                return "O".Single();
            }
            return "x".Single();
        };
    }
    public TicTacToeCollection(IEnumerable<SpaceInfoCP> previousList)
    {
        _privateBoard = new BoardCollection<SpaceInfoCP>(previousList);
        FinishInit();
    }
    public WinInfo GetWin()
    {
        WinInfo output = new();
        output.WinList = _privateBoard.GetWinCombo(_winList!);
        output.IsDraw = _privateBoard.IsAllFilled();
        if (output.WinList.Count > 0)
        {
            if (output.WinList.Count != 3)
            {
                throw new CustomBasicException("Must have 3 to win");
            }
            Vector FirstSpace = output.WinList.First().Vector;
            Vector SecondSpace = output.WinList[1].Vector;
            if (FirstSpace.Column == SecondSpace.Column)
            {
                output.Category = EnumWinCategory.TopDown;
            }
            else if (FirstSpace.Row == SecondSpace.Row)
            {
                output.Category = EnumWinCategory.LeftRight;
            }
            else if (SecondSpace.Column > FirstSpace.Column)
            {
                output.Category = EnumWinCategory.TopLeft;
            }
            else
            {
                output.Category = EnumWinCategory.TopRight;
            }
        }
        return output;
    }
    public BasicList<SpaceInfoCP> GetAlmostWinList()
    {
        return _privateBoard.GetAlmostWinList(_winList!);
    }
    public void Clear()
    {
        _privateBoard.Clear();
    }
    public SpaceInfoCP this[int row, int column]
    {
        get
        {
            return _privateBoard[row, column];
        }
    }
    public SpaceInfoCP this[Vector thisV] //when you click, it has to be a vector.
    {
        get
        {
            return _privateBoard[thisV];
        }
    }
    public bool IsFilled(Vector thisV)
    {
        return _privateBoard.IsFilled(thisV);
    }
    public bool IsFilled(int row, int column)
    {
        return _privateBoard.IsFilled(row, column);
    }
    public IEnumerator<SpaceInfoCP> GetEnumerator()
    {
        return _privateBoard.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _privateBoard.GetEnumerator();
    }
    public int GetTotalColumns()
    {
        return _privateBoard.GetTotalColumns();
    }
    public int GetTotalRows()
    {
        return _privateBoard.GetTotalRows();
    }
}