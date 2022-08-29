namespace ClueBoardGame.Core.Data;
public class DetectiveInfo
{
    public string Name { get; set; } = "";
    public EnumCardType Category { get; set; }
    public bool IsChecked { get; set; }
}