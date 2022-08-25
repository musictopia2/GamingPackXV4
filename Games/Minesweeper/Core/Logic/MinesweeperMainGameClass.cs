namespace Minesweeper.Core.Logic;
[SingletonGame]
public class MinesweeperMainGameClass : IAggregatorContainer
{
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly IMessageBox _message;
    private readonly ISystemError _error;
    private readonly IToast _toast;
    private readonly IRandomGenerator _rs;
    private readonly CommandContainer _command;
    public int NumberOfColumns = 9;
    public int NumberOfRows = 9;
    public int NumberOfMines = 10;
    private BasicList<MineSquareModel> _arr_Squares = new();
    public MinesweeperMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IMessageBox message,
        ISystemError error,
        IToast toast,
        IRandomGenerator rs,
        CommandContainer command
        )
    {
        _thisState = thisState;
        Aggregator = aggregator;
        _message = message;
        _error = error;
        _toast = toast;
        _rs = rs;
        _command = command;
    }
    public BasicList<MineSquareModel> GetSquares()
    {
        return _arr_Squares;
    }
    public IEventAggregator Aggregator { get; }
    public async Task NewGameAsync()
    {
        CreateSquares();
        _command.UpdateAll();
        await ProcessGameStateAsync(EnumGameStates.NotFinished);
    }
    private async Task ProcessGameStateAsync(EnumGameStates state)
    {
        if (state == EnumGameStates.NotFinished)
        {
            return;
        }
        if (state == EnumGameStates.Won)
        {
            _command.UpdateAll(); //has to update in this case.
            await this.MessageGameOverAsync("Congratulations, you won the game!", _toast, _error);
            return;
        }
        if (state == EnumGameStates.Lost)
        {
            _command.UpdateAll();
            await this.MessageGameOverAsync("Sorry, you lost the game!", _toast, _error);
            return;
        }
        throw new CustomBasicException("Rethink");
    }
    public static void FlagSingleSquare(MineSquareModel thisSquare)
    {
        if (thisSquare.IsFlipped == true)
        {
            return;
        }
        thisSquare.Flagged = !thisSquare.Flagged;
    }
    public async Task ClickSingleSquareAsync(MineSquareModel thisSquare)
    {
        if (thisSquare.IsFlipped == true || thisSquare.Flagged == true)
        {
            return;
        }
        if (thisSquare.IsMine == true)
        {
            BlowUp();
            await ProcessGameStateAsync(EnumGameStates.Lost);
            return; // you lost now.
        }
        thisSquare.IsFlipped = true;
        if (thisSquare.NeighborMines == 0)
            FlipAllNeighbors(thisSquare);
        if (NumberOfMines == CountUnflippedSquares())
        {
            await ProcessGameStateAsync(EnumGameStates.Won);
            return;
        }
    }
    public int GetMinesLeft()
    {
        int int_Return = NumberOfMines;
        foreach (MineSquareModel obj_Square in _arr_Squares)
        {
            if (obj_Square.Flagged)
            {
                int_Return -= 1;
            }
        }
        return int_Return;
    }
    private int CountUnflippedSquares()
    {
        int int_Return = 0;
        foreach (MineSquareModel obj_Square in _arr_Squares)
        {
            if (!obj_Square.IsFlipped)
            {
                int_Return += 1;
            }
        }
        return int_Return;
    }
    private int GetRandomInteger(int int_Span)
    {
        int int_RandomInteger;
        if (int_Span <= 0)
        {
            int_RandomInteger = 0;
        }
        else
        {
            int_RandomInteger = _rs.GetRandomNumber(int_Span, 0);
        }
        return int_RandomInteger;
    }
    private MineSquareModel GetSquare(int column, int row)
    {
        return (from xx in _arr_Squares
                where xx.Column == column && xx.Row == row
                select xx).SingleOrDefault()!;
    }
    private bool SquareHasMine(int column, int row)
    {
        var obj_CheckSquare = GetSquare(column, row);
        if (!(obj_CheckSquare == null))
        {
            if (obj_CheckSquare.IsMine)
            {
                return true;
            }
        }
        return false;
    }
    private void BlowUp()
    {
        foreach (MineSquareModel obj_Square in _arr_Squares)
        {
            if (obj_Square.IsMine)
            {
                obj_Square.IsFlipped = true;
            }
            else if ((!obj_Square.IsMine) & (obj_Square.Flagged))
            {
                obj_Square.IsFlipped = true;
            }
        }
    }
    private void FlipAllNeighbors(MineSquareModel obj_TempSquare)
    {
        MineSquareModel obj_Flip;
        for (var int_X = -1; int_X <= 1; int_X++)
        {
            for (var int_Y = -1; int_Y <= 1; int_Y++)
            {
                obj_Flip = GetSquare(obj_TempSquare.Column + int_X, obj_TempSquare.Row + int_Y);
                if (!(obj_Flip == null))
                {
                    if ((!obj_Flip.Flagged) & (!obj_Flip.IsFlipped))
                    {
                        obj_Flip.IsFlipped = true;
                        if (obj_Flip.NeighborMines == 0)
                        {
                            FlipAllNeighbors(obj_Flip);
                        }
                    }
                }
            }
        }
    }
    private int CountNeighborMines(MineSquareModel obj_TempSquare)
    {
        int int_Count = 0;
        for (var int_X = -1; int_X <= 1; int_X++)
        {
            for (var int_Y = -1; int_Y <= 1; int_Y++)
            {
                if (SquareHasMine(obj_TempSquare.Column + int_X, obj_TempSquare.Row + int_Y))
                {
                    int_Count += 1;
                }
            }
        }
        return int_Count;
    }
    private void CreateSquares()
    {
        BasicList<int> arr_Mines = new();
        int int_Square;
        int int_SquareCount = 1;
        _arr_Squares = new();
        while (arr_Mines.Count < NumberOfMines)
        {
            int_Square = GetRandomInteger((NumberOfRows * NumberOfColumns));
            if ((!arr_Mines.Contains(int_Square)) & int_Square > 0)
            {
                arr_Mines.Add(int_Square);
            }
        }
        var loopTo = NumberOfRows;
        for (int int_Row = 1; int_Row <= loopTo; int_Row++)
        {
            var loopTo1 = NumberOfColumns;
            for (int int_Col = 1; int_Col <= loopTo1; int_Col++)
            {
                var obj_TempSquare = new MineSquareModel(int_Col, int_Row);
                if (arr_Mines.Contains(int_SquareCount))
                {
                    obj_TempSquare.IsMine = true;
                }
                _arr_Squares.Add(obj_TempSquare);
                int_SquareCount += 1;
            }
        }
        // *** Get numbers for each of the non-mine squares
        foreach (var obj_TempSquare in _arr_Squares)
        {
            obj_TempSquare.NeighborMines = CountNeighborMines(obj_TempSquare);
        }
    }
    public async Task ShowWinAsync()
    {
        _command.UpdateAll();
        await _message.ShowMessageAsync("You Win");
        await _thisState.DeleteSinglePlayerGameAsync();
        await this.SendGameOverAsync(_error);
    }
}