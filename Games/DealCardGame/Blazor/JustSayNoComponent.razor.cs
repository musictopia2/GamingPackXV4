namespace DealCardGame.Blazor;
public partial class JustSayNoComponent
{
    [Inject]
    private IToast? Toast { get; set; }
    private JustSayNoViewModel? DataContext { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    protected override void OnInitialized()
    {
        DataContext = aa1.Resolver!.Resolve<JustSayNoViewModel>();
        DataContext.AddAction(StateHasChanged);
        //DataContext.AddCommand(StateHasChanged);
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DealCardGameVMData.OtherTurn))
            .AddLabel("Owed", nameof(DealCardGameVMData.Owed))
            .AddLabel("Paid So Far", nameof(DealCardGameVMData.PaidSoFar));
        ;
        base.OnInitialized();
    }
    //private void RunTest()
    //{
    //    Toast!.ShowInfoToast("Updating");
    //    StateHasChanged();
    //}
    private BasicGameCommand AcceptCommand => DataContext!.AcceptCommand!;
    private BasicGameCommand RejectCommand => DataContext!.RejectCommand!;
}