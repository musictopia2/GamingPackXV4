namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Data;

public class RowInfo
{
    public string Description { get; set; } = "";
    public int RowNumber { get; set; }
    public EnumRow RowSection { get; set; }
    public bool IsTop { get; set; }
    public bool IsRecent { get; set; }
    public int? Possible { get; set; }
    public int? PointsObtained { get; set; }
    internal bool IsAllFive()
    {
        if (Description == "Kismet (5 Of A Kind)" || Description == "Yahtzee")
        {
            return true;
        }
        return false;
    }
    public bool HasFilledIn()
    {
        if (RowSection == EnumRow.Header || RowSection == EnumRow.Totals)
        {
            throw new Exception("HasFilledIn can only be figured out for Bonus or Regular Rows");
        }
        if (PointsObtained.HasValue == false)
        {
            return false;
        }
        return true;
    }
    public void ClearText()
    {
        Possible = default;
        PointsObtained = default;
        IsRecent = false;
    }
    public void ClearPossibleScores()
    {
        Possible = default;
    }
    public RowInfo(EnumRow section, bool isTop)
    {
        RowSection = section;
        IsTop = isTop;
    }
    public RowInfo() { }
}