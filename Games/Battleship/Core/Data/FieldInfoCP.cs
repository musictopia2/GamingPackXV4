namespace Battleship.Core.Data;
public class FieldInfoCP : IBasicSpace
{
    public Vector Vector { get; set; } //no need for column or row because of vector.  this takes care of that.
    public string Letter { get; set; } = "";
    public EnumWhatHit Hit { get; set; }
    public int ShipNumber { get; set; }
    public string FillColor { get; set; } = cs.Blue;
    public void ClearSpace()
    {
        FillColor = cs.Blue;
        Hit = EnumWhatHit.None;
        ShipNumber = 0; //i think this too.
    }
    public bool IsFilled()
    {
        return Hit != EnumWhatHit.None; //if you missed, its still filled.
    }
}