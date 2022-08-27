namespace PlainBoardGamesMultiplayer.Core.Data;
public class PlainBoardGamesMultiplayerPlayerItem : PlayerBoardGame<EnumColorChoice>
{
    public override bool DidChooseColor => Color.IsNull == false && Color != EnumColorChoice.None;
    public override void Clear()
    {
        Color = EnumColorChoice.None;
    }
}