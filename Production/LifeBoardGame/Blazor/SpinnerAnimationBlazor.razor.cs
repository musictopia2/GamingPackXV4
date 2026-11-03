namespace LifeBoardGame.Blazor;
public partial class SpinnerAnimationBlazor
{
    [Parameter]
    [EditorRequired]
    public SpinnerViewModel? DataContext { get; set; }

    private BasicList<string> _matrixs = [];
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