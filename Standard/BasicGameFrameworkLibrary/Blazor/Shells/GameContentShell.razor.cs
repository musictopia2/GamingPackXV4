namespace BasicGameFrameworkLibrary.Blazor.Shells;

public partial class GameContentShell
{
    [CascadingParameter]
    public IGameInfo GameData { get; set; } = null!;
    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    [CascadingParameter]
    private MediaQueryListComponent Media { get; set; } = null!;
    private bool IsSupported()
    {
        if (Media.DeviceCategory == EnumDeviceCategory.Phone && GameData.SupportsCompactLayout == false)
        {
            return false;
        }
        return true;
    }

    protected override void OnInitialized()
    {
        if (Media.DeviceSize == EnumDeviceSize.LargePhone || Media.DeviceSize == EnumDeviceSize.SmallPhone || Media.DeviceSize == EnumDeviceSize.SmallTablet)
        {
            ll1.LayoutMode = EnumGameLayoutMode.Compact;
        }
        if (Media.DeviceSize == EnumDeviceSize.LargeTablet)
        {
            ll1.LayoutMode = EnumGameLayoutMode.Tablet;
        }
        if (Media.DeviceSize == EnumDeviceSize.Desktop)
        {
            ll1.LayoutMode = EnumGameLayoutMode.Desktop;
        }
        base.OnInitialized();
    }

}