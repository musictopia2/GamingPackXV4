namespace BasicGameFrameworkLibrary.Blazor.Views;
public abstract partial class BasicSubmitView<VM>
    where VM : class, ISubmitText
{
    [CascadingParameter]
    public VM? DataContext { get; set; }
    private string GetText => DataContext!.Text;
    [Parameter]
    public string FontSize { get; set; } = "3vh";
    protected virtual ICustomCommand Command => DataContext!.Command;
}