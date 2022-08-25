namespace Froggies.Blazor.Views;
internal record LilyRecord(bool HasFrog, bool IsSelected, bool IsTarget);
public partial class LilyPadBlazor
{
    private LilyRecord? _previous;
    protected override void OnAfterRender(bool firstRender)
    {
        _previous = GetRecord;
        base.OnAfterRender(firstRender);
    }
    private LilyRecord GetRecord => new(Space!.HasFrog, Space.IsSelected, Space.IsTarget);
    protected override bool ShouldRender() //still fires it.  but somehow is rendering anyways.
    {
        if (_previous is null)
        {
            return true;
        }
        return _previous!.Equals(GetRecord) == false;
    }
    [Parameter]
    public LilyPadModel? Space { get; set; }
    private string GetStrokeInfo()
    {
        if (Space!.IsSelected)
        {
            return "stroke: red; stroke-width: 3px";
        }
        return "stroke: lime; stroke-width: 3px; stroke-dasharray:2 2;";
    }
}