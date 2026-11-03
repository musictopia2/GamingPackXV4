namespace Millebournes.Blazor;
public partial class SafetiesBlazor
{
    [CascadingParameter]
    public MillebournesGameContainer? GameContainer { get; set; }
    [CascadingParameter]
    public MillebournesMainViewModel? DataContext { get; set; }
    [Parameter]
    public TeamCP? Team { get; set; }
    [Parameter]
    public int Index { get; set; }
    private ICustomCommand SafetyCommand => DataContext!.SafetyClickCommand!;
}