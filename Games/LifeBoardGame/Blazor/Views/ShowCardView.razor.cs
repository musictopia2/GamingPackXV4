namespace LifeBoardGame.Blazor.Views;
public partial class ShowCardView
{
    private LifeBoardGameVMData? DataContext { get; set; }
    protected override void OnInitialized()
    {
        DataContext = aa.Resolver!.Resolve<LifeBoardGameVMData>();
        base.OnInitialized();
    }
}