namespace TileRummy.Blazor;
public partial class TileRummyTempSetBlazor
{
    [Parameter]
    public string TargetContainerSize { get; set; } = "";
    [Parameter]
    public TempSetsObservable<EnumColorType, EnumColorType, TileInfo>? TempPiles { get; set; }
}