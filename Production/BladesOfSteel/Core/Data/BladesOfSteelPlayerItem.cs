
namespace BladesOfSteel.Core.Data;
[UseScoreboard]
public partial class BladesOfSteelPlayerItem : PlayerSingleHand<RegularSimpleCard>
{
    public RegularSimpleCard? FaceOff { get; set; }
    [ScoreColumn]
    public partial int Score { get; set; }
    public DeckRegularDict<RegularSimpleCard> AttackList { get; set; } = [];
    public DeckRegularDict<RegularSimpleCard> DefenseList { get; set; } = [];
    [JsonIgnore]
    public PlayerAttackCP? AttackPile { get; set; }
    [JsonIgnore]
    public PlayerDefenseCP? DefensePile { get; set; }
}