namespace Risk.Blazor.Views;
public partial class RiskMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    private readonly RiskGameContainer _container;
    private readonly BasicList<ScoreColumnModel> _scores = new();
    public RiskMainView()
    {
        _container = aa.Resolver!.Resolve<RiskGameContainer>();
    }
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(RiskVMData.NormalTurn))
             .AddLabel("Instructions", nameof(RiskVMData.Instructions))
             .AddLabel("Armies To Place", nameof(RiskVMData.ArmiesToPlace))
             .AddLabel("Bonus Reenforcements", nameof(RiskVMData.BonusReenforcements));
        _scores.AddColumn("Color", true, nameof(RiskPlayerItem.Color)
            )
            .AddColumn("Cards", true, nameof(RiskPlayerItem.RiskCards));
        base.OnInitialized();
    }
    private string CurrentColor => _container.SingleInfo!.Color.Color;
    private bool CanShowBoard
    {
        get
        {
            return _container.SaveRoot.Stage switch
            {
                EnumStageList.Move => true,
                EnumStageList.Place => true,
                EnumStageList.StartAttack => true,
                EnumStageList.EndTurn => true,
                EnumStageList.TransferAfterBattle => true,
                EnumStageList.EndGame => true,
                EnumStageList.Begin => true,
                _ => false
            };
        }
    }
    private ICustomCommand EndCommand => DataContext!.EndTurnCommand!;
    private ICustomCommand ToNextStepCommand => DataContext!.ToNextStepCommand!;
    private ICustomCommand StartAttackCommand => DataContext!.StartAttackCommand!;
    private ICustomCommand MoveArmiesCommand => DataContext!.MoveArmiesCommand!;
    private ICustomCommand PlaceArmiesCommand => DataContext!.PlaceArmiesCommand!;
    private ICustomCommand ReturnRiskCardsCommand => DataContext!.ReturnRiskCardsCommand!;
}