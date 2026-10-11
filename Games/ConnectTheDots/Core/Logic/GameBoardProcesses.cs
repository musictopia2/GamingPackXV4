namespace ConnectTheDots.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses(ConnectTheDotsGameContainer gameContainer)
{
    public void ClearBoard()
    {
        gameContainer.PreviousLine = new LineInfo();
        gameContainer.PreviousDot = new DotInfo(); //i think.
        if (gameContainer.SquareList == null)
        {
            LoadGame(); //try this.
        }
        foreach (var thisSquare in gameContainer.SquareList!.Values)
        {
            thisSquare.Color = 0;
            thisSquare.Player = 0;
        }
        foreach (var thisLine in gameContainer.LineList!.Values)
        {
            thisLine.IsTaken = false;
        }
        foreach (var thisDot in gameContainer.DotList!.Values)
        {
            thisDot.IsSelected = false;
        }
        gameContainer.PlayerList!.ForEach(thisPlayer => thisPlayer.Score = 0);

        gameContainer.RepaintBoard();
    }
    private LineInfo FindLine(int row, int column, bool horizontal)
    {
        return gameContainer.LineList!.Values.Single(items => items.Row == row && items.Column == column && items.Horizontal == horizontal);
    }
    private bool IsGameOver => gameContainer.SquareList!.Values.All(items => items.Player > 0);
    private int CalculateTotalPoints
    {
        get
        {
            int output = gameContainer.SquareList!.Values.Count(items => items.Player == gameContainer.SaveRoot!.PlayOrder.WhoTurn);
            if (output == 0)
            {
                throw new CustomBasicException("Must have at least one point");
            }
            return output;
        }
    }
    private void GetSavedDot(int row, int column)
    {
        gameContainer.PreviousDot = gameContainer.DotList!.Values.Single(items => items.Row == row && items.Column == column);
    }
    public void SaveGame()
    {
        SavedBoardData thisData = new();
        thisData.LineList = gameContainer.LineList!.Values.Select(items => items.IsTaken).ToBasicList();
        thisData.DotList = gameContainer.DotList!.Values.Select(items => items.IsSelected).ToBasicList();
        thisData.SquarePlayerList = gameContainer.SquareList!.Values.Select(items => items.Player).ToBasicList();
        thisData.PreviousColumn = gameContainer.PreviousDot.Column;
        thisData.PreviousRow = gameContainer.PreviousDot.Row;
        thisData.PreviousLine = gameContainer.PreviousLine.Index;
        gameContainer.SaveRoot!.BoardData = thisData;
    }
    public void LoadGame()
    {
        if (gameContainer.SaveRoot.BoardData == null)
        {
            gameContainer.SaveRoot.BoardData = new(); //i think.
        }
        if (gameContainer.DotList == null)
        {
            GameBoardGraphicsCP.InitBoard(gameContainer);
        }
        int x = 0;
        gameContainer.SaveRoot!.BoardData!.DotList.ForEach(thisDot =>
        {
            x++;
            gameContainer.DotList![x].IsSelected = thisDot;
        });
        x = 0;
        gameContainer.SaveRoot.BoardData.LineList.ForEach(thisLine =>
        {
            x++;
            gameContainer.LineList![x].IsTaken = thisLine;
        });
        x = 0;
        gameContainer.SaveRoot.BoardData.SquarePlayerList.ForEach(thisSquare =>
        {
            x++;
            gameContainer.SquareList![x].Player = thisSquare;
            if (thisSquare > 0)
            {
                var tempPlayer = gameContainer.PlayerList![thisSquare];
                gameContainer.SquareList[x].Color = tempPlayer.Color.Value;
            }
        });
        if (gameContainer.SaveRoot.BoardData.PreviousColumn > 0 && gameContainer.SaveRoot.BoardData.PreviousRow > 0)
        {
            GetSavedDot(gameContainer.SaveRoot.BoardData.PreviousRow, gameContainer.SaveRoot.BoardData.PreviousColumn);
        }
        else
        {
            gameContainer.PreviousDot = new DotInfo();
        }
        if (gameContainer.SaveRoot.BoardData.PreviousLine > 0)
        {
            gameContainer.PreviousLine = gameContainer.LineList![gameContainer.SaveRoot.BoardData.PreviousLine];
        }
        else
        {
            gameContainer.PreviousLine = new LineInfo();
        }
    }
    private static bool WonSelectedList(BasicList<LineInfo> thisList)
    {
        return thisList.All(items => items.IsTaken == true);
    }
    private BasicList<LineInfo> GetLineList(SquareInfo thisSquare)
    {
        BasicList<LineInfo> output = [];
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
        if (gameContainer!.Test!.DoubleCheck == true)
        {
            return false;
        }
        BasicList<SquareInfo> winList = [];
        foreach (var thisSquare in gameContainer.SquareList!.Values)
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
        foreach (var thisLine in gameContainer.LineList!.Values)
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
        DotInfo thisDot = gameContainer.DotList![dot];
        if (thisDot.Equals(gameContainer.PreviousDot))
        {
            return true; //because undoing move.
        }
        if (gameContainer.PreviousDot.Column == 0 && gameContainer.PreviousDot.Row == 0)
        {
            return true; //because starting new move.
        }
        bool doesConnect = HasConnectedDot(gameContainer.PreviousDot, thisDot);
        if (doesConnect == false)
        {
            return false;
        }
        LineInfo thisLine = GetConnectedLine(gameContainer.PreviousDot, thisDot);
        return !thisLine.IsTaken;
    }
    public async Task MakeMoveAsync(int dot)
    {
        DotInfo thisDot = gameContainer.DotList![dot];
        if (thisDot.Equals(gameContainer.PreviousDot))
        {
            thisDot.IsSelected = false;
            gameContainer.PreviousDot = new DotInfo();
            gameContainer.RepaintBoard();
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        if (gameContainer.PreviousDot.Column == 0 && gameContainer.PreviousDot.Row == 0)
        {
            thisDot.IsSelected = true;
            gameContainer.PreviousDot = thisDot;
            gameContainer.RepaintBoard();
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        LineInfo thisLine = GetConnectedLine(gameContainer.PreviousDot, thisDot);
        thisLine.IsTaken = true;
        gameContainer.PreviousLine = thisLine;
        gameContainer.PreviousDot.IsSelected = false;
        gameContainer.PreviousDot = new DotInfo();
        bool wins = DidWinSquare(gameContainer.SaveRoot!.PlayOrder.WhoTurn, gameContainer.SingleInfo!.Color.Value);
        gameContainer.RepaintBoard();
        if (wins == false)
        {
            await gameContainer.EndTurnAsync!.Invoke();
            return;
        }
        if (gameContainer.Test.ImmediatelyEndGame)
        {
            await gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        int totalPoints = CalculateTotalPoints;
        gameContainer.SingleInfo.Score = totalPoints;
        if (IsGameOver)
        {
            gameContainer.SingleInfo = gameContainer.PlayerList!.OrderByDescending(items => items.Score).First();
            await gameContainer.ShowWinAsync!.Invoke();
            return;
        }
        await gameContainer.ContinueTurnAsync!.Invoke(); //you get another turn for winning a square.
    }
}