namespace LifeBoardGame.Core.Data;
public class SpinnerPositionData
{
    public int SpinnerPosition { get; set; } //try to make them set again (don't trust what was there).
    public int ChangePositions { get; set; }
    public bool CanBetween { get; set; }
    public int HighSpeedUpTo { get; set; } // this is how long it will do highspeed (so it can vary)
}