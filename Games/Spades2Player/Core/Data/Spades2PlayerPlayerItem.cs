namespace Spades2Player.Core.Data;
[UseScoreboard]
public partial class Spades2PlayerPlayerItem : PlayerTrick<EnumSuitList, Spades2PlayerCardInformation>
{
    [ScoreColumn]
    public int HowManyBids { get; set; }
    [ScoreColumn]
    public int Bags { get; set; }
    [ScoreColumn]
    public int CurrentScore { get; set; }
    [ScoreColumn]
    public int TotalScore { get; set; }
    private bool IsNil => HowManyBids == 0;
    public void CalculateScore()
    {
        bool nils = IsNil;
        int thisScore = 0;
        int bids = HowManyBids;
        if (TricksWon > 0 && IsNil == true)
        {
            thisScore = -100;
            thisScore += TricksWon;
            Bags += TricksWon;
        }
        else if (nils == true)
        {
            thisScore += 100;
        }
        else if (TricksWon < bids)
        {
            thisScore = bids * 10;
            thisScore *= -1;
        }
        else
        {
            thisScore = bids * 10;
            int Extras = TricksWon - bids;
            thisScore += Extras;
            Bags += Extras;
        }
        if (Bags >= 20)
        {
            thisScore -= 200;
            Bags -= 20;
        }
        else if (Bags >= 10)
        {
            thisScore -= 100;
            Bags -= 10;
        }
        CurrentScore = thisScore;
        TotalScore += CurrentScore;
    }
}