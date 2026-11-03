namespace Battleship.Core.Data;

[UseStateChanged]
public partial class FieldInfoCP : IBasicSpace
{
    public Vector Vector { get; set; } //no need for column or row because of vector.  this takes care of that.
    public string Letter { get; set; } = "";
    [StateProperty]
    public partial EnumWhatHit Hit { get; set; }
    [StateProperty]
    public partial int ShipNumber { get; set; }
    [StateProperty]
    public partial string FillColor { get; set; } = cs1.Blue;
    public void ClearSpace()
    {
        FillColor = cs1.Blue;
        Hit = EnumWhatHit.None;
        ShipNumber = 0; //i think this too.
    }
    public bool IsFilled()
    {
        return Hit != EnumWhatHit.None; //if you missed, its still filled.
    }
}