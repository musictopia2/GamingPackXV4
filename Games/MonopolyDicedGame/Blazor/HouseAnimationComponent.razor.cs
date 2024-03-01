namespace MonopolyDicedGame.Blazor;
public partial class HouseAnimationComponent
{
    [CascadingParameter]
    public MonopolyDicedGameMainViewModel? DataContext { get; set; }
    [Parameter]
    [EditorRequired]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    [EditorRequired]
    public HouseDice? HouseDice { get; set; }
    protected override void OnInitialized()
    {
        DataContext!.CommandContainer.AddAction(ShowChange, "housedice");
        base.OnInitialized();
    }
    private void ShowChange()
    {
        InvokeAsync(StateHasChanged);
    }
}