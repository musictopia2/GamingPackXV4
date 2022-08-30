namespace MonasteryCardGame.Core.Data;
public class MissionList
{
    public string Description { get; set; } = "";
    public BasicList<SetInfo> MissionSets { get; set; } = new();
}