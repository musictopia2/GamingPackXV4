namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Frames;
public partial class BaseFrameBlazor
{

    [Parameter, EditorRequired]
    public RenderFragment LegendContent { get; set; } = default!;

    [Parameter, EditorRequired]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public int HeightPixels { get; set; }

    //if i need width, needs to rethink.


    //[Parameter]
    //public string TargetHeight { get; set; } = "";
    [Parameter]
    public string TargetWidth { get; set; } = ""; //usually percents.
    [Parameter]
    public int PaddingHeight { get; set; } = 0;
    [Parameter]
    public bool IsEnabled { get; set; } = true;
    [Parameter]
    public bool FullFrame { get; set; } = false;
    private bool IsDisabled => !IsEnabled;
    private string GetColorStyle()
    {
        if (IsDisabled == false)
        {
            return "";
        }
        return $"color:{cs1.LightGray.ToWebColor}; border-color: {cs1.LightGray.ToWebColor};";
    }
    private string GetContainerStyle()
    {
        if (HeightPixels > 0 && TargetWidth == "")
        {
            return $"height: {HeightPixels}px;";
        }
        if (HeightPixels > 0 && TargetWidth != "")
        {
            return $"height: {HeightPixels}px; width: {TargetWidth};";
        }
        if (TargetWidth != "")
        {
            return $"width: {TargetWidth};";
        }
        return ""; //for now.
        //if (TargetHeight == "" && TargetWidth == "")
        //{
        //    return "";
        //}
        //if (TargetWidth == "")
        //{
        //    return $"height: {TargetHeight};";
        //}
        //if (TargetHeight == "")
        //{
        //    return $"width: {TargetWidth};";
        //}
        //return $"height: {TargetHeight};width: {TargetWidth};";
    }
    private string PaddingStyle()
    {
        if (PaddingHeight > 0)
        {
            return $"margin-right: {PaddingHeight}px;";
        }
        return "";
    }
    private string FrameClass
    {
        get
        {
            if (FullFrame)
            {
                return "fullframe";
            }
            return "";
        }
    }
}