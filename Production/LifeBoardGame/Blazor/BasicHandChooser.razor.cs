namespace LifeBoardGame.Blazor;
public partial class BasicHandChooser<B>
    where B : BasicSubmitViewModel
{
    [Parameter]
    [EditorRequired]
    public B? DataContext { get; set; }
    [CascadingParameter]
    public LifeBoardGameVMData? GameContainer { get; set; }
}