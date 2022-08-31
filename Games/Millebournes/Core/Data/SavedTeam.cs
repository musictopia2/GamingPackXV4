namespace Millebournes.Core.Data;
public class SavedTeam
{
    public BasicList<MillebournesCardInformation> PreviousList { get; set; } = new();
    public BasicList<BasicPileInfo<MillebournesCardInformation>> SavedPiles { get; set; } = new();
    public BasicList<SafetyInfo> SafetyList { get; set; } = new();
    public int Wrongs { get; set; }
    public EnumHazardType CurrentHazard { get; set; }
    public bool CurrentSpeed { get; set; }
    public int Miles { get; set; }
    public int TotalScore { get; set; }
    public MillebournesCardInformation? CurrentCard { get; set; }
    public int Number200s { get; set; }
}