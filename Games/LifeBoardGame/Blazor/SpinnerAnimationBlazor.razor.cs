using System.Reflection;
namespace LifeBoardGame.Blazor;
public partial class SpinnerAnimationBlazor
{
    [CascadingParameter]
    public SpinnerViewModel? DataContext { get; set; }
    private Assembly GetAssembly => Assembly.GetAssembly(GetType())!;

    private BasicList<string> _matrixs = new();
    protected override void OnInitialized()
    {
        _matrixs = Resources.ArrowMatrix.GetResource<BasicList<string>>();
        DataContext!.GameContainer.RefreshSpinner = ShowChange;
        base.OnInitialized();
    }
    private void ShowChange()
    {
        InvokeAsync(StateHasChanged);
    }
}