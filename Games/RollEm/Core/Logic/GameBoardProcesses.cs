namespace RollEm.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses
{
    private readonly RollEmGameContainer _gameContainer;
    private readonly RollEmVMData _model;
    public GameBoardProcesses(RollEmGameContainer gameContainer, RollEmVMData model)
    {
        _gameContainer = gameContainer;
        _model = model;
    }
    private static string GetSavedItem(NumberInfo thisNumber) //not sure if we eventually do enums.
    {
        if (thisNumber.IsCrossed == true)
        {
            return "crossed";
        }
        if (thisNumber.Recently == true)
        {
            return "recent";
        }
        return "open";
    }
    public void SaveGame()
    {
        _gameContainer.SaveRoot!.SpaceList = _gameContainer.NumberList!.Values.Select(items => GetSavedItem(items)).ToBasicList();
    }
    public void ClearBoard()
    {
        foreach (var item in _gameContainer.NumberList!.Values)
        {
            item.IsCrossed = false;
            item.Recently = false;
        }
        _model.Cup!.HowManyDice = 2;
        _model.Cup.HideDice();
        _model.Cup.CanShowDice = false;
        _gameContainer.Aggregator.RepaintBoard(); //try this too.
    }
    public void LoadSavedGame()
    {
        int x = 0;
        _gameContainer.SaveRoot!.SpaceList.ForEach(items =>
        {
            x++;
            if (items == "crossed")
            {
                CrossOffSaved(x);
            }
            else if (items == "recent")
            {
                CrossOffTemp(x);
            }
            else if (items != "open")
            {
                throw new CustomBasicException("Wrong Text.  Rethink");
            }
        });
        _gameContainer.Aggregator.RepaintBoard();
    }
    private void CrossOffSaved(int x)
    {
        _gameContainer.NumberList![x].IsCrossed = true;
    }
    private void CrossOffTemp(int x)
    {
        _gameContainer.NumberList![x].Recently = true;
    }
    private void CrossOffPreviousNumbers()
    {
        var temps = _gameContainer.NumberList!.Values.Where(items => items.Recently == true).ToBasicList();
        temps.ForEach(items =>
        {
            items.Recently = false;
            items.IsCrossed = true;
        });
    }
    private void ClearPreviousNumbers()
    {
        var temps = _gameContainer.NumberList!.Values.Where(items => items.Recently == true).ToBasicList();
        temps.ForEach(items =>
        {
            items.Recently = false;
        });
    }
    private int HowManyRecently => _gameContainer.NumberList!.Values.Count(items => items.Recently == true);
    private int RecentTotal => _gameContainer.NumberList!.Values.Where(items => items.Recently == true).Sum(Items => Items.Number);
    public int CalculateScore => _gameContainer.NumberList!.Values.Where(items => items.IsCrossed == false && items.Number <= 9).Sum(Items => Items.Number);
    private bool NeedOneDice => !_gameContainer.NumberList!.Values.Any(items => items.Number >= 7 && items.IsCrossed == false && items.Number <= 9);
    public async Task MakeMoveAsync(int Number)
    {
        CrossOffTemp(Number);
        _gameContainer.Aggregator.RepaintBoard();
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self && _gameContainer.Test!.NoAnimations == false)
        {
            await _gameContainer.Delay!.DelaySeconds(.5);
        }
    }
    public void FinishMove()
    {
        CrossOffPreviousNumbers();
        if (NeedOneDice == true)
        {
            _model.Cup!.HowManyDice = 1;
        }
        _gameContainer.Aggregator.RepaintBoard();
    }
    public bool IsMoveFinished()
    {
        int Nums = RecentTotal;
        if (Nums == GetDiceTotal)
        {
            return true;
        }
        return false;
    }
    public int GetDiceTotal => _model.Cup!.TotalDiceValue;
    public bool CanMakeMove(int number)
    {
        if (_gameContainer.Test!.AllowAnyMove == true)
        {
            return true; //for testing.
        }
        int diceTotal = GetDiceTotal;
        if (HowManyRecently == 1)
        {
            return RecentTotal + number == diceTotal;
        }
        return number <= diceTotal;
    }
    public bool HadRecent
    {
        get
        {
            int Manys = HowManyRecently;
            if (Manys > 1)
            {
                throw new CustomBasicException("Can only have one recent one");
            }
            return Manys == 1;
        }
    }
    public void ClearRecent(bool doRefresh)
    {
        ClearPreviousNumbers();
        if (doRefresh == true)
        {
            _gameContainer.Aggregator.RepaintBoard();
        }
    }
    public BasicList<int> GetNumberList()
    {
        if (_gameContainer.NumberList!.Values.Any(items => items.Recently))
        {
            throw new CustomBasicException("Cannot be recently checked when the computer is checking for numbers left");
        }
        BasicList<NumberInfo> tempList = _gameContainer.NumberList.Values.Where(items => items.IsCrossed == false).ToBasicList();
        return tempList.Select(items => items.Number).ToBasicList();
    }
}