namespace SorryDicedGame.Core.Data;
public class WaitingModel
{
    public required SorryDicedGamePlayerItem Player { get; set; }
    //public int PlayerId { get; set; }
    public EnumColorChoice ColorUsed { get; set; }

}