namespace LifeBoardGame.Blazor;
public partial class BasicPlayerPicker<B>
    where B : BasicSubmitViewModel
{
    [CascadingParameter]
    public B? DataContext { get; set; }

    [CascadingParameter]
    public LifeBoardGameVMData? GameContainer { get; set; }
}