namespace ConnectTheDots.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses
{
    private readonly ConnectTheDotsGameContainer _gameContainer;
    public GameBoardProcesses(ConnectTheDotsGameContainer gameContainer)
    {
        _gameContainer = gameContainer;
    }
    public void ClearBoard()
    {
        _gameContainer.PreviousLine = new LineInfo();
        _gameContainer.PreviousDot = new DotInfo(); //i think.
        if (_gameContainer.SquareList == null)
        {
            LoadGame(); //try this.
        }
        foreach (var thisSquare in _gameContainer.SquareList!.Values)
        {
            thisSquare.Color = 0;
            thisSquare.Player = 0;
        }
        foreach (var thisLine in _gameContainer.LineList!.Values)
        {
            thisLine.IsTaken = false;
        }
        foreach (var thisDot in _gameContainer.DotList!.Values)
        {
            thisDot.IsSelected = false;
        }
        _gameContainer.PlayerList!.ForEach(thisPlayer => thisPlayer.Score = 0);

        _gameContainer.RepaintBoard();
    }
    private LineInfo FindLine(int row, int column, bool horizontal)
    {
        return _gameContainer.LineList!.Values.Single(items => items.Row == row && items.Column == column && items.Horizontal == horizontal);
    }
    private bool IsGameOver => _gameContainer.SquareList!.Values.All(items => items.Player > 0);
    private int CalculateTotalPoints
    {
        get
        {
            int output = _gameContainer.SquareList!.Values.Count(items => items.Player == _gameContainer.SaveRoot!.PlayOrder.WhoTurn);
            if (output == 0)
            {
                throw new CustomBasicException("Must have at least one point");
            }
            return output;
        }
    }
    private void GetSavedDot(int row, int column)
    {
        _gameContainer.PreviousDot = _gameContainer.DotList!.Values.Single(items => items.Row == row && items.Column == column);
    }
    public void SaveGame()
    {
        SavedBoardData thisData = new();
        thisData.LineList = _gameContainer.LineList!.Values.Select(items => items.IsTaken).ToBasicList();
        thisData.DotList = _gameContainer.DotList!.Values.Select(items => items.IsSelected).ToBasicList();
        thisData.SquarePlayerList = _gameContainer.SquareList!.Values.Select(items => items.Player).ToBasicList();
        thisData.PreviousColumn = _gameContainer.PreviousDot.Column;
        thisData.PreviousRow = _gameContainer.PreviousDot.Row;
        thisData.PreviousLine = _gameContainer.PreviousLine.Index;
        _gameContainer.SaveRoot!.BoardData = thisData;
    }
    public void LoadGame()
    {
        if (_gameContainer.SaveRoot.BoardData == null)
        {
            _gameContainer.SaveRoot.BoardData = new(); //i think.
        }
        if (_gameContainer.DotList == null)
        {
            GameBoardGraphicsCP.InitBoard(_gameContainer);
        }
        int x = 0;
        _gameContainer.SaveRoot!.BoardData!.DotList.ForEach(thisDot =>
        {
            x++;
            _gameContainer.DotList![x].IsSelected = thisDot;
        });
        x = 0;
        _gameContainer.SaveRoot.BoardData.LineList.ForEach(thisLine =>
        {
            x++;
            _gameContainer.LineList![x].IsTaken = thisLine;
        });
        x = 0;
        _gameContainer.SaveRoot.BoardData.SquarePlayerList.ForEach(thisSquare =>
        {
            x++;
            _gameContainer.SquareList![x].Player = thisSquare;
            if (thisSquare > 0)
            {
                var tempPlayer = _gameContainer.PlayerList![thisSquare];
                _gameContainer.SquareList[x].Color = tempPlayer.Color.Value;
            }
        });
        if (_gameContainer.SaveRoot.BoardData.PreviousColumn > 0 && _gameContainer.SaveRoot.BoardData.PreviousRow > 0)
        {
            GetSavedDot(_gameContainer.SaveRoot.BoardData.PreviousRow, _gameContainer.SaveRoot.BoardData.PreviousColumn);
        }
        else
        {
            _gameContainer.PreviousDot = new DotInfo();
        }
        if (_gameContainer.SaveRoot.BoardData.PreviousLine > 0)
        {
            _gameContainer.PreviousLine = _gameContainer.LineList![_gameContainer.SaveRoot.BoardData.PreviousLine];
        }
        else
        {
            _gameContainer.PreviousLine = new LineInfo();
        }
    }
    private static bool WonSelectedList(BasicList<LineInfo> thisList)
    {
        return thisList.All(items => items.IsTaken == true);
    }
    private BasicList<LineInfo> GetLineList(SquareInfo thisSquare)
    {
        BasicList<LineInfo> output = new();
        var thisLine = FindLine(thisSquare.Row, thisSquare.Column, true);
        output.Add(thisLine);
        thisLine = FindLine(thisSquare.Row, thisSquare.Column, false);
        output.Add(thisLine);
        thisLine = FindLine(thisSquare.Row + 1, thisSquare.Column, true);
        output.Add(thisLine);
        thisLine = FindLine(thisSquare.Row, thisSquare.Column + 1, false);
        output.Add(thisLine);
        return output;
    }
    private bool DidWinSquare(int player, int color)
    {
        if (_gameContainer!.Test!.DoubleCheck == true)
        {
            return false;
        }
        BasicList<SquareInfo> winList = new();
        foreach (var thisSquare in _gameContainer.SquareList!.Values)
        {
            if (thisSquare.Player == 0)
            {
                var tempList = GetLineList(thisSquare);
                if (WonSelectedList(tempList))
                {
                    winList.Add(thisSquare);
                }
            }
        }
        if (winList.Count == 0)
        {
            return false;
        }
        winList.ForEach(thisSquare =>
        {
            thisSquare.Color = color;
            thisSquare.Player = player;
        });
        return true;
    }
    private static bool HasConnectedDot(DotInfo previousDot, DotInfo newDot)
    {
        if (previousDot.Equals(newDot))
        {
            return false;
        }
        if ((previousDot.Column == newDot.Column) & (previousDot.Row == newDot.Row))
        {
            return false;
        }
        if (((previousDot.Column + 1) == newDot.Column) & (previousDot.Row == newDot.Row))
        {
            return true;
        }
        if (((previousDot.Column - 1) == newDot.Column) & (previousDot.Row == newDot.Row))
        {
            return true;
        }
        if ((previousDot.Column == newDot.Column) & ((previousDot.Row + 1) == newDot.Row))
        {
            return true;
        }
        if ((previousDot.Column == newDot.Column) & ((previousDot.Row - 1) == newDot.Row))
        {
            return true;
        }
        return false;
    }
    private LineInfo GetConnectedLine(DotInfo previousDot, DotInfo newDot)
    {
        if (HasConnectedDot(previousDot, newDot) == false)
        {
            throw new CustomBasicException("There is no connected dots here");
        }
        foreach (var thisLine in _gameContainer.LineList!.Values)
        {
            if (thisLine.DotColumn1 == newDot.Column && thisLine.DotColumn2 == previousDot.Column && thisLine.DotRow1 == newDot.Row && thisLine.DotRow2 == previousDot.Row)
            {
                return thisLine;
            }
            if (thisLine.DotColumn2 == newDot.Column && thisLine.DotColumn1 == previousDot.Column && thisLine.DotRow2 == newDot.Row && thisLine.DotRow1 == previousDot.Row)
            {
                return thisLine;
            }
        }
        throw new CustomBasicException("Cannot find the connected dot.  Rethink");
    }
    public bool IsValidMove(int dot)
    {
        DotInfo thisDot = _gameContainer.DotList![dot];
        if (thisDot.Equals(_gameContainer.PreviousDot))
        {
            return true; //because undoing move.
        }
        if (_gameContainer.PreviousDot.Column == 0 && _gameContainer.PreviousDot.Row == 0)
        {
            return true; //because starting new move.
        }
        bool doesConnect = HasConnectedDot(_gameContainer.PreviousDot, thisDot);
        if (doesConnect == false)
        {
            return false;
        }
        LineInfo thisLine = GetConnectedLine(_gameContainer.PreviousDot, thisDot);
        return !thisLine.IsTaken;
    }
    public async Task MakeMoveAsync(int dot)
    {
        DotInfo thisDot = _gameContainer.DotList![dot];
        if (thisDot.Equals(_gameContainer.PreviousDot))
        {
            thisDot.IsSelected = false;
            _gameContainer.PreviousDot = new DotInfo();
            _gameContainer.RepaintBoard();
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (_gameContainer.PreviousDot.Column == 0 && _gameContainer.PreviousDot.Row == 0)
        {
            thisDot.IsSelected = true;
            _gameContainer.PreviousDot = thisDot;
            _gameContainer.RepaintBoard();
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        LineInfo thisLine = GetConnectedLine(_gameContainer.PreviousDot, thisDot);
        thisLine.IsTaken = true;
        _gameContainer.PreviousLine = thisLine;
        _gameContainer.PreviousDot.IsSelected = false;
        _gameContainer.PreviousDot = new DotInfo();
        bool wins = DidWinSquare(_gameContainer.SaveRoot!.PlayOrder.WhoTurn, _gameContainer.SingleInfo!.Color.Value);
        _gameContainer.RepaintBoard();
        if (wins == false)
        {
            await _gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        if (_gameContainer.Test.ImmediatelyEndGame)
        {
            await _gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        int totalPoints = CalculateTotalPoints;
        _gameContainer.SingleInfo.Score = totalPoints;
        if (IsGameOver)
        {
            _gameContainer.SingleInfo = _gameContainer.PlayerList!.OrderByDescending(items => items.Score).First();
            await _gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        await _gameContainer.ContinueTurnAsync!.Invoke(); //you get another turn for winning a square.
    }
}