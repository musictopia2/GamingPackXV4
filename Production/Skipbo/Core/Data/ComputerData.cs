namespace Skipbo.Core.Data;
public class ComputerData
{
    public EnumCardType WhichType { get; set; }
    public int Pile { get; set; }
    public SkipboCardInformation? ThisCard { get; set; }
    public int Discard { get; set; }
}