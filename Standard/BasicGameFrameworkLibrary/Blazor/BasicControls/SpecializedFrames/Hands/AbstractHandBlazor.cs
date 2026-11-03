namespace BasicGameFrameworkLibrary.Blazor.BasicControls.SpecializedFrames.Hands;
public abstract class AbstractHandBlazor<D> : KeyComponentBase
    where D: class, IDeckObject, new()
{

    [Parameter]
    public bool UseKey { get; set; } = true;
    [Parameter]
    public bool Rotated { get; set; } = false;

    [CascadingParameter]
    public int ImageHeight { get; set; }
    [Parameter]
    public HandObservable<D>? Hand { get; set; }

    [Parameter]
    public EnumHandList HandType { get; set; } = EnumHandList.Horizontal;
    [Parameter]
    public EnumSize? SizeCategory { get; set; }
    [Parameter]
    public double Divider { get; set; } = 1;
    [Parameter]
    public string FontSizeForFrame { get; set; } = "";
    [Parameter]
    public bool Bold { get; set; } = false;

    [Parameter]
    public double AdditionalSpacing { get; set; } = 0;

    /// <summary>
    /// this is where you usually set a percentage which represents how high or wide the container is.
    /// if hand type is horizontal, then its the width
    /// otherwise, its the height
    /// </summary>
    [Parameter]

    public string TargetContainerSize { get; set; } = ""; //if not set, will keep going forever.
    [Parameter]
    public int HeightPixels { get; set; }
    [Parameter]
    public int LegendExtraWidth { get; set; } = 0; // in pixels, default is no extra width

    protected int RealHeight()
    {
        if (HeightPixels > 0)
        {
            return HeightPixels;
        }
        if (ImageHeight > 0)
        {
            return ll1.GetHeight(ImageHeight);
        }
        throw new CustomBasicException("Cannot get real height");
    }
}