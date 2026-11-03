namespace BasicGameFrameworkLibrary.Blazor.Configuration;
public static class GameDisplayOptions
{


    

    public static EnumSize LabelSize { get; set; }
    = EnumSize.Standard;

    public static EnumWidth LabelWidth { get; set; }
        = EnumWidth.Standard;

    public static EnumSize ButtonSize { get; set; }
        = EnumSize.Standard;

    public static EnumWidth ButtonWidth { get; set; }
        = EnumWidth.Standard;


    public static bool UseSuggestedLabelWidth { get; set; } = false;
    public static bool UseSuggestedButtonWidth { get; set; } = false;

    //not sure if i need more flexibility.  if i do later, will later rethink.

    public static bool GetSuggestLabelWidth(bool? suggested)
    {
        if (suggested.HasValue)
        {
            return suggested.Value;
        }
        return UseSuggestedLabelWidth;
    }
    public static bool GetSuggestedButtonWidth(bool? suggested)
    {
        if (suggested.HasValue)
        {
            return suggested.Value;
        }
        return UseSuggestedButtonWidth;
    }
    public static EnumSize GetLabelSize(EnumSize? size)
    {
        if (size is not null)
        {
            return size.Value;
        }
        return LabelSize;
    }
    public static EnumWidth GetLabelWidth(EnumWidth? width)
    {
        if (width is not null)
        {
            return width.Value;
        }
        return LabelWidth;
    }
    public static EnumSize GetButtonSize(EnumSize? size)
    {
        if (size is not null)
        {
            return size.Value;
        }
        return ButtonSize;
    }
    public static EnumWidth GetButtonWidth(EnumWidth? width)
    {
        if (width is not null)
        {
            return width.Value;
        }
        return ButtonWidth;

    }
    public static EnumSize ChooserItemHeight { get; set; }
        = EnumSize.Standard;

    //private static EnumSize GetChooserItemHeight(EnumSize? size)
    //{
    //    return size ?? ChooserItemHeight;
    //}


    public static string GetChooserItemHeight(
    EnumSize? sizeCategory)
    {
        EnumGameLayoutMode layoutMode = ll1.LayoutMode;
        EnumSize size = sizeCategory ?? ChooserItemHeight;

        return layoutMode switch
        {
            EnumGameLayoutMode.Desktop => size switch
            {
                EnumSize.Standard => "100px",
                EnumSize.Large => "150px",
                _ => "60px"
            },
            EnumGameLayoutMode.Tablet => size switch
            {
                EnumSize.Standard => "60px",
                EnumSize.Large => "120px",
                _ => "40px"
            },
            _ => size switch
            {
                EnumSize.Large => "65px",
                EnumSize.Small => "30px",
                _ => "50px"
            }
        };
    }

}