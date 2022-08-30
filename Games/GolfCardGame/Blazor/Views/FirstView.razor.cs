namespace GolfCardGame.Blazor.Views;
public partial class FirstView
{
    [CascadingParameter]
    public GolfCardGameVMData? VMData { get; set; }
    [CascadingParameter]
    public FirstViewModel? DataContext { get; set; }
    private ICustomCommand ChooseCommand => DataContext?.ChooseFirstCardsCommand!;
    private static string ChooseMethod => nameof(FirstViewModel.ChooseFirstCardsAsync);
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Instructions", nameof(FirstViewModel.Instructions));
        base.OnInitialized();
    }
    private void ShowChange()
    {
        InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }
}
