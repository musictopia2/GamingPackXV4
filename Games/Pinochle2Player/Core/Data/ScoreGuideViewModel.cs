namespace Pinochle2Player.Core.Data;
public class ScoreGuideViewModel
{
    public BasicList<ScoreValuePair> PointValueList = new();
    public BasicList<ScoreValuePair> ClassAList = new();
    public BasicList<ScoreValuePair> ClassBList = new();
    public BasicList<ScoreValuePair> ClassCList = new();
    private static BasicList<ScoreValuePair> PopulatePoints()
    {
        return new()
        {
            new ScoreValuePair("Each Ace", 11),
            new ScoreValuePair("Each 10", 10),
            new ScoreValuePair("Each King", 4),
            new ScoreValuePair("Each Queen", 3),
            new ScoreValuePair("Each Jack", 2),
            new ScoreValuePair("Last Trick", 10)
        };
    }
    private static BasicList<ScoreValuePair> PopulateACustomBasicList()
    {
        return new()
        {
            new ScoreValuePair("A, K, Q, J, 10 of trump suit (flush or sequence)", 150),
            new ScoreValuePair("K, Q of trump (royal marriage)", 40),
            new ScoreValuePair("K, Q same suit of any other suit (marriage)", 20),
            new ScoreValuePair("9 of trump-(lowest trump)", 10)
        };
    }
    private static BasicList<ScoreValuePair> PopulateBCustomBasicList()
    {
        return new()
        {
            new ScoreValuePair("4 Aces (A) of different suits", 100),
            new ScoreValuePair("4 Kings (K) of different suits", 80),
            new ScoreValuePair("4 Queens (Q) of different suits", 60),
            new ScoreValuePair("4 Jacks (J) of different suits", 40)
        };
    }
    private static BasicList<ScoreValuePair> PopulateCCustomBasicList()
    {
        return new()
        {
            new ScoreValuePair("Queen of Spades and Jack of Diamonds" + Constants.VBCrLf + "(Pinochle)", 40)
        };
    }
    public ScoreGuideViewModel()
    {
        PointValueList = PopulatePoints();
        ClassAList = PopulateACustomBasicList();
        ClassBList = PopulateBCustomBasicList();
        ClassCList = PopulateCCustomBasicList();
    }
}