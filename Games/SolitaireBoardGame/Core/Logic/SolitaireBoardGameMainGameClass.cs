namespace SolitaireBoardGame.Core.Logic;
[SingletonGame]
public class SolitaireBoardGameMainGameClass : IAggregatorContainer
{
    public GameSpace PreviousSpace = new();
    private readonly ISolitaireBoardEvents _solitaireBoard;
    private readonly ISaveSinglePlayerClass _thisState;
    private readonly ISystemError _error;
    private readonly CommandContainer _command;
    private readonly IToast _toast;
    internal SolitaireBoardGameSaveInfo _saveRoot;
    public IGamePackageResolver MainContainer { get; set; }
    public SolitaireBoardGameMainGameClass(ISaveSinglePlayerClass thisState,
        IEventAggregator aggregator,
        IGamePackageResolver container,
        ISolitaireBoardEvents solitaireBoard,
        ISystemError error,
        CommandContainer command,
        IToast toast
        )
    {
        _thisState = thisState;
        Aggregator = aggregator;
        _solitaireBoard = solitaireBoard;
        _error = error;
        _command = command;
        _toast = toast;
        MainContainer = container;
        _saveRoot = container.ReplaceObject<SolitaireBoardGameSaveInfo>(); //can't create new one.  because if doing that, then anything that needs it won't have it.
        LoadBoard();
    }
    private void LoadBoard()
    {
        int x;
        int y;
        GameSpace thisSpace;
        for (x = 1; x <= 7; x++)
        {
            for (y = 1; y <= 7; y++)
            {
                if (x < 3 & y < 3 | x > 5 & y < 3 | x < 3 & y > 5 | y > 5 & x > 5)
                {
                }
                else
                {
                    thisSpace = new()
                    {
                        Vector = new Vector(x, y)
                    };
                    _saveRoot.SpaceList.Add(thisSpace);
                }
            }
        }
        if (_saveRoot.SpaceList.Any() == false)
        {
            throw new CustomBasicException("Can't have 0 items in the gameboard collection");
        }
    }
    public Vector PreviousPiece
    {
        get
        {
            return _saveRoot.PreviousPiece;
        }
        set
        {
            _saveRoot.PreviousPiece = value;
        }
    }
    public IEventAggregator Aggregator { get; }
    public async Task NewGameAsync()
    {
        PreviousPiece = new Vector();
        await ClearBoardAsync();
    }
    public async Task ClearBoardAsync()
    {
        if (await _thisState.CanOpenSavedSinglePlayerGameAsync() == false)
        {
            foreach (var space in _saveRoot.SpaceList)
            {
                if (space.Vector.Row == 4 && space.Vector.Column == 4)
                {
                    space.HasImage = false;
                }
                else
                {
                    space.HasImage = true;
                }
                space.ClearSpace();
            }
            PreviousSpace = new GameSpace();
        }
        else
        {
            _saveRoot = await _thisState.RetrieveSinglePlayerGameAsync<SolitaireBoardGameSaveInfo>();
            MainContainer.ReplaceObject(_saveRoot);
        }
        _command.UpdateAll();
    }
    public void SelectUnSelectSpace(GameSpace thisSpace)
    {
        foreach (GameSpace tempSpace in _saveRoot.SpaceList)
        {
            tempSpace.ClearSpace();
        }
        if (thisSpace == PreviousSpace)
        {
            PreviousSpace = new GameSpace();
        }
        else
        {
            PreviousSpace = thisSpace;
            PreviousSpace.Color = cs1.Yellow;
        }
    }
    public async Task ProcessCommandAsync(GameSpace thisSpace)
    {
        if (thisSpace.HasImage == false)
        {
            if (PreviousPiece.Column == 0 || PreviousPiece.Row == 0)
            {
                return; // because nothing selected
            }
            await _solitaireBoard.PiecePlacedAsync(thisSpace, this);
            return;
        }
        await _solitaireBoard.PieceSelectedAsync(thisSpace, this);
    }
    public async Task HightlightSpaceAsync(GameSpace thisSpace)
    {
        SelectUnSelectSpace(thisSpace);
        PreviousPiece = thisSpace.Vector;
        await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot);
    }
    private IEnumerable<GameSpace> MoveList(GameSpace thisSpace) // i think its of the type of gamespace well see
    {
        GameSpace previousSpace;
        previousSpace = _saveRoot.SpaceList[PreviousPiece];
        return MoveList(thisSpace, previousSpace);
    }
    private IEnumerable<GameSpace> MoveList(GameSpace thisSpace, GameSpace previousSpace)
    {
        IEnumerable<GameSpace> output;
        if (thisSpace.Vector.Column == previousSpace.Vector.Column)
        {
            if (thisSpace.Vector.Row > previousSpace.Vector.Row)
            {
                output = from Spaces in _saveRoot.SpaceList
                         where Spaces.Vector.Column == thisSpace.Vector.Column
                         & Spaces.Vector.Row > previousSpace.Vector.Row & Spaces.Vector.Row < thisSpace.Vector.Row
                         select Spaces;
                if (output.Any())
                {
                    output = from Moves in output
                             orderby Moves.Vector.Row
                             select Moves;
                }
            }
            else
            {
                output = from Spaces in _saveRoot.SpaceList
                         where Spaces.Vector.Column == thisSpace.Vector.Column &
                         Spaces.Vector.Row < previousSpace.Vector.Row & Spaces.Vector.Row > thisSpace.Vector.Row
                         select Spaces;
                if (output.Any())
                {
                    output = from Moves in output
                             orderby Moves.Vector.Row descending
                             select Moves;
                }
            }
            return output;
        }
        if (thisSpace.Vector.Column > previousSpace.Vector.Column)
        {
            output = from Spaces in _saveRoot.SpaceList
                     where Spaces.Vector.Row == thisSpace.Vector.Row &
                     Spaces.Vector.Column > previousSpace.Vector.Column & Spaces.Vector.Column < thisSpace.Vector.Column
                     select Spaces;
            if (output.Any())
            {
                output = from Moves in output
                         orderby Moves.Vector.Row
                         select Moves;
            }
            return output;
        }
        else
        {
            output = from Spaces in _saveRoot.SpaceList
                     where Spaces.Vector.Row == thisSpace.Vector.Row &
                     Spaces.Vector.Column < previousSpace.Vector.Column & Spaces.Vector.Column > thisSpace.Vector.Column
                     select Spaces;
            if (output.Any())
            {
                output = from Moves in output
                         orderby Moves.Vector.Row descending
                         select Moves;
            }
            return output;
        }
    }
    public bool IsValidMove(GameSpace thisSpace)
    {
        GameSpace previousSpace;
        previousSpace = _saveRoot.SpaceList[PreviousPiece];
        if (thisSpace.Vector.Column != previousSpace.Vector.Column &&
            thisSpace.Vector.Row != previousSpace.Vector.Row)
        {
            return false;
        }
        IEnumerable<GameSpace> thisCol;
        thisCol = MoveList(thisSpace, previousSpace);
        if (thisCol.Any() == false)
        {
            return false;
        }
        return !thisCol.Any(Items => Items.HasImage == false);
    }
    public async Task UnselectPieceAsync(GameSpace thisSpace)
    {
        if (PreviousPiece.Equals(thisSpace))
        {
            PreviousPiece = new Vector();
        }
        SelectUnSelectSpace(thisSpace);
        await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot);
    }
    private int PiecesRemaining()
    {
        return _saveRoot.SpaceList.Count(Items => Items.HasImage == true);
    }
    public async Task MakeMoveAsync(GameSpace thisSpace)
    {
        var thisList = MoveList(thisSpace);
        foreach (var item in thisList)
        {
            item.HasImage = false; //for now.
        }
        var nextSpace = _saveRoot.SpaceList[PreviousPiece];
        nextSpace.HasImage = false;
        GameSpace tempPiece = new();
        tempPiece.Color = cs1.Blue;
        tempPiece.HasImage = true;
        await Aggregator.AnimateMovePiecesAsync(PreviousPiece, thisSpace.Vector, tempPiece);
        thisSpace.HasImage = true;
        PreviousPiece = new(); //i think
        int manys = PiecesRemaining();
        if (manys == 1)
        {
            await _thisState.DeleteSinglePlayerGameAsync();
            await ShowWinAsync();
            return;
        }
        await _thisState.SaveSimpleSinglePlayerGameAsync(_saveRoot);
    }
    public async Task ShowWinAsync()
    {
        _toast.ShowSuccessToast("You Win");
        await _thisState.DeleteSinglePlayerGameAsync();
        await this.SendGameOverAsync(_error);
    }
}