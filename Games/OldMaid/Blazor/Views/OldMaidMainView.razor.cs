namespace OldMaid.Blazor.Views;
public partial class OldMaidMainView
{
    private readonly BasicList<LabelGridModel> _labels = new();
    private OldMaidVMData? _vmData;
    private OldMaidGameContainer? _gameContainer;
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<OldMaidVMData>();
        _gameContainer = aa1.Resolver.Resolve<OldMaidGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(OldMaidVMData.NormalTurn))
            .AddLabel("Status", nameof(OldMaidVMData.Status));
        base.OnInitialized();
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
}