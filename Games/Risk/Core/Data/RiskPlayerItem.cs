namespace Risk.Core.Data;
[UseScoreboard]
public partial class RiskPlayerItem : PlayerBoardGame<EnumColorChoice>, IPlayerObject<RiskCardInfo>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
    [JsonIgnore] //only needed temporarily this time.
    public int ReinforcementsGained { get; set; }
    [JsonIgnore] //i think its only here for claiming territories.
    public int ArmiesToBegin { get; set; }
    [JsonIgnore] //just in case.
    [ScoreColumn]
    public int RiskCards => MainHandList.Count;
    public DeckRegularDict<RiskCardInfo> MainHandList { get; set; } = new();
    [JsonIgnore]
    public DeckRegularDict<RiskCardInfo> StartUpList { get; set; } = new();
}