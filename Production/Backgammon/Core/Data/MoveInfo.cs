namespace Backgammon.Core.Data;
public class MoveInfo
{
    public int SpaceFrom { get; set; }
    public int SpaceTo { get; set; }
    public int DiceNumber { get; set; }
    public EnumStatusType Results { get; set; }
}