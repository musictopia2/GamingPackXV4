namespace BasicGameFrameworkLibrary.Blazor.GameGraphics.Base;
public partial class FlexibleDeckRenderComponent<D>
    where D : class, IDeckObject
{
    [Parameter]
    public bool UseKey { get; set; } = true; //default to true.  if not using key, then don't specify the key (to fix problems with the duplicate key error).
    [Parameter]
    public D? DeckObject { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Parameter]
    public bool ConsiderEnabled { get; set; } = false; //most of the time, won't be considered.
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        if (DeckObject is not null)
        {
            DeckObject.ChangeSelectAction = () => InvokeAsync(StateHasChanged); //try here.
        }
    }
    private BasicDeckRecordModel? _previous;
    protected override void OnAfterRender(bool firstRender)
    {
        _previous = GetRecord();
    }
    private BasicDeckRecordModel GetRecord()
    {
        var record = DeckObject!.GetRecord;
        BasicDeckRecordModel output;
        if (ConsiderEnabled == false)
        {
            output = record with
            {
                IsEnabled = true
            };
        }
        else
        {
            output = record;
        }
        return output;
    }
    private bool CanRender => _previous != GetRecord();
}