namespace FiveCrowns.Blazor;
public partial class TempSetsBlazor
{
    [Parameter]
    public string TargetContainerSize { get; set; } = "";
    [Parameter]
    public TempSetsObservable<EnumSuitList, EnumColorList, FiveCrownsCardInformation>? TempPiles { get; set; }
}