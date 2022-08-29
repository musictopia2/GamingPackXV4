namespace ClueBoardGame.Core.Data;
public partial class ClueBoardGamePlayerItem : PlayerBoardGame<EnumColorChoice>, IPlayerObject<CardInfo>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
    public DeckRegularDict<CardInfo> MainHandList { get; set; } = new DeckRegularDict<CardInfo>();
    public override bool CanStartInGame
    {
        get
        {
            if (PlayerCategory != EnumPlayerCategory.Computer)
            {
                return true;
            }
            if (NickName.StartsWith("Computeridle"))
            {
                return false;
            }
            return true;
        }
    }
    public Dictionary<int, DetectiveInfo> DetectiveList { get; set; } = new();
    [JsonIgnore]
    public DeckRegularDict<CardInfo> StartUpList { get; set; } = new DeckRegularDict<CardInfo>();
}