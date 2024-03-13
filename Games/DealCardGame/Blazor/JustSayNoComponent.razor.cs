namespace DealCardGame.Blazor;
public partial class JustSayNoComponent
{
    private JustSayNoViewModel? DataContext { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    protected override void OnInitialized()
    {
        DataContext = aa1.Resolver!.Resolve<JustSayNoViewModel>();
        DataContext.AddAction(StateHasChanged);
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DealCardGameVMData.OtherTurn))
            .AddLabel("Owed", nameof(DealCardGameVMData.Owed))
            .AddLabel("Paid So Far", nameof(DealCardGameVMData.PaidSoFar));
        ;
        base.OnInitialized();
    }
    private BasicGameCommand AcceptCommand => DataContext!.AcceptCommand!;
    private BasicGameCommand RejectCommand => DataContext!.RejectCommand!;
}