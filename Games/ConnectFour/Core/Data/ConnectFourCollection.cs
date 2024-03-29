﻿namespace ConnectFour.Core.Data;
public class ConnectFourCollection : IBoardCollection<SpaceInfoCP>
{
    private readonly BoardCollection<SpaceInfoCP> _privateBoard;
    private BasicList<BasicList<SpaceInfoCP>>? _winList;
    public SpaceInfoCP this[Vector thisV] => _privateBoard[thisV];
    public SpaceInfoCP this[int row, int column] => _privateBoard[row, column];
    public ConnectFourCollection()
    {
        _privateBoard = new BoardCollection<SpaceInfoCP>(6, 7);
        FinishInit();
    }
    public WinInfo GetWin()
    {
        WinInfo output = new();
        output.WinList = _privateBoard.GetWinCombo(_winList!);
        output.IsDraw = _privateBoard.IsAllFilled();
        return output;
    }
    public void Clear()
    {
        _privateBoard.Clear();
    }
    public bool IsFilled(int column)
    {
        BasicList<SpaceInfoCP> thisList = _privateBoard.GetAllRows(column);
        return (thisList.All(items => items.IsFilled() == true));
    }
    public Vector GetBottomSpace(int column)
    {
        if (column == 0)
        {
            throw new CustomBasicException("Column cannot be 0");
        }
        BasicList<SpaceInfoCP> thisList = _privateBoard.GetAllRows(column);
        return thisList.Where(items => items.HasImage == false).OrderByDescending(items => items.Vector.Row).Take(1).Single().Vector;
    }
    private void FinishInit()
    {
        _winList = _privateBoard.GetPossibleCombinations(4);
        _privateBoard.MainObjectSelector = items => items.Player;
    }
    public ConnectFourCollection(IEnumerable<SpaceInfoCP> previousList)
    {
        _privateBoard = new BoardCollection<SpaceInfoCP>(previousList);
        FinishInit();
    }
    public IEnumerator<SpaceInfoCP> GetEnumerator()
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
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _privateBoard.GetEnumerator();
    }
}