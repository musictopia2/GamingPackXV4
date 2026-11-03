namespace LifeCardGame.Core.Data;
[UseScoreboard]
public partial class LifeCardGamePlayerItem : PlayerSingleHand<LifeCardGameCardInformation>
{
    [ScoreColumn]
    public partial int Points { get; set; }
    public string LifeString { get; set; } = "";
    [JsonIgnore]
    public LifeStoryHand? LifeStory { get; set; }
}