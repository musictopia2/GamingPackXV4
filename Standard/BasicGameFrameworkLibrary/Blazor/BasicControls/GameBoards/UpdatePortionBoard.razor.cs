namespace BasicGameFrameworkLibrary.Blazor.BasicControls.GameBoards;
public partial class UpdatePortionBoard
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    protected override void OnInitialized()
    {
        RepaintEventModel.UpdatePartOfBoard = ShowChange;
        base.OnInitialized();
    }
    private void ShowChange()
    {
        InvokeAsync(StateHasChanged);
    }
}