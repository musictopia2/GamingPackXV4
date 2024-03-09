namespace DealCardGame.Blazor;
public partial class MakePaymentComponent : IDisposable
{
    //don't want to deal with screens either.
    private PaymentViewModel? DataContext { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    protected override void OnInitialized()
    {
        DataContext = aa1.Resolver!.Resolve<PaymentViewModel>();

        DataContext.AddCommand(StateHasChanged);

        //DataContext.NotifyStateChange = StateHasChanged;
        //DataContext.AddCommandAction(StateHasChanged);
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(DealCardGameVMData.OtherTurn))
            .AddLabel("Owed", nameof(DealCardGameVMData.Owed))
            .AddLabel("Paid So Far", nameof(DealCardGameVMData.PaidSoFar));
            ;
        base.OnInitialized();
    }

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
    public void Dispose()
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    {
        //DataContext!.RemoveCommandAction();
    }

    private BasicGameCommand AddPaymentsCommand => DataContext!.AddPaymentsCommand!;
    private BasicGameCommand StartOverCommand => DataContext!.StartOverCommand!;
    private BasicGameCommand FinishPaymentCommand => DataContext!.FinishPaymentCommand!;
}