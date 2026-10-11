namespace MonopolyDicedGame.Blazor;
public partial class HouseAnimationComponent : IDisposable
{
    [CascadingParameter]
    public MonopolyDicedGameMainViewModel? DataContext { get; set; }
    [Parameter]
    [EditorRequired]
    public string ImageHeight { get; set; } = "";
    [Parameter]
    [EditorRequired]
    public HouseDice? HouseDice { get; set; }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
        DataContext!.CommandContainer.RemoveSpecificAction("housedice");
    }

    protected override void OnInitialized()
    {
        DataContext!.CommandContainer.AddSpecificAction("housedice", ShowChange);
        base.OnInitialized();
    }
    private void ShowChange()
    {
        InvokeAsync(StateHasChanged);
    }
}