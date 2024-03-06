namespace SorryDicedGame.Core.Data;
public class WaitingModel
{
    //now has to allow nulls unfortunately.  has to fix the source generators to work with required.
    //has to see how the off the shelf one does it.
    public SorryDicedGamePlayerItem? Player { get; set; }
    //public int PlayerId { get; set; }
    public EnumColorChoice ColorUsed { get; set; }
}