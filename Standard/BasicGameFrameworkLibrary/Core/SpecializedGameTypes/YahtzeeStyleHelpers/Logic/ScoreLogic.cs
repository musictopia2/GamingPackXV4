namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Logic;
public class ScoreLogic : IScoreLogic
{
    private readonly ScoreContainer _scoreContainer;
    private readonly IYahtzeeStyle _yahtzeeStyle;
    public ScoreLogic(ScoreContainer scoreContainer, IYahtzeeStyle yahtzeeStyle)
    {
        _scoreContainer = scoreContainer;
        _yahtzeeStyle = yahtzeeStyle;
    }
    private int GetTopScore => _scoreContainer.RowList.Where(items => items.IsTop == true
        && items.PointsObtained.HasValue == true && (items.RowSection == EnumRow.Regular
    || items.RowSection == EnumRow.Bonus))
    .Sum(items => items.PointsObtained!.Value);
    private int GetBottomScore => _scoreContainer.RowList.Where(items => items.PointsObtained.HasValue == true && items.IsTop == false
        && items.RowSection == EnumRow.Regular).Sum(items => items.PointsObtained!.Value);
    public int TotalScore => GetTopScore + GetBottomScore;
    public BasicList<RowInfo> GetAvailableScores => _scoreContainer.RowList.Where(items => items.RowSection == EnumRow.Regular).ToBasicList();
    public void ClearRecent()
    {
        _scoreContainer.RowList.ForEach(x => x.IsRecent = false);
    }
    public void LoadBoard()
    {
        _scoreContainer.RowList.Clear();
        RowInfo row;
        row = new(EnumRow.Header, true);
        row.RowNumber = 0;
        _scoreContainer.RowList.Add(row);
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
            _scoreContainer.RowList.Add(newRow);
        }
        row.IsTop = true;
        row.RowSection = EnumRow.Bonus;
        row.Description = "Bonus";
        x += 1;
        row.RowNumber = x;
        _scoreContainer.RowList.Add(row);
        x += 1;
        row = new();
        row.IsTop = true;
        row.RowSection = EnumRow.Totals;
        row.RowNumber = x;
        _scoreContainer.RowList.Add(row);
        x = 0;
        tempList = _yahtzeeStyle.GetBottomText;
        foreach (var thisItem in tempList)
        {
            x += 1;
            RowInfo newRow = new();
            newRow.RowSection = EnumRow.Regular;
            newRow.Description = thisItem;
            newRow.IsTop = false;
            newRow.RowNumber = x;
            _scoreContainer.RowList.Add(newRow);
        }
        x += 1;
        row = new();
        row.IsTop = false;
        row.RowSection = EnumRow.Totals;
        row.RowNumber = x;
        _scoreContainer.RowList.Add(row);
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
        bool rets = _scoreContainer.RowList.Where(items => items.IsTop == true && items.RowSection == EnumRow.Regular)
            .All(xx => xx.HasFilledIn() == true);
        if (rets == false)
        {
            return false;
        }
        RowInfo thisRow = _scoreContainer.RowList.Single(items => items.RowSection == EnumRow.Bonus);
        return !thisRow.HasFilledIn();
    }
    private void FinishMarking(RowInfo currentRow)
    {
        _scoreContainer.RowList.Last().PointsObtained = GetBottomScore;
        RowInfo tempRow;
        if (NeedsToCalculateBonus() == false)
        {
            tempRow = (from xx in _scoreContainer.RowList
                       where xx.IsTop == true && xx.RowSection == EnumRow.Totals
                       select xx).Single();
            tempRow.PointsObtained = GetTopScore;
            if (Extra5OfAKind(currentRow) == true)
            {
                _yahtzeeStyle.Extra5OfAKind();
            }
            return; // because no need to calculate bonus
        }
        tempRow = (from xx in _scoreContainer.RowList
                   where xx.RowSection == EnumRow.Bonus
                   select xx).Single();
        tempRow.PointsObtained = _yahtzeeStyle.BonusAmount(GetTopScore);
        tempRow = (from xx in _scoreContainer.RowList
                   where xx.IsTop == true && xx.RowSection == EnumRow.Totals
                   select xx).Single();
        tempRow.PointsObtained = GetTopScore;
        if (currentRow != null)
        {
            if (Extra5OfAKind(currentRow!) == true)
            {
                _yahtzeeStyle.Extra5OfAKind();

            }
        }
    }
    private bool Extra5OfAKind(RowInfo currentRow)
    {
        if (_scoreContainer.HasAllFive() == false)
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
        if (_yahtzeeStyle.HasExceptionFor5Kind == true)
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
        _scoreContainer.DiceList = _yahtzeeStyle.GetDiceList();
        if (_scoreContainer.DiceList.Count != 5)
        {
            throw new CustomBasicException("Must have 5 dice, not " + _scoreContainer.DiceList.Count);
        }
        PopulateTopScores();
        _yahtzeeStyle.PopulateBottomScores();
    }
    private void PopulateTopScores()
    {
        6.Times(x =>
        {
            if (_scoreContainer.RowList[x].HasFilledIn() == false)
            {
                _scoreContainer.RowList[x].Possible = _scoreContainer.DiceList.Where(y => y.Value == x).Sum(y => y.Value);
                if (_scoreContainer.RowList[x].Possible == 0)
                {
                    _scoreContainer.RowList[x].Possible = null;
                }
            }
        });
    }
    private void ClearPossibleScores()
    {
        _scoreContainer.RowList.ForEach(x => x.ClearPossibleScores());
    }
    public void StartTurn()
    {
        if (_scoreContainer.StartTurn == null)
        {
            throw new CustomBasicException("Nobody is handling the start turn which should mark the missnextturn to false");
        }
        _scoreContainer.StartTurn.Invoke();
        ClearRecent();
    }
}