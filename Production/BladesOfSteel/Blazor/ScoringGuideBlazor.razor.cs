namespace BladesOfSteel.Blazor;
public partial class ScoringGuideBlazor
{
    private bb1? _data;
    private BasicList<string> _offenseList = new();
    private BasicList<string> _defenseList = new();
    protected override void OnInitialized()
    {
        _data = new();
        _offenseList = bb1.OffenseList();
        _defenseList = bb1.DefenseList();
        base.OnInitialized();
    }
    private static string GetColumns()
    {
        return $"{gg1.Auto} {gg1.Auto}";
    }
}