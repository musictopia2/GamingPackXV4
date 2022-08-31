namespace Phase10.Blazor;
public partial class TempSetsBlazor
{
    [Parameter]
    public string TargetContainerSize { get; set; } = "";
    [Parameter]
    public TempSetsObservable<EnumColorTypes, EnumColorTypes, Phase10CardInformation>? TempPiles { get; set; }
}