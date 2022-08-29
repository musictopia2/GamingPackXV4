namespace DiceBoardGamesMultiplayer.Core.Data;
[UseScoreboard]
public partial class DiceBoardGamesMultiplayerPlayerItem : PlayerBoardGame<EnumColorChoice>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
}