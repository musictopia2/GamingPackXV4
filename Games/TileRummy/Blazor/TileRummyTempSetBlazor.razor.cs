namespace TileRummy.Blazor;
public partial class TileRummyTempSetBlazor
{
    [Parameter]
    public double HeightPercent { get; set; } 
    [Parameter]
    public TempSetsObservable<EnumColorType, EnumColorType, TileInfo>? TempPiles { get; set; }
}