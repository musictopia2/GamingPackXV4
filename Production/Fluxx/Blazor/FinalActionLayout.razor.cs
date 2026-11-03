namespace Fluxx.Blazor;
public partial class FinalActionLayout
{
    [CascadingParameter]
    public CompleteContainerClass? CompleteContainer { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    private static string FirstGridColumns => "33vw 33vw auto";
}