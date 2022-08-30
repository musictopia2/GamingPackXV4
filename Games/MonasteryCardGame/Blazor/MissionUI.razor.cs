namespace MonasteryCardGame.Blazor;
public partial class MissionUI
{
    [Parameter]
    public string MissionColumnWidth { get; set; } = "";
    [Parameter]
    public BasicList<MissionList>? Missions { get; set; }
    [CascadingParameter]
    public MonasteryCardGameMainViewModel? DataContext { get; set; }
    private ICustomCommand DescriptionCommand => DataContext!.MissionDetailCommand!;
    private ICustomCommand SelectCommand => DataContext!.SelectPossibleMissionCommand!;
    private ICustomCommand CompleteCommand => DataContext!.CompleteChosenMissionCommand!;
    private static string GetRowsColumn => bb.RepeatAuto(3); //to be 3 by 3.
    private string BackgroundColor(MissionList mission)
    {
        if (mission.Description == DataContext!.MissionChosen)
        {
            return cc.LimeGreen;
        }
        return cc.Aqua;
    }
}
