namespace TicTacToe.Core.Data;
public class SpaceInfoCP : IBasicSpace
{
    public Vector Vector { get; set; }
    public EnumSpaceType Status { get; set; }
    public void ClearSpace()
    {
        Status = EnumSpaceType.Blank;
    }
    public bool IsFilled()
    {
        return Status != EnumSpaceType.Blank;
    }
}