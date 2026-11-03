namespace Risk.Blazor;
public partial class RiskHandBlazor
{
    [Parameter]
    public HandObservable<RiskCardInfo>? Hand { get; set; }
}