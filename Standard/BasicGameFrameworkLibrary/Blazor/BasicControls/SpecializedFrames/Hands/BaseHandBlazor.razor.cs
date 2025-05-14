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
    public string FontSizeForFrame { get; set; } = "";
    [Parameter]
    public bool Bold { get; set; } = false;

    private string GetLegendStyle()
    {
        if (FontSizeForFrame == "" && Bold == false)
        {
            return "";
        }
        if (Bold == false)
        {
            return $"font-size: {FontSizeForFrame};"; //so you can decide how it would be.
        }
        if (FontSizeForFrame != "")
        {
            return $"font-size: {FontSizeForFrame}; font-weight: bold;";
        }
        return "font-weight: bold;";
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
    public string TargetImageSize { get; set; } = "";
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
        return $"color:{cs1.LightGray.ToWebColor()}; border-color: {cs1.LightGray.ToWebColor()};";
    }
    private string GetContainerStyle()
    {
        double widthVH = 0, heightVH = 0;

        if (Hand?.HandList.Count <= 2 && TargetContainerSize == "")
        {
            if ((Hand?.Maximum ?? 0) == 0)
            {
                (widthVH, heightVH) = GetMinimumSize();
            }
            else
            {
                (widthVH, heightVH) = CalculateHandMax();
            }
        }
        else if ((Hand?.Maximum ?? 0) > 0)
        {
            (widthVH, heightVH) = CalculateHandMax();
        }
        // else: let CSS auto-size based on content

        string style = "position: relative; overflow: auto;";
        if (widthVH == 0 && heightVH == 0)
        {
            return GetOldContainerStyle();
        }
        if (HandType == EnumHandList.Horizontal)
        {
            if (widthVH > 0)
            {
                //double extraWidthVH = 0;
                if (LegendExtraWidth > 0)
                {
                    // If your layout is in vh, convert px to vh based on viewport height
                    // 1vh = 1% of viewport height, so: px / window.innerHeight * 100
                    // For simplicity, you can approximate or use JS interop for exact
                    // Here, just add as px for demonstration
                    style += $" width: calc({widthVH}vh + {LegendExtraWidth}px);";
                }
                else
                {
                    style += $" width: {widthVH}vh;";
                }
            }

            if (heightVH > 0)
            {
                style += $" height: {heightVH}vh;";
            }
            style += "padding-bottom: 3vh;";
        }
        else
        {
            if (heightVH > 0)
            {
                style += $" height: {heightVH}vh;";
            }

            if (widthVH > 0)
            {
                style += $" width: {widthVH}vh;";
            }
        }
        style += $" padding-right: {PaddingRight}px;";
        return style;
    }
    private string GetHeightWithNoContainerSize()
    {
        if (Hand == null || Hand.HandList.Count == 0)
        {
            return "auto";
        }

        var card = new D();
        var defaultSize = card.DefaultSize;
        double cardHeight = TargetImageHeight > 0
            ? TargetImageHeight
            : double.TryParse(TargetImageSize.Replace("vh", ""), out var h) ? h : defaultSize.Height;

        double aspectRatio = defaultSize.Width / defaultSize.Height;
        double cardWidth = cardHeight * aspectRatio;
        double spacing = AdditionalSpacing;
        double divider = Divider <= 0 ? 1 : Divider;

        double extras;
        if (Rotated)
        {
            // When rotated, the "height" is actually the width of the card
            extras = divider <= 1 ? cardWidth + spacing : (cardWidth / divider) + spacing;
        }
        else
        {
            extras = divider <= 1 ? cardHeight + spacing : (cardHeight / divider) + spacing;
        }

        int count = Hand.HandList.Count;
        double totalHeightVH;
        if (Rotated)
        {
            totalHeightVH = (extras * (count - 1)) + cardWidth + 1;
        }
        else
        {
            totalHeightVH = (extras * (count - 1)) + cardHeight + 1;
        }

        // Cap at 95vh
        string heightStyle;
        if (totalHeightVH > 95)
        {
            heightStyle = "95vh";
        }
        else
        {
            heightStyle = $"{totalHeightVH}vh";
        }

        return heightStyle;
    }
    private string GetWidthWithNoContainerSize()
    {
        if (Hand == null || Hand.HandList.Count == 0)
        {
            return "auto";
        }

        var card = new D();
        var defaultSize = card.DefaultSize;
        double aspectRatio = defaultSize.Width / defaultSize.Height;
        double cardHeight = TargetImageHeight > 0
            ? TargetImageHeight
            : double.TryParse(TargetImageSize.Replace("vh", ""), out var h) ? h : defaultSize.Height;

        double cardWidth = cardHeight * aspectRatio;
        double spacing = AdditionalSpacing;
        double divider = Divider <= 0 ? 1 : Divider;

        double extras = divider <= 1 ? cardWidth + spacing : (cardWidth / divider) + spacing;
        int count = Hand.HandList.Count;
        double totalWidthVH = (extras * (count - 1)  +10);

        //string widthStyle = $"{totalWidthVH}vh";

        //// Cap at 95vw
        string widthStyle;
        if (totalWidthVH > 150)
        {
            //for now, set at 150
            widthStyle = "95vw";
        }
        else
        {
            widthStyle = $"{totalWidthVH}vh";
        }

        return widthStyle;
    }
    private string GetOldContainerStyle()
    {
        string realSize;
        if (TargetImageSize != "")
        {
            realSize = TargetImageSize;
        }
        else if (TargetImageHeight > 0)
        {
            realSize = $"{TargetImageHeight}vh";
        }
        else
        {
            throw new CustomBasicException("Unable to calculate container");
        }
        //TargetContainerSize = "50vw"; //for now.
        if (TargetContainerSize == "")
        {
            if (HandType == EnumHandList.Horizontal)
            {
                //figure out width.
                string widths = GetWidthWithNoContainerSize();
                //widths = "50vw";
                return $"position: relative; overflow-x: auto; overflow-y: hidden; width: {widths}; hidden; margin-right: 10px; height: {realSize}; padding-bottom: 3vh; padding-right: {PaddingRight}px;";
            }
            //iffy now.
            
            string heights = GetHeightWithNoContainerSize();
            if (LegendExtraWidth == 0)
            {
                return $"position: relative; overflow-x: auto; overflow-y: hidden; height: {heights}; hidden; margin-right: 10px; width: {realSize};  padding-right: {PaddingRight}px;";
            }
            string widthValue;
            widthValue = $"calc({realSize} + {LegendExtraWidth}px)";
            return $"position: relative; overflow-x: auto; overflow-y: hidden; width: {widthValue}; margin-right: 10px; height: {heights}; padding-bottom: 3vh; padding-right: {PaddingRight}px;";
        }
        if (HandType == EnumHandList.Horizontal)
        {
            return $"position: relative; width: {TargetContainerSize}; padding-bottom: 4vh; height: {realSize}; overflow-x: auto; overflow-y: hidden;";
        }
        else
        {
            if (Rotated == false)
            {
                return $"position: relative; height: {TargetContainerSize}; width: {realSize}; overflow-y: auto; overflow-x: hidden; padding-right: {PaddingRight}px;";
            }
            //iffy now.
            int cardHeight;
            if (TargetImageHeight > 0)
            {
                cardHeight = TargetImageHeight;
            }
            else
            {
                if (TargetImageSize.ToLower().EndsWith("vh") == false)
                {
                    throw new CustomBasicException("Must end with view height for now");
                }
                string tempValue = TargetImageSize.ToLower().Replace("vh", "");
                cardHeight = int.Parse(tempValue);
            }
            cardHeight += 2;
            return $"position: relative; height: {TargetContainerSize}; width: {cardHeight}vh; overflow-y: auto; overflow-x: hidden; padding-right: {PaddingRight}px;";
        }
    }
    private string GetCardStyle(int index)
    {
        //this should not channge.
        int cardHeight;
        if (TargetImageHeight > 0)
        {
            cardHeight = TargetImageHeight;
        }
        else
        {
            if (!TargetImageSize.ToLower().EndsWith("vh"))
            {
                throw new CustomBasicException("Must end with view height for now");
            }
            string tempValue = TargetImageSize.ToLower().Replace("vh", "");
            cardHeight = int.Parse(tempValue);
        }
        double realSpacing;
        if (AdditionalSpacing <= 0)
        {
            realSpacing = -1;
        }
        else if (AdditionalSpacing == 1)
        {
            realSpacing = 0;
        }
        else
        {
            realSpacing = AdditionalSpacing;
        }
        if (HandType == EnumHandList.Horizontal)
        {
            var card = new D();
            double aspectRatio = card.DefaultSize.Width / card.DefaultSize.Height;
            double cardWidthVH = cardHeight * aspectRatio;

            double spacingVH = Divider <= 1
                ? cardWidthVH + realSpacing
                : (cardWidthVH / Divider) + realSpacing;

            double leftVH = spacingVH * index;
            return $"position: absolute; top: 0; left: {leftVH}vh; width: {cardWidthVH}vh;";
        }

        // For vertical layout:
        double spacingForTopVH;

        if (Rotated)
        {
            var card = new D();
            double aspectRatio = card.DefaultSize.Width / card.DefaultSize.Height;
            double rotatedHeightVH = cardHeight * aspectRatio;

            spacingForTopVH = Divider <= 1
                ? rotatedHeightVH + realSpacing
                : (rotatedHeightVH / Divider) + realSpacing;
        }
        else
        {
            spacingForTopVH = Divider <= 1
                ? cardHeight + realSpacing
                : (cardHeight / Divider) + realSpacing;
        }

        double topVH = spacingForTopVH * index;
        return $"position: absolute; top: {topVH}vh; left: 0;";
    }

    private (double widthVH, double heightVH) CalculateHandMax()
    {
        var card = new D();
        var defaultSize = card.DefaultSize;
        int max = Hand?.Maximum ?? 0;
        double divider = Divider <= 0 ? 1 : Divider;
        double spacing = AdditionalSpacing;

        if (HandType == EnumHandList.Horizontal)
        {
            double aspectRatio = defaultSize.Width / defaultSize.Height;
            double cardHeight = TargetImageHeight > 0
                ? TargetImageHeight
                : double.TryParse(TargetImageSize.Replace("vh", ""), out var h) ? h : defaultSize.Height;

            double cardWidth = cardHeight * aspectRatio;
            double extras = divider <= 1 ? cardWidth + spacing : (cardWidth / divider) + spacing;
            // The last card's right edge is at extras * (max - 1) + cardWidth
            double totalWidth = (extras * (max - 1)) + cardWidth;
            totalWidth += 1;
            cardHeight += 1;
            return (totalWidth, cardHeight);
        }
        else
        {
            double cardHeight = TargetImageHeight > 0
                ? TargetImageHeight
                : double.TryParse(TargetImageSize.Replace("vh", ""), out var h) ? h : defaultSize.Height;

            double extras = divider <= 1 ? cardHeight + spacing : (cardHeight / divider) + spacing;
            // The last card's bottom edge is at extras * (max - 1) + cardHeight
            double totalHeight = (extras * (max - 1)) + cardHeight;
            double cardWidth = defaultSize.Width / defaultSize.Height * cardHeight;
            cardWidth += 1;
            totalHeight += 1;
            return (cardWidth, totalHeight);
        }
    }

    private (double widthVH, double heightVH) GetMinimumSize()
    {
        var card = new D();
        var defaultSize = card.DefaultSize;
        double cardHeight = TargetImageHeight > 0
            ? TargetImageHeight
            : double.TryParse(TargetImageSize.Replace("vh", ""), out var h) ? h : defaultSize.Height;

        double aspectRatio = defaultSize.Width / defaultSize.Height;
        double cardWidth = cardHeight * aspectRatio;
       
        if (HandType == EnumHandList.Horizontal)
        {
            return (cardWidth * 3, cardHeight);
        }
        else
        {
            return (cardWidth, cardHeight * 3);
        }
    }
    private async Task BoardClicked()
    {
        await Hand!.BoardSingleClickCommand!.ExecuteAsync(null);
    }
    
    
}