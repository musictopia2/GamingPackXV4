namespace Pinochle2Player.Blazor;
public partial class GuideUI
{
    [Parameter]
    public Pinochle2PlayerVMData? GameData { get; set; }
    private static string GetRows => bb1.RepeatAuto(20);
}