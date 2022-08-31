namespace Flinch.Core.Data;
public class ComputerData
{
    public EnumCardType WhichType { get; set; }
    public int Pile { get; set; }
    public FlinchCardInformation? CardToPlay { get; set; }
    public int Discard { get; set; }
}