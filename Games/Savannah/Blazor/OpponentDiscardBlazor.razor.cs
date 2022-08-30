namespace Savannah.Blazor;
public partial class OpponentDiscardBlazor
{
    [Parameter]
    public SavannahPlayerItem? Player { get; set; }
    [Parameter]
    public bool IsEnabled { get; set; } = false;
    [Parameter]
    public RenderFragment<RegularSimpleCard>? ChildContent { get; set; }
    private RegularSimpleCard? LastCard => Player!.DiscardList.Count == 0 ? null : Player.DiscardList.Last();
}