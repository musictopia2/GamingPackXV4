namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
public class ScoreLogic(ScoreContainer scoreContainer, IYahtzeeStyle yahtzeeStyle) : IScoreLogic
{
    private int GetTopScore => scoreContainer.RowList.Where(items => items.IsTop == true
        && items.PointsObtained.HasValue == true && (items.RowSection == EnumRow.Regular
    || items.RowSection == EnumRow.Bonus))
    .Sum(items => items.PointsObtained!.Value);
    private int GetBottomScore => scoreContainer.RowList.Where(items => items.PointsObtained.HasValue == true && items.IsTop == false
        && items.RowSection == EnumRow.Regular).Sum(items => items.PointsObtained!.Value);
    public int TotalScore => GetTopScore + GetBottomScore;
    public BasicList<RowInfo> GetAvailableScores => scoreContainer.RowList.Where(items => items.RowSection == EnumRow.Regular).ToBasicList();
    public void ClearRecent()
    {
        scoreContainer.RowList.ForEach(x => x.IsRecent = false);
    }
    public void LoadBoard()
    {
        scoreContainer.RowList.Clear();
        RowInfo row;
        row = new(EnumRow.Header, true);
        row.RowNumber = 0;
        scoreContainer.RowList.Add(row);
        row = new(EnumRow.Header, false);
        row.RowNumber = 0;
        BasicList<string> tempList = new()
    {
        "Aces (1's)",
        "Dueces (2's)",
        "Treys (3's)",
        "Fours",
        "Fives",
        "Sixes"
    };
        int x = 0;
        foreach (var thisItem in tempList)
        {
            x += 1;
            RowInfo newRow = new();
            newRow.RowSection = EnumRow.Regular;
            newRow.Description = thisItem;
            newRow.IsTop = true;
            newRow.RowNumber = x;
            scoreContainer.RowList.Add(newRow);
        }
        row.IsTop = true;
        row.RowSection = EnumRow.Bonus;
        row.Description = "Bonus";
        x += 1;
        row.RowNumber = x;
        scoreContainer.RowList.Add(row);
        x += 1;
        row = new();
        row.IsTop = true;
        row.RowSection = EnumRow.Totals;
        row.RowNumber = x;
        scoreContainer.RowList.Add(row);
        x = 0;
        tempList = yahtzeeStyle.GetBottomText;
        foreach (var thisItem in tempList)
        {
            x += 1;
            RowInfo newRow = new();
            newRow.RowSection = EnumRow.Regular;
            newRow.Description = thisItem;
            newRow.IsTop = false;
            newRow.RowNumber = x;
            scoreContainer.RowList.Add(newRow);
        }
        x += 1;
        row = new();
        row.IsTop = false;
        row.RowSection = EnumRow.Totals;
        row.RowNumber = x;
        scoreContainer.RowList.Add(row);
    }
    public void MarkScore(RowInfo currentRow)
    {
        currentRow.IsRecent = true;
        if (currentRow.Possible.HasValue == true)
        {
            currentRow.PointsObtained = currentRow.Possible;
        }
        else
        {
            currentRow.PointsObtained = 0;
        }
        ClearPossibleScores();
        FinishMarking(currentRow);
    }
    private bool NeedsToCalculateBonus()
    {
        bool rets = scoreContainer.RowList.Where(items => items.IsTop == true && items.RowSection == EnumRow.Regular)
            .All(xx => xx.HasFilledIn() == true);
        if (rets == false)
        {
            return false;
        }
        RowInfo thisRow = scoreContainer.RowList.Single(items => items.RowSection == EnumRow.Bonus);
        return !thisRow.HasFilledIn();
    }
    private void FinishMarking(RowInfo currentRow)
    {
        scoreContainer.RowList.Last().PointsObtained = GetBottomScore;
        RowInfo tempRow;
        if (NeedsToCalculateBonus() == false)
        {
            tempRow = (from xx in scoreContainer.RowList
                       where xx.IsTop == true && xx.RowSection == EnumRow.Totals
                       select xx).Single();
            tempRow.PointsObtained = GetTopScore;
            if (Extra5OfAKind(currentRow) == true)
            {
                yahtzeeStyle.Extra5OfAKind();
            }
            return; // because no need to calculate bonus
        }
        tempRow = (from xx in scoreContainer.RowList
                   where xx.RowSection == EnumRow.Bonus
                   select xx).Single();
        tempRow.PointsObtained = yahtzeeStyle.BonusAmount(GetTopScore);
        tempRow = (from xx in scoreContainer.RowList
                   where xx.IsTop == true && xx.RowSection == EnumRow.Totals
                   select xx).Single();
        tempRow.PointsObtained = GetTopScore;
        if (currentRow != null)
        {
            if (Extra5OfAKind(currentRow!) == true)
            {
                yahtzeeStyle.Extra5OfAKind();

            }
        }
    }
    private bool Extra5OfAKind(RowInfo currentRow)
    {
        if (scoreContainer.HasAllFive() == false)
        {
            return false;
        }
        if (currentRow.IsAllFive() == true)
        {
            return false;
        }
        if (currentRow.PointsObtained.HasValue == false)
        {
            throw new CustomBasicException("If its 5 of a kind and no score, should have shown as allfives.");
        }
        if (yahtzeeStyle.HasExceptionFor5Kind == true)
        {
            return true;
        }
        if (currentRow.PointsObtained == 0)
        {
            return false; //no exception.  means if you marked it off, then nothing period.
        }
        return true;
    }
    public void PopulatePossibleScores()
    {
        ClearRecent();
        ClearPossibleScores();
        scoreContainer.DiceList = yahtzeeStyle.GetDiceList();
        if (scoreContainer.DiceList.Count != 5)
        {
            throw new CustomBasicException("Must have 5 dice, not " + scoreContainer.DiceList.Count);
        }
        PopulateTopScores();
        yahtzeeStyle.PopulateBottomScores();
    }
    private void PopulateTopScores()
    {
        6.Times(x =>
        {
            if (scoreContainer.RowList[x].HasFilledIn() == false)
            {
                scoreContainer.RowList[x].Possible = scoreContainer.DiceList.Where(y => y.Value == x).Sum(y => y.Value);
                if (scoreContainer.RowList[x].Possible == 0)
                {
                    scoreContainer.RowList[x].Possible = null;
                }
            }
        });
    }
    private void ClearPossibleScores()
    {
        scoreContainer.RowList.ForEach(x => x.ClearPossibleScores());
    }
    public void StartTurn()
    {
        if (scoreContainer.StartTurn == null)
        {
            throw new CustomBasicException("Nobody is handling the start turn which should mark the missnextturn to false");
        }
        scoreContainer.StartTurn.Invoke();
        ClearRecent();
    }
}