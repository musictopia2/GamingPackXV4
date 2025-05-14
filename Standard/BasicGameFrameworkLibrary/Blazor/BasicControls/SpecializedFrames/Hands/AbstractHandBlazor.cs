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
    /// <summary>
    /// this is where you set the percentage which represents how big the images are
    /// if horizontal is used, then its the height.  otherwise, its the width.
    /// 
    /// </summary>
    [Parameter]
    public string TargetImageSize { get; set; } = "";

    protected string RealHeight()
    {
        if (TargetImageSize != "")
        {
            return TargetImageSize;
        }
        if (ImageHeight > 0)
        {
            return $"{ImageHeight}vh";
        }
        throw new CustomBasicException("Cannot get real height");
    }
}
