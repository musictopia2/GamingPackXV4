namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.CardBoards;
public partial class TextCardBoardBlazor<D>
    where D : class, IDeckObject, new()
{
    [Parameter]
    public GameBoardObservable<D>? DataContext { get; set; }
    [Parameter]
    public string TargetWidth { get; set; } = "95%";
    [Parameter]
    public RenderFragment<D>? ChildContent { get; set; }
    [CascadingParameter]
    private MediaQueryListComponent? Media { get; set; }
    private string GetFontStyle
    {
        get
        {
            string output;
            if (Media!.DeviceCategory == EnumDeviceCategory.Phone)
            {
                output = ".8rem";
            }
            else
            {
                output = "2rem";
            }
            return $"font-size: {output};";
        }
    }
}