namespace SorryDicedGame.Core.Data;
[UseScoreboard]
public partial class SorryDicedGamePlayerItem : PlayerBoardGame<EnumColorChoice>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
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
    public EnumColorChoice SlideColor { get; set; }
}