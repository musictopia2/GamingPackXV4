namespace SequenceDice.Core.Data;
public class SpaceInfoCP : IBasicSpace
{
    public Vector Vector { get; set; }
    public int Player { get; set; }
    public string Color { get; set; } = cs.Transparent;
    public bool WasRecent { get; set; }
    public int Number { get; set; }
    public void ClearSpace()
    {
        Player = 0;
        Color = cs.Transparent;
        WasRecent = false;
        if (Number == 0)
        {
            throw new CustomBasicException("Number cannot be 0.  Rethink");
        }
    }
    public bool IsFilled()
    {
        return Player > 0; //if player is put in, then its filled this time.
    }
}