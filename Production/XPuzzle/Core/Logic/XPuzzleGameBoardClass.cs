namespace XPuzzle.Core.Logic;
[InstanceGame] //i think its best to do as instance.
public class XPuzzleGameBoardClass : IAdvancedDIContainer, IAggregatorContainer
{
    private XPuzzleSaveInfo _games;
    private readonly IRandomGenerator _rs;
    private readonly ISaveSinglePlayerClass _thisSave;
    private readonly IEventAggregator _aggregator;
    private readonly ISystemError _error;
    public IGamePackageResolver? MainContainer { get; set; }
    public XPuzzleGameBoardClass(IGamePackageResolver container,
        XPuzzleSaveInfo saveroot,
        IRandomGenerator random,
        ISaveSinglePlayerClass saves,
        IEventAggregator aggregator,
        ISystemError error
        )
    {
        MainContainer = container;
        _games = saveroot;
        _rs = random;
        _thisSave = saves;
        _aggregator = aggregator;
        _error = error;
        _games.SpaceList.SetContainer(container);
    }
    private Vector PreviousOpen
    {
        get
        {
            return _games.PreviousOpen;
        }
        set
        {
            _games.PreviousOpen = value;
        }
    }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }
    IEventAggregator IAggregatorContainer.Aggregator => _aggregator;
    public async Task NewGameAsync()
    {
        if (await _thisSave.CanOpenSavedSinglePlayerGameAsync() == false)
        {
            _games.SpaceList.ClearBoard();
            PreviousOpen = new Vector(3, 3);
            BasicList<int> thisList = _rs.GenerateRandomList(8);
            _games.SpaceList.PopulateBoard(thisList);
        }
        else
        {
            _games = await _thisSave.RetrieveSinglePlayerGameAsync<XPuzzleSaveInfo>();
            MainContainer!.ReplaceObject(_games);
        }
        await _thisSave.SaveSimpleSinglePlayerGameAsync(_games);
    }
    public async Task MakeMoveAsync(int row, int column)
    {
        XPuzzleSpaceInfo thisSpace = _games.SpaceList[row, column];
        await MakeMoveAsync(thisSpace);
    }
    public async Task MakeMoveAsync(XPuzzleSpaceInfo thisSpace)
    {
        if (IsValidMove(thisSpace) == false)
        {
            return;
        }
        XPuzzleSpaceInfo previousSpace;
        previousSpace = _games.SpaceList[PreviousOpen];
        previousSpace.Color = cs1.Navy;
        thisSpace.Color = cs1.Black;
        previousSpace.Text = thisSpace.Text;
        thisSpace.Text = "";
        PreviousOpen = thisSpace.Vector;
        if (HasWon() == true)
        {
            await _thisSave.DeleteSinglePlayerGameAsync();
            //let this do game over too now.
            await this.SendGameOverAsync(_error);
            return;
        }
        await _thisSave.SaveSimpleSinglePlayerGameAsync(_games);
    }
    public bool IsValidMove(int row, int column)
    {
        XPuzzleSpaceInfo thisSpace = _games.SpaceList[row, column];
        return IsValidMove(thisSpace);
    }
    public bool IsValidMove(XPuzzleSpaceInfo thisSpace)
    {
        if (thisSpace.Vector.Column == PreviousOpen.Column &&
            thisSpace.Vector.Row == PreviousOpen.Row)
        {
            return false;
        }
        XPuzzleSpaceInfo newSpace;
        newSpace = _games.SpaceList[PreviousOpen];
        if (thisSpace.Vector.Column + 1 == newSpace.Vector.Column)
        {
            if (thisSpace.Vector.Row == newSpace.Vector.Row)
            {
                return true;
            }
        }
        if (thisSpace.Vector.Column - 1 == newSpace.Vector.Column)
        {
            if (thisSpace.Vector.Row == newSpace.Vector.Row)
            {
                return true;
            }
        }
        if (thisSpace.Vector.Row + 1 == newSpace.Vector.Row)
        {
            if (thisSpace.Vector.Column == newSpace.Vector.Column)
            {
                return true;
            }
        }
        if (thisSpace.Vector.Row - 1 == newSpace.Vector.Row)
        {
            if (thisSpace.Vector.Column == newSpace.Vector.Column)
            {
                return true;
            }
        }
        return false;
    }
    public bool HasWon()
    {
        if (PreviousOpen.Column != 3 && PreviousOpen.Row != 3)
        {
            return false;
        }
        int x = 0;
        foreach (var thisSpace in _games.SpaceList)
        {
            x += 1;
            if (x < 9)
            {
                if (thisSpace.Text == "")
                {
                    return false;
                }
                if (int.Parse(thisSpace.Text) != x)
                {
                    return false;
                }
            }
        }
        return true;
    }
}