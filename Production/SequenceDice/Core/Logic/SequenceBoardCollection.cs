namespace SequenceDice.Core.Logic;
public class SequenceBoardCollection : IBoardCollection<SpaceInfoCP>
{
    public SequenceBoardCollection()
    {
        _privateBoard = new BoardCollection<SpaceInfoCP>(6, 6);
    }
    public SequenceBoardCollection(IEnumerable<SpaceInfoCP> PreviousList)
    {
        _privateBoard = new BoardCollection<SpaceInfoCP>(PreviousList);
        _autoResume = true;
    }
    private bool _didInit;
    private readonly bool _autoResume;
    private BasicList<BasicList<SpaceInfoCP>>? _winList;
    private TestOptions? _thisTest;
    private SequenceDiceVMData? _model;
    public void LoadBoard(PlayerCollection<SequenceDicePlayerItem> playerList, TestOptions thisTest, SequenceDiceVMData model)
    {
        _model = model;
        if (_didInit == true)
        {
            return;
        }
        if (playerList.Count == 2)
        {
            _winList = _privateBoard.GetPossibleCombinations(6);
        }
        else if (playerList.Count == 3)
        {
            _winList = _privateBoard.GetPossibleCombinations(5);
        }
        else
        {
            throw new CustomBasicException("Should have been 2 or 3 players.  Rethink");
        }
        _privateBoard.MainObjectSelector = Items => Items.Player;
        _didInit = true;
        _thisTest = thisTest;
        if (_autoResume == true)
        {
            return;
        }
        _privateBoard.ForEach(thisSpace =>
        {
            //populate the numbers.  that is needed for lots of things.
            if (thisSpace.Vector.Row == 1 & thisSpace.Vector.Column == 1 |
                thisSpace.Vector.Row == 6 & thisSpace.Vector.Column == 6 |
                thisSpace.Vector.Row == 1 & thisSpace.Vector.Column == 6 |
                thisSpace.Vector.Row == 6 & thisSpace.Vector.Column == 1)
            {
                thisSpace.Number = 2;
            }
            else if (((thisSpace.Vector.Row == 5) & (thisSpace.Vector.Column == 1)) |
                ((thisSpace.Vector.Row == 1) & (thisSpace.Vector.Column == 2)) |
                ((thisSpace.Vector.Row == 2) & (thisSpace.Vector.Column == 6)) |
                ((thisSpace.Vector.Row == 6) & (thisSpace.Vector.Column == 5)))
            {
                thisSpace.Number = 3;
            }
            else if (((thisSpace.Vector.Row == 4) & (thisSpace.Vector.Column == 1)) |
                ((thisSpace.Vector.Row == 1) & (thisSpace.Vector.Column == 3)) |
                ((thisSpace.Vector.Row == 3) & (thisSpace.Vector.Column == 6)) |
                ((thisSpace.Vector.Row == 6) & (thisSpace.Vector.Column == 4)))
            {
                thisSpace.Number = 4;
            }
            else if (((thisSpace.Vector.Row == 3) & (thisSpace.Vector.Column == 1)) |
                ((thisSpace.Vector.Row == 1) & (thisSpace.Vector.Column == 4)) |
                ((thisSpace.Vector.Row == 4) & (thisSpace.Vector.Column == 6)) |
                ((thisSpace.Vector.Row == 6) & (thisSpace.Vector.Column == 3)))
            {
                thisSpace.Number = 5;
            }
            else if (((thisSpace.Vector.Row == 2) & (thisSpace.Vector.Column == 1)) |
                ((thisSpace.Vector.Row == 1) & (thisSpace.Vector.Column == 5)) |
                ((thisSpace.Vector.Row == 5) & (thisSpace.Vector.Column == 6)) |
                ((thisSpace.Vector.Row == 6) & (thisSpace.Vector.Column == 2)))
            {
                thisSpace.Number = 6;
            }
            else if (((thisSpace.Vector.Row == 5) & (thisSpace.Vector.Column == 2)) |
                ((thisSpace.Vector.Row == 2) & (thisSpace.Vector.Column == 2)) |
                ((thisSpace.Vector.Row == 5) & (thisSpace.Vector.Column == 5)) |
                ((thisSpace.Vector.Row == 2) & (thisSpace.Vector.Column == 5)))
            {
                thisSpace.Number = 7;
            }
            else if (((thisSpace.Vector.Row == 4) & (thisSpace.Vector.Column == 2)) |
                ((thisSpace.Vector.Row == 2) & (thisSpace.Vector.Column == 3)) |
                ((thisSpace.Vector.Row == 5) & (thisSpace.Vector.Column == 4)) |
                ((thisSpace.Vector.Row == 3) & (thisSpace.Vector.Column == 5)))
            {
                thisSpace.Number = 8;
            }
            else if (((thisSpace.Vector.Row == 3) & (thisSpace.Vector.Column == 2)) |
                ((thisSpace.Vector.Row == 2) & (thisSpace.Vector.Column == 4)) |
                ((thisSpace.Vector.Row == 5) & (thisSpace.Vector.Column == 3)) |
                ((thisSpace.Vector.Row == 4) & (thisSpace.Vector.Column == 5)))
            {
                thisSpace.Number = 9;
            }
            else if (((thisSpace.Vector.Row == 4) & (thisSpace.Vector.Column == 3)) |
                ((thisSpace.Vector.Row == 3) & (thisSpace.Vector.Column == 3)) |
                ((thisSpace.Vector.Row == 3) & (thisSpace.Vector.Column == 4)) |
                ((thisSpace.Vector.Row == 4) & (thisSpace.Vector.Column == 4)))
            {
                thisSpace.Number = 12;
            }
            else
            {
                throw new CustomBasicException($"Cannnot find a space for {thisSpace.Vector.Row} row, {thisSpace.Vector.Column} column");
            }
        });
    }
    public bool HasWon()
    {
        var tempList = _privateBoard.GetWinCombo(_winList!);
        return tempList.Count != 0;
    }
    private void ClearRecent(SpaceInfoCP thisSpace)
    {
        foreach (var otherSpace in _privateBoard)
        {
            otherSpace.WasRecent = otherSpace.Equals(thisSpace);
        }
    }
    int _whoTurn;
    public void StartTurn(int whoTurn)
    {
        _whoTurn = whoTurn;
    }
    public void MakeMove(SpaceInfoCP thisSpace, SequenceDicePlayerItem singleInfo)
    {
        if (singleInfo.Id != _whoTurn)
        {
            throw new CustomBasicException("Can't make move because you never started turn");
        }
        ClearRecent(thisSpace);
        if (_model!.Cup!.TotalDiceValue == 10)
        {
            thisSpace.Player = 0;
            thisSpace.Color = cs1.Transparent;
            return;
        }
        thisSpace.Player = singleInfo.Id;
        thisSpace.Color = singleInfo.Color.Color;
    }
    public bool HasValidMove()
    {
        int Totals = _model!.Cup!.TotalDiceValue;
        if (_whoTurn == 0)
        {
            throw new CustomBasicException("Cannot be 0 for whoturn.  Find out what happened");
        }
        if (_thisTest!.AllowAnyMove == true || Totals == 11)
        {
            return true;
        }
        if (Totals == 10)
        {
            return _privateBoard.Any(Items => Items.Player > 0 && Items.Player != _whoTurn && Items.Number != 2 && Items.Number != 12);
        }
        else
        {
            return _privateBoard.Any(Items => Items.Number == Totals && Items.Player != _whoTurn);
        }
    }
    public bool CanMakeMove(SpaceInfoCP thisSpace)
    {
        if (_thisTest!.AllowAnyMove == true)
        {
            return true; //needs to allow any move so we can test different situations easily.
        }
        int Totals = _model!.Cup!.TotalDiceValue;
        if (Totals == 11)
        {
            if (thisSpace.Player == 0)
            {
                return true;
            }
            return !_privateBoard.Any(Items => Items.Player == 0);
        }
        if (Totals == 10)
        {
            if (thisSpace.Number == 2 || thisSpace.Number == 12)
            {
                return false;
            }
            if (thisSpace.Player == 0)
            {
                return false;
            }
            return thisSpace.Player != _whoTurn;
        }
        if (thisSpace.Number != Totals)
        {
            return false;
        }
        if (thisSpace.Player == 0)
        {
            return true;
        }
        if (thisSpace.Player == _whoTurn)
        {
            return false;
        }
        return !_privateBoard.Any(Items => Items.Player == 0 && Items.Number == Totals);
    }
    public void ClearBoard()
    {
        _privateBoard.Clear();
    }
    private readonly BoardCollection<SpaceInfoCP> _privateBoard;
    public SpaceInfoCP this[Vector thisV] => _privateBoard[thisV];
    public SpaceInfoCP this[int row, int column] => _privateBoard[row, column];
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