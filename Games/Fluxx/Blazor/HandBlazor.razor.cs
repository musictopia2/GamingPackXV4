namespace Fluxx.Blazor;
public partial class HandBlazor<D>
       where D : FluxxCardInformation, new()
{
    [Parameter]
    public HandObservable<D>? Hand { get; set; }
    [Parameter]
    public EnumHandList HandType { get; set; } = EnumHandList.Horizontal;
    [Parameter]
    public double Divider { get; set; } = 1;
    [Parameter]
    public double AdditionalSpacing { get; set; } = -5;
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
}