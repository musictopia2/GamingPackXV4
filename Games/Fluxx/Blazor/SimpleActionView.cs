namespace Fluxx.Blazor;
public class SimpleActionView : KeyComponentBase
{
    [CascadingParameter]
    public CompleteContainerClass? CompleteContainer { get; set; }
}