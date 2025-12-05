namespace MonopolyDicedGame.Blazor;
public partial class MiscSpaceComponent
{
    [Parameter]
    public BasicList<EnumMiscType> MiscList { get; set; } = [];

    [Parameter]
    public string TargetImageHeight { get; set; } = "";


    //has to repeat 3 times

    private EnumMiscType GetType(int index)
    {
        if (index > MiscList.Count)
        {
            return EnumMiscType.None;
        }
        return MiscList[index -1];
    }
    private static string GetActionColor => cc1.Yellow.ToWebColor;
    private static Vector GetSpaceInfo(int index)
    {
        return new(5, index + 2);
    }
}