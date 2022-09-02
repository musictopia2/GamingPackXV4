namespace RageCardGame.Blazor;
public static class ScoreModule
{
    public static BasicList<ScoreColumnModel> GetScores()
    {
        BasicList<ScoreColumnModel> output = new();
        output.AddColumn("Cards Left", true, nameof(RageCardGamePlayerItem.ObjectCount))
            .AddColumn("Bid Amount", true, nameof(RageCardGamePlayerItem.BidAmount), visiblePath: nameof(RageCardGamePlayerItem.RevealBid))
            .AddColumn("Tricks Won", true, nameof(RageCardGamePlayerItem.TricksWon))
            .AddColumn("Correctly Bidded", true, nameof(RageCardGamePlayerItem.CorrectlyBidded))
            .AddColumn("Score Round", true, nameof(RageCardGamePlayerItem.ScoreRound))
            .AddColumn("Score Game", true, nameof(RageCardGamePlayerItem.ScoreGame));
        return output;
    }
}