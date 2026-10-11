namespace TicTacToe.Core.Data;
[UseStateChanged]
public partial class SpaceInfoCP : IBasicSpace
{
    public Vector Vector { get; set; }
    [StateProperty]
    public partial EnumSpaceType Status { get; set; }
    public void ClearSpace()
    {
        Status = EnumSpaceType.Blank;
    }
    public bool IsFilled()
    {
        return Status != EnumSpaceType.Blank;
    }
}