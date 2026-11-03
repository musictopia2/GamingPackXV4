namespace BasicGameFrameworkLibrary.Blazor.Views;
public abstract partial class BasicSubmitView<VM>
    where VM : class, ISubmitText
{
    [Parameter]
    [EditorRequired]
    public VM? DataContext { get; set; }
    private string GetText => DataContext!.Text;
    [Parameter]
    public EnumSize Size { get; set; }
    protected virtual ICustomCommand Command => DataContext!.Command;
}