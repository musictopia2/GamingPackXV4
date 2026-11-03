namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SimpleControls.Labels;

public partial class GameLabel
{
    [Parameter]
    public RenderFragment ChildContent { get; set; } = null!;

    [Parameter]
    public bool StartsOnNewLine { get; set; }


    [Parameter]
    public bool? UseSuggestedWidth { get; set; } = null;


    [Parameter]
    public EnumSize? SizeCategory { get; set; }
    [Parameter]
    public EnumWidth? WidthCategory { get; set; }
    private string GetFontSize()
    {
        EnumSize size = gg1.GetLabelSize(SizeCategory);
        if (ll1.LayoutMode == EnumGameLayoutMode.Desktop)
        {
            if (size == EnumSize.Standard)
            {
                return "2.0rem";
            }
            if (size == EnumSize.Large)
            {
                return "3.0rem";
            }
            return "1.3rem";
        }
        if (ll1.LayoutMode == EnumGameLayoutMode.Tablet)
        {
            if (size == EnumSize.Standard)
            {
                return "1.5rem";
            }
            if (size == EnumSize.Large)
            {
                return "2.0rem";
            }
            return "1.2rem";
        }
        if (size == EnumSize.Large)
        {
            return "1.5rem";
        }
        if (size == EnumSize.Small)
        {
            return "0.9rem";
        }
        return "1.1rem";
    }
    //for now, no extra widths.   if i ever need to tweak a little further, then rethink.

    private string GetWidth()
    {
        double width;
        EnumWidth category = gg1.GetLabelWidth(WidthCategory);
        if (ll1.LayoutMode == EnumGameLayoutMode.Desktop)
        {
            width = category switch
            {
                EnumWidth.Narrow => 96,
                EnumWidth.Wide => 300,
                _ => 200
            };
        }
        else if (ll1.LayoutMode == EnumGameLayoutMode.Tablet)
        {
            width = category switch
            {
                EnumWidth.Narrow => 80,
                EnumWidth.Wide => 200,
                _ => 150
            };
        }
        else
        {
            width = category switch
            {
                EnumWidth.Narrow => 64,
                EnumWidth.Wide => 150,
                _ => 96
            };
        }
        EnumSize size = gg1.GetLabelSize(SizeCategory);
        if (size == EnumSize.Large)
        {
            width *= 1.5;
        }
        if (size == EnumSize.Small)
        {
            width *= .80;
        }

        return $"{(int)Math.Ceiling(width)}px";
    }

    private string GetStyle()
    {
        string widths = "";

        bool useSuggestion = gg1.GetSuggestLabelWidth(UseSuggestedWidth);

        if (useSuggestion)
        {
            widths = $"width: {GetWidth()};";
        }


        string fonts = $"font-size: {GetFontSize()};";
        if (useSuggestion)
        {
            return $"{widths} {fonts}";
        }
        return fonts;
    }
}