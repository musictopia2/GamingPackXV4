namespace BasicGameFrameworkLibrary.Blazor.Shells;
public partial class LandscapeShell
{
    [CascadingParameter]
    private MediaQueryListComponent? Media { get; set; }
    private bool IsLandscape() => Media!.ScreenOrientation == EnumScreenOrientation.Landscape;
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}