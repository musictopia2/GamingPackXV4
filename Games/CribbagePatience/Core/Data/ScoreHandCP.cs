namespace CribbagePatience.Core.Data;
public class ScoreHandCP
{
    public DeckRegularDict<CribbageCard> CardList { get; set; } = new ();
    public BasicList<CribbageCombos> Scores { get; set; } = new();
    public DeckRegularDict<CribbageCard> TempList { get; set; } = new ();
    public EnumHandCategory HandCategory { get; set; }
    public string Text
    {
        get
        {
            if (HandCategory == EnumHandCategory.Crib)
            {
                return "Crib Score";
            }
            if (HandCategory == EnumHandCategory.Hand1)
            {
                return "Hand 1 Score";
            }
            if (HandCategory == EnumHandCategory.Hand2)
            {
                return "Hand 2 Score";
            }
            throw new CustomBasicException("Text Not Found");
        }
    }
    public string HandData()
    {
        if (HandCategory == EnumHandCategory.Hand1)
        {
            return "Hand 1";
        }
        else if (HandCategory == EnumHandCategory.Hand2)
        {
            return "Hand 2";
        }
        else if (HandCategory == EnumHandCategory.Crib)
        {
            return "Crib So Far";
        }
        else
        {
            throw new CustomBasicException("Nothing found");
        }
    }
    public (int Row, int Column) GetRowColumn()
    {
        if (HandCategory == EnumHandCategory.Crib)
        {
            return (0, 1);
        }
        if (HandCategory == EnumHandCategory.Hand1)
        {
            return (1, 1);
        }
        if (HandCategory == EnumHandCategory.Hand2)
        {
            return (1, 2);
        }
        throw new CustomBasicException("Nothing found for the row, column information");
    }
    public int TotalScore()
    {
        return Scores.Sum(x => x.Points);
    }
    public void NewRound()
    {
        Scores.Clear();
        CardList.Clear();
        TempList.Clear();
    }
}