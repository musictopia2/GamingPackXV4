namespace BuncoDiceGame.Core.Data;
[UseLabelGrid]
public partial class StatisticsInfo
{
    [LabelColumn]
    public string Turn { get; set; } = "None";
    [LabelColumn]
    public int NumberToGet { get; set; }
    [LabelColumn]
    public int Set { get; set; }
    [LabelColumn]
    public int YourTeam { get; set; }
    [LabelColumn]
    public int YourPoints { get; set; }
    [LabelColumn]
    public int OpponentScore { get; set; }
    [LabelColumn]
    public int Buncos { get; set; }
    [LabelColumn]
    public int Wins { get; set; }
    [LabelColumn]
    public int Losses { get; set; }
    [LabelColumn]
    public int YourTable { get; set; }
    [LabelColumn]
    public string TeamMate { get; set; } = "None";
    [LabelColumn]
    public string Opponent1 { get; set; } = "None";
    [LabelColumn]
    public string Opponent2 { get; set; } = "None";
    [LabelColumn]
    public string Status { get; set; } = "Disconnected";
}
