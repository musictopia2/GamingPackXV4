namespace Xactika.Core.Data;
[UseStateChanged]
public partial class PieceModel
{
    public EnumShapes ShapeUsed { get; set; }
    public int HowMany { get; set; }
    [StateProperty]
    public partial bool IsSelected { get; set; }
}