namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls;
public partial class BaseFrameBlazor
{
    [Parameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public string TargetWidth { get; set; } = "";
    [Parameter]
    public int PaddingHeight { get; set; } = 0;
    [Parameter]
    public string Text { get; set; } = ""; //they have to populate the text as well.
    [Parameter]
    public bool IsEnabled { get; set; } = true;
    [Parameter]
    public bool FullFrame { get; set; } = false;
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    private bool IsDisabled => !IsEnabled;
    private string GetColorStyle()
    {
        if (IsDisabled == false)
        {
            return "";
        }
        return $"color:{cs.LightGray.ToWebColor()}; border-color: {cs.LightGray.ToWebColor()};";
    }
    private string GetContainerStyle()
    {
        if (TargetHeight == "" && TargetWidth == "")
        {
            return "";
        }
        if (TargetWidth == "")
        {
            return $"height: {TargetHeight};";
        }
        if (TargetHeight == "")
        {
            return $"width: {TargetWidth};";
        }
        return $"height: {TargetHeight};width: {TargetWidth};";
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