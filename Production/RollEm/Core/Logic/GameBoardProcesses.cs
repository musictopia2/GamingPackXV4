namespace RollEm.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses(RollEmGameContainer gameContainer, RollEmVMData model)
{
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
        gameContainer.SaveRoot!.SpaceList = gameContainer.NumberList!.Values.Select(items => GetSavedItem(items)).ToBasicList();
    }
    public void ClearBoard()
    {
        foreach (var item in gameContainer.NumberList!.Values)
        {
            item.IsCrossed = false;
            item.Recently = false;
        }
        model.Cup!.HowManyDice = 2;
        model.Cup.HideDice();
        model.Cup.CanShowDice = false;
        gameContainer.Aggregator.RepaintBoard(); //try this too.
    }
    public void LoadSavedGame()
    {
        int x = 0;
        gameContainer.SaveRoot!.SpaceList.ForEach(items =>
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
        gameContainer.Aggregator.RepaintBoard();
    }
    private void CrossOffSaved(int x)
    {
        gameContainer.NumberList![x].IsCrossed = true;
    }
    private void CrossOffTemp(int x)
    {
        gameContainer.NumberList![x].Recently = true;
    }
    private void CrossOffPreviousNumbers()
    {
        var temps = gameContainer.NumberList!.Values.Where(items => items.Recently == true).ToBasicList();
        temps.ForEach(items =>
        {
            items.Recently = false;
            items.IsCrossed = true;
        });
    }
    private void ClearPreviousNumbers()
    {
        var temps = gameContainer.NumberList!.Values.Where(items => items.Recently == true).ToBasicList();
        temps.ForEach(items =>
        {
            items.Recently = false;
        });
    }
    private int HowManyRecently => gameContainer.NumberList!.Values.Count(items => items.Recently == true);
    private int RecentTotal => gameContainer.NumberList!.Values.Where(items => items.Recently == true).Sum(Items => Items.Number);
    public int CalculateScore => gameContainer.NumberList!.Values.Where(items => items.IsCrossed == false && items.Number <= 9).Sum(Items => Items.Number);
    private bool NeedOneDice => !gameContainer.NumberList!.Values.Any(items => items.Number >= 7 && items.IsCrossed == false && items.Number <= 9);
    public async Task MakeMoveAsync(int Number)
    {
        CrossOffTemp(Number);
        gameContainer.Aggregator.RepaintBoard();
        if (gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self && gameContainer.Test!.NoAnimations == false)
        {
            await gameContainer.Delay!.DelaySeconds(.5);
        }
    }
    public void FinishMove()
    {
        CrossOffPreviousNumbers();
        if (NeedOneDice == true)
        {
            model.Cup!.HowManyDice = 1;
        }
        gameContainer.Aggregator.RepaintBoard();
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
    public int GetDiceTotal => model.Cup!.TotalDiceValue;
    public bool CanMakeMove(int number)
    {
        if (gameContainer.Test!.AllowAnyMove == true)
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
            gameContainer.Aggregator.RepaintBoard();
        }
    }
    public BasicList<int> GetNumberList()
    {
        if (gameContainer.NumberList!.Values.Any(items => items.Recently))
        {
            throw new CustomBasicException("Cannot be recently checked when the computer is checking for numbers left");
        }
        BasicList<NumberInfo> tempList = gameContainer.NumberList.Values.Where(items => items.IsCrossed == false).ToBasicList();
        return tempList.Select(items => items.Number).ToBasicList();
    }
}