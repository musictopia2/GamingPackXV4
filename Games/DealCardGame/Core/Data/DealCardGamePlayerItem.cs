namespace DealCardGame.Core.Data;
[UseScoreboard]
public partial class DealCardGamePlayerItem : PlayerSingleHand<DealCardGameCardInformation>
{
    [ScoreColumn]
    public decimal Money { get; set; } //has to decide where the animation will take place at (since its not really being discarded)
    public DeckRegularDict<DealCardGameCardInformation> BankedCards { get; set; } = [];


    public BasicList<SetPropertiesModel> SetData { get; set; } = [];
    //public Dictionary<EnumColor, BasicList<DealCardGameCardInformation>> SetData { get; set; } = [];
}