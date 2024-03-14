namespace DealCardGame.Core.Data;
[UseScoreboard]
public partial class DealCardGamePlayerItem : PlayerSingleHand<DealCardGameCardInformation>
{
    [ScoreColumn]
    public decimal Money { get; set; } //has to decide where the animation will take place at (since its not really being discarded)
    public DeckRegularDict<DealCardGameCardInformation> BankedCards { get; set; } = [];
    public BasicList<SetPropertiesModel> SetData { get; set; } = [];
    public decimal Debt { get; set; } //go ahead and store as decimal even though it would be whole numbers.
    public BasicList<int> Payments { get; set; } = []; //this is so after everybody has paid, then will finish processing.
    public EnumAllPlayerStatus AllPlayerStatus { get; set; } = EnumAllPlayerStatus.None;
}