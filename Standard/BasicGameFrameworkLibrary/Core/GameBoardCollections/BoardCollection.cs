namespace BasicGameFrameworkLibrary.Core.GameBoardCollections;

public class BoardCollection<C> : IEnumerable<C>, IAdvancedDIContainer, IBoardCollection<C> where C : class, IBasicSpace, new()
{
    private readonly Dictionary<Vector, C> _privateDict = new();
    public IGamePackageResolver? MainContainer { get; set; }
    public IGamePackageGeneratorDI? GeneratorContainer { get; set; }

    public Func<C, object>? MainObjectSelector;
    public Func<C, char>? BoardResultSelector;
    public BoardCollection() { }
    public BoardCollection(IEnumerable<C> previousBoard)
    {
        _privateDict = previousBoard.ToDictionary(Items => Items.Vector);
        _howManyColumns = _privateDict.Values.Max(Items => Items.Vector.Column);
        _howManyRows = _privateDict.Values.Max(Items => Items.Vector.Row);
    }
    public BoardCollection(int rows, int columns)
    {
        SetDimensions(rows, columns);
    }
    public int GetTotalRows()
    {
        return _howManyRows;
    }
    public int GetTotalColumns()
    {
        return _howManyColumns;
    }
    private static Vector GetVector(int row, int column)
    {
        Vector thisV = new(row, column);
        return thisV;
    }
    public void ForEach(Action<C> action)
    {
        foreach (var item in _privateDict.Values)
        {
            action.Invoke(item);
        }
    }
    public void PerformActionOnConditional(Predicate<C> predicate, Action<C> action)
    {
        foreach (var item in _privateDict.Values)
        {
            if (predicate(item) == true)
            {
                action.Invoke(item);
            }
        }
    }
    public async Task ForEachAsync(ActionAsync<C> action)
    {
        foreach (C thisItem in _privateDict.Values)
        {
            await action.Invoke(thisItem);
        }
    }
    public BasicList<BasicList<C>> GetPossibleCombinations(int howManyNeeded)
    {
        if (howManyNeeded <= 0 || howManyNeeded > _howManyColumns || howManyNeeded > _howManyRows)
        {
            throw new CustomArgumentException(nameof(howManyNeeded), "The number needed can't be less than one or more than rows and columns you have");
        }
        int currentC;
        int currentR;
        void ResetRowColumns()
        {
            currentC = 1;
            currentR = 1;
        }
        ResetRowColumns();
        BasicList<BasicList<C>> output = new();
        BasicList<BasicList<C>> temps;
        temps = GetSpecificCombo(currentR, currentC, EnumTempInfo.Horizontal, howManyNeeded);
        output.AddRange(temps);
        ResetRowColumns();
        temps = GetSpecificCombo(currentR, currentC, EnumTempInfo.Vertical, howManyNeeded);
        output.AddRange(temps);
        ResetRowColumns();
        temps = GetSpecificCombo(currentR, currentC, EnumTempInfo.DiagRightH, howManyNeeded);
        output.AddRange(temps);
        currentC = _howManyColumns;
        temps = GetSpecificCombo(currentR, currentC, EnumTempInfo.DiagLeftH, howManyNeeded);
        output.AddRange(temps);
        return output;
    }
    private BasicList<BasicList<C>> GetSpecificCombo(int startR, int startC, EnumTempInfo direction, int howMany, bool possibleDup = false)
    {
        BasicList<BasicList<C>> output = new();
        int currentR;
        int currentC;
        currentC = startC;
        currentR = startR;
        var real = direction switch
        {
            EnumTempInfo.None => throw new CustomBasicException("None not supported"),
            EnumTempInfo.Horizontal => EnumDirection.Horizontal,
            EnumTempInfo.Vertical => EnumDirection.Vertical,
            EnumTempInfo.DiagRightH or EnumTempInfo.DiagRightV => EnumDirection.DiagRight,
            EnumTempInfo.DiagLeftH or EnumTempInfo.DiagLeftV => EnumDirection.DiagLeft,
            _ => throw new CustomBasicException("Not Supported"),
        };
        int x = 0;
        BasicList<C> temps;
        do
        {
            x++;
            if (x > 1 || possibleDup == false)
            {
                temps = GetSpecificVectors(currentR, currentC, real, howMany);
                if (temps.Count == howMany)
                {
                    output.Add(temps); //so it can try other combos.
                }
            }
            switch (direction)
            {
                case EnumTempInfo.None:
                    break;
                case EnumTempInfo.Horizontal:
                    currentR++;
                    if (currentR > _howManyRows)
                    {
                        currentC++;
                        currentR = startR;
                    }
                    if (currentC > _howManyColumns)
                    {
                        return output;
                    }
                    break;
                case EnumTempInfo.DiagRightH:
                    currentR++;
                    if (currentR > _howManyRows)
                    {
                        currentC++;
                        currentR = startR;
                    }
                    if (currentC > _howManyColumns)
                    {
                        return output;
                    }
                    break;
                case EnumTempInfo.DiagLeftH:
                    currentC--;

                    if (currentC == 0)
                    {
                        currentC = startC;
                        currentR++;
                    }
                    if (currentR > _howManyRows)
                    {
                        return output;
                    }
                    break;
                case EnumTempInfo.Vertical:
                    currentC++;
                    if (currentC > _howManyColumns)
                    {
                        currentR++;
                        currentC = startC;
                    }
                    if (currentR > _howManyRows)
                        return output;
                    break;
                case EnumTempInfo.DiagRightV:

                case EnumTempInfo.DiagLeftV:
                    currentC++;
                    if (currentC > _howManyColumns)
                    {
                        return output;
                    }
                    break;
                default:
                    break;
            }

        } while (true);
    }
    public BasicList<C> GetWinCombo(BasicList<BasicList<C>> comboList)
    {
        foreach (var currentCombo in comboList)
            if (currentCombo.All(temps => temps.IsFilled()) == true)
            {
                object searchObject = MainObjectSelector!.Invoke(currentCombo.First());
                bool allTrue = true;
                object compareObject;
                foreach (var item in currentCombo)
                {
                    compareObject = MainObjectSelector.Invoke(item);
                    if (compareObject.Equals(searchObject) == false)
                    {
                        allTrue = false;
                        break;
                    }
                }
                if (allTrue == true)
                {
                    return currentCombo;
                }
            };
        return new();
    }
    public BasicList<C> GetEmptySpaces()
    {
        return _privateDict.Values.Where(items => items.IsFilled() == false).ToBasicList();
    }
    public BasicList<C> GetAlmostWinList(BasicList<BasicList<C>> comboList)
    {
        BasicList<C> output = new();
        bool allTrue;
        foreach (var currentCombo in comboList)
        {
            int needs;
            needs = currentCombo.Count - 1;
            int actual = currentCombo.Count(Items => Items.IsFilled());
            if (needs == actual)
            {
                object? searchObject = null;
                object? compareObject;
                C? fillItem = null;
                allTrue = true;
                foreach (var item in currentCombo)
                    if (item.IsFilled() == false)
                    {
                        fillItem = item;
                    }
                    else if (searchObject == null)
                    {
                        searchObject = MainObjectSelector!.Invoke(item);
                    }
                    else
                    {
                        compareObject = MainObjectSelector!.Invoke(item);
                        if (compareObject.Equals(searchObject) == false)
                        {
                            allTrue = false;
                            break;
                        }
                    }
                if (fillItem == null)
                {
                    throw new CustomBasicException("There was no fill item.  Rethink");
                }
                if (allTrue == true)
                {
                    output.Add(fillItem);
                }
            }
        }
        return output;
    }
    public bool IsFilled(Vector thisV)
    {
        C thisS = this[thisV];
        return thisS.IsFilled();
    }
    public bool IsFilled(int row, int column)
    {
        C thisS = this[row, column];
        return thisS.IsFilled();
    }
    public bool DidWin(BasicList<BasicList<C>> comboList)
    {
        foreach (var currentCombo in comboList)
            if (currentCombo.All(Temps => Temps.IsFilled()) == true)
            {
                object searchObject = MainObjectSelector!.Invoke(currentCombo.First());
                bool allTrue = true;
                object compareObject;
                foreach (var item in currentCombo)
                {
                    compareObject = MainObjectSelector.Invoke(item);
                    if (compareObject.Equals(searchObject) == false)
                    {
                        allTrue = false;
                        break;
                    }
                }
                if (allTrue == true)
                {
                    return true;
                }
            };
        return false;
    }
    public bool IsAllFilled()
    {
        return _privateDict.Values.All(items => items.IsFilled() == true);
    }
    public BasicList<C> GetAllColumns(int row)
    {
        return GetSpecificVectors(row, 1, EnumDirection.Horizontal, _howManyRows);
    }
    public BasicList<C> GetAllRows(int column)
    {
        return GetSpecificVectors(1, column, EnumDirection.Vertical, _howManyColumns);
    }
    public BasicList<C> GetSpecificVectors(int row, int column, EnumDirection direction, int count, Predicate<C>? predicate = null)
    {
        Check();
        if (column > _howManyColumns || row > _howManyRows)
        {
            throw new CustomBasicException("Out of bounds for getting specific vectors");
        }
        if (column == 0)
        {
            throw new CustomBasicException("Column cannot be 0 When Getting Vectors");
        }
        BasicList<C> output = new();
        int currentR;
        int currentC;
        currentC = column;
        currentR = row;
        for (int i = 0; i < count; i++)
        {
            Vector thisV;
            try
            {
                thisV = GetVector(currentR, currentC);
                if (predicate == null)
                {
                    output.Add(_privateDict[thisV]);
                }
                else if (predicate.Invoke(_privateDict[thisV]) == true)
                {
                    output.Add(_privateDict[thisV]);
                }
            }
            catch (Exception ex)
            {
                throw new CustomBasicException($"Error.  Row Trying Is {row} And Column Is {column}.  Message Was {ex.Message}");
            }
            switch (direction)
            {
                case EnumDirection.None:
                    throw new CustomBasicException("No direction specified");
                case EnumDirection.Horizontal:
                    currentC++;
                    break;
                case EnumDirection.Vertical:
                    currentR++;
                    break;
                case EnumDirection.DiagRight:
                    currentC++;
                    currentR++;
                    break;
                case EnumDirection.DiagLeft:
                    currentC--;
                    currentR++;
                    break;
                default:
                    throw new CustomBasicException("Not supported");
            }
            if (currentC > _howManyColumns || currentR > _howManyRows || currentC <= 0 || currentR <= 0)
            {
                return output;
            }
        }
        return output;
    }
    private int _howManyRows;
    private int _howManyColumns;
    public C this[int row, int column]
    {
        get
        {
            Check();
            Vector ThisGrid = GetVector(row, column);
            return _privateDict[ThisGrid];
        }
    }
    public C this[Vector thisV]
    {
        get
        {
            Check();
            return _privateDict[thisV];
        }
    }
    private void Check()
    {
        if (_privateDict.Count == 0)
        {
            throw new CustomBasicException("You have to have at least one item for your dictionary");
        }
    }
    public void SetDimensions(int rows, int columns)
    {
        if (rows <= 0 || columns <= 0)
        {
            throw new CustomBasicException("The rows and columns must be greater than 0");
        }
        _howManyRows = rows;
        _howManyColumns = columns;
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                Vector thisGrid = new(x + 1, y + 1);
                C thisC = new();
                thisC.Vector = thisGrid;
                _privateDict[thisGrid] = thisC;
            }
        }
    }
    public void Clear()
    {
        Check();
        foreach (var thisItem in _privateDict.Values)
        {
            thisItem.ClearSpace();
        }
    }
    public IEnumerator<C> GetEnumerator()
    {
        return _privateDict.Values.GetEnumerator();
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
        return _privateDict.Values.GetEnumerator();
    }
}