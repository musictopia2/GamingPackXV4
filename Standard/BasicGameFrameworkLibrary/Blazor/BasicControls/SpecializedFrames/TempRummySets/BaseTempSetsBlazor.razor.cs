namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.TempRummySets;
public partial class BaseTempSetsBlazor<SU, CO, RU>
    where SU : IFastEnumSimple
    where CO : IFastEnumColorSimple
    where RU : class, IRummmyObject<SU, CO>, IDeckObject, new()
{
    [Parameter]
    public RenderFragment<TempSetInfoBlazor<SU, CO, RU>>? ChildContent { get; set; }
    [Parameter]
    public double Divider { get; set; } = 1;
    [Parameter]
    public double AdditionalSpacing { get; set; } = -5;
    [Parameter]
    public string TargetContainerSize { get; set; } = ""; //if not set, will keep going forever.
    [Parameter]
    public TempSetsObservable<SU, CO, RU>? TempPiles { get; set; }
    private static TempSetInfoBlazor<SU, CO, RU> GetTempInfo(RU image, HandObservable<RU> hand)
    {
        TempSetInfoBlazor<SU, CO, RU> output = new()
        {
            Image = image,
            Hand = hand
        };
        return output;
    }
}