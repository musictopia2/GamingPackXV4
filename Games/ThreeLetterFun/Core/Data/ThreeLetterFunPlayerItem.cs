namespace ThreeLetterFun.Core.Data;
[UseScoreboard]
public partial class ThreeLetterFunPlayerItem : PlayerSingleHand<ThreeLetterFunCardData>
{
    [ScoreColumn]
    public int CardsWon { get; set; }
    public bool TookTurn { get; set; }
    public int TimeToGetWord { get; set; } = -1; //-1 means did not get it.
    public BasicList<TilePosition> TileList { get; set; } = new(); //this is the tiles used to get the word.  only send if you actually get a word.  the other players needs to know the positions as well.  before sending; will have for yourself
    public int CardUsed { get; set; }
    public int MostRecent { get; set; }
    public void ClearTurn()
    {
        PrivateClear(false);
    }
    private void PrivateClear(bool tookTurn)
    {
        TookTurn = tookTurn;
        TimeToGetWord = -1;
        TileList.Clear();
        CardUsed = 0;
    }
    public void GaveUp()
    {
        PrivateClear(true);
    }
}