namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.Hands;
public partial class BaseHandBlazor<D>
    where D : class, IDeckObject, new()
{
    [Parameter]
    public bool UseKey { get; set; } = true; //allow the possibility of setting to false to see if that helps for a game like payday.
    [Parameter]
    public bool Rotated { get; set; } = false; //maybe best to do this way.  not too often its rotated anyways.
    [Parameter]
    public RenderFragment<D>? ChildContent { get; set; }
    [Parameter]
    public HandObservable<D>? Hand { get; set; }
    [Parameter]
    public EnumHandList HandType { get; set; } = EnumHandList.Horizontal;
    [Parameter]
    public int PaddingRight { get; set; } = 0; //will be in pixels this time.
    [Parameter]
    public double Divider { get; set; } = 1;
    [Parameter]
    public double AdditionalSpacing { get; set; } = 0;
    [Parameter]
    public EnumSize? SizeCategory { get; set; }
    [Parameter]
    public bool Bold { get; set; } = false;
    private string GetLegendStyle()
    {
        if (Bold == false)
        {
            return "";
        }
        return "font-weight: bold;";
    }
    private string GetContentStyle()
    {
        var size = GetHandSize(Hand!.HandList.Count);

        return "position: relative;" +
            $" width: {Pixels(size.Width)};" +
            $" height: {Pixels(size.Height)};";
    }
    private (double Width, double Height) GetCardSize()
    {
        var original = new D().DefaultSize;
        const double borderWidth = 4;

        double targetSize = GetCardHeightPixels();
        double originalWidth = original.Width + borderWidth;
        double originalHeight = original.Height + borderWidth;

        double scale = targetSize /
            Math.Max(originalWidth, originalHeight);

        double width = originalWidth * scale;
        double height = originalHeight * scale;

        return Rotated
            ? (height, width)
            : (width, height);
    }

    private double GetCardStep()
    {
        var (Width, Height) = GetCardSize();

        double length = HandType == EnumHandList.Horizontal
            ? Width
            : Height;

        return Math.Max(
            1,
            length / Math.Max(1, Divider) + AdditionalSpacing);
    }

    private (double Width, double Height) GetHandSize(int count)
    {
        var (Width, Height) = GetCardSize();

        // Keep space for one card even when the hand is empty.
        double length = HandType == EnumHandList.Horizontal
            ? Width
            : Height;

        double total = length + GetCardStep() * Math.Max(0, count - 1);

        return HandType == EnumHandList.Horizontal
            ? (total, Height)
            : (Width, total);
    }

    private double GetCardHeightPixels()
    {
        if (HeightPixels > 0)
        {
            return HeightPixels;
        }

        if (TargetImageHeight > 0)
        {
            return ll1.GetHeight(TargetImageHeight);
        }

        throw new CustomBasicException("A card height must be supplied.");
    }

    //we are doing to do a viewbox.
    /// <summary>
    /// this is where you usually set a percentage which represents how high or wide the container is.
    /// if hand type is horizontal, then its the width
    /// otherwise, its the height
    /// </summary>
    [Parameter]
    public string TargetContainerSize { get; set; } = ""; //if not set, will keep going forever.
    /// <summary>
    /// this is where you set the percentage which represents how big the images are
    /// if horizontal is used, then its the height.  otherwise, its the width.
    /// 
    /// </summary>
    [Parameter]
    public int HeightPixels { get; set; }
    [CascadingParameter]
    public int TargetImageHeight { get; set; }


    [Parameter]
    public int LegendExtraWidth { get; set; } = 0; // in pixels, default is no extra width

    

    private bool IsDisabled => !Hand!.IsEnabled;
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
        if (Hand == null)
        {
            return "";
        }

        (double Width, double Height) size;

        if (Hand.Maximum > 0)
        {
            size = CalculateHandMax();
        }
        else if (Hand.HandList.Count <= 2)
        {
            size = GetMinimumSize();
        }
        else
        {
            size = GetHandSize(Hand.HandList.Count);
        }

        bool hasContainerSize =
            !string.IsNullOrWhiteSpace(TargetContainerSize);

        string common =
            "position: relative; box-sizing: content-box;" +
            $" padding-right: {PaddingRight}px;";

        if (HandType == EnumHandList.Horizontal)
        {
            string width = hasContainerSize
                ? TargetContainerSize
                : $"min({Pixels(size.Width + LegendExtraWidth)}, 95vw)";

            return common +
                $" width: {width};" +
                $" height: {Pixels(size.Height)};" +
                " padding-bottom: 20px;" +
                " overflow-x: auto; overflow-y: hidden;";
        }

        string height = hasContainerSize
            ? TargetContainerSize
            : Pixels(Math.Min(size.Height, ll1.GetHeight(95)));

        return common +
            $" width: {Pixels(size.Width + LegendExtraWidth)};" +
            $" height: {height};" +
            " padding-bottom: 20px;" +
            " overflow-y: auto; overflow-x: hidden;";
    }
    private string GetHeightWithNoContainerSize()
    {
        if (Hand == null || Hand.HandList.Count == 0)
        {
            return "auto";
        }

        double contentHeight = GetHandSize(Hand.HandList.Count).Height;
        double maximumHeight = ll1.GetHeight(95);

        double height = Math.Min(contentHeight, maximumHeight);

        return height.ToString(
            System.Globalization.CultureInfo.InvariantCulture) + "px";
    }
    private string GetWidthWithNoContainerSize()
    {
        if (Hand == null || Hand.HandList.Count == 0)
        {
            return "auto";
        }

        double contentWidth = GetHandSize(Hand.HandList.Count).Width;

        string pixels = contentWidth.ToString(
            System.Globalization.CultureInfo.InvariantCulture);

        return $"min({pixels}px, 95vw)";
    }
    private static string Pixels(double value)
    {
        return value.ToString(
            System.Globalization.CultureInfo.InvariantCulture) + "px";
    }

    private string GetOldContainerStyle()
    {
        var (Width, Height) = GetCardSize();

        string common =
            "position: relative; margin-right: 10px;" +
            $" padding-right: {PaddingRight}px;";

        if (HandType == EnumHandList.Horizontal)
        {
            string width = string.IsNullOrWhiteSpace(TargetContainerSize)
                ? GetWidthWithNoContainerSize()
                : TargetContainerSize;

            return common +
                $" width: {width};" +
                $" height: {Pixels(Height)};" +
                " overflow-x: auto; overflow-y: hidden;";
        }

        string height = string.IsNullOrWhiteSpace(TargetContainerSize)
            ? GetHeightWithNoContainerSize()
            : TargetContainerSize;

        return common +
            $" width: {Pixels(Width + LegendExtraWidth)};" +
            $" height: {height};" +
            " overflow-y: auto; overflow-x: hidden;";
    }
    private string GetCardStyle(int index)
    {
        var (Width, _) = GetCardSize();
        double offset = GetCardStep() * index;

        if (HandType == EnumHandList.Horizontal)
        {
            return "position: absolute; top: 0;" +
                $" left: {Pixels(offset)};" +
                $" width: {Pixels(Width)};";
        }

        return "position: absolute; left: 0;" +
            $" top: {Pixels(offset)};" +
            $" width: {Pixels(Width)};";
    }

    private (double Width, double Height) CalculateHandMax()
    {
        return GetHandSize(Hand?.Maximum ?? 0);
    }

    private (double Width, double Height) GetMinimumSize()
    {
        var (Width, Height) = GetCardSize();

        if (HandType == EnumHandList.Horizontal)
        {
            return (Width * 2, Height);
        }

        return (Width, Height * 2);
    }
    private async Task BoardClicked()
    {
        await Hand!.BoardSingleClickCommand!.ExecuteAsync(null);
    }
}