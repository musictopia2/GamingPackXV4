namespace FiveCrowns.Blazor;
public partial class TempSetsBlazor
{
    [Parameter]
    public double HeightPercent { get; set; }
    [Parameter]
    public TempSetsObservable<EnumSuitList, EnumColorList, FiveCrownsCardInformation>? TempPiles { get; set; }
}