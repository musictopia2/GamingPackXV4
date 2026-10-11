namespace SequenceDice.Core.Data;

[UseStateChanged]
public partial class SpaceInfoCP : IBasicSpace
{
    public Vector Vector { get; set; }
    [StateProperty]
    public partial int Player { get; set; }
    [StateProperty]
    public partial string Color { get; set; } = cs1.Transparent;
    [StateProperty]
    public partial bool WasRecent { get; set; }
    public int Number { get; set; }
    public void ClearSpace()
    {
        Player = 0;
        Color = cs1.Transparent;
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