namespace SorryDicedGame.Core.Data;
public class BoardModel
{
    public EnumColorChoice Color { get; set; }
    public int PlayerOwned { get; set; }
    public EnumBoardCategory At { get; set; } = EnumBoardCategory.Start; //start out at start.
}