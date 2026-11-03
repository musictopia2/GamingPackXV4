namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Labels;
public partial class CompactLabel
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;
    [Parameter]
    public EnumSize? SizeCategory { get; set; }

    private string GetFontSize()
    {
        EnumSize size = gg1.GetLabelSize(SizeCategory);


        if (size == EnumSize.Standard)
        {
            return "1.0rem";
        }
        if (size == EnumSize.Large)
        {
            return "2.0rem";
        }
        return "0.8rem";

    }

    private string GetStyle()
    {
        string fonts = $"font-size: {GetFontSize()};";
        return fonts;
    }

}