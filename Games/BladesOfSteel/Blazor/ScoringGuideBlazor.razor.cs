namespace BladesOfSteel.Blazor;
public partial class ScoringGuideBlazor
{
    private bb? _data;
    private BasicList<string> _offenseList = new();
    private BasicList<string> _defenseList = new();
    protected override void OnInitialized()
    {
        _data = new();
        _offenseList = bb.OffenseList();
        _defenseList = bb.DefenseList();
        base.OnInitialized();
    }
    private static string GetColumns()
    {
        return $"{gg.Auto} {gg.Auto}";
    }
}