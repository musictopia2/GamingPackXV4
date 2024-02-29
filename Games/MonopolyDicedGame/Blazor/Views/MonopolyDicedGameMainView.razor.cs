namespace MonopolyDicedGame.Blazor.Views;
public partial class MonopolyDicedGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private BasicList<EnumMiscType> _others = [];
    private MonopolyDicedGameGameContainer? _container;
    protected override void OnInitialized()
    {
        _container = aa1.Resolver!.Resolve<MonopolyDicedGameGameContainer>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(MonopolyDicedGameVMData.NormalTurn))
            .AddLabel("Status", nameof(MonopolyDicedGameVMData.Status))
            .AddLabel("Roll", nameof(MonopolyDicedGameVMData.RollNumber));
            ;
        base.OnInitialized();
    }
    private bool CanTestRoll() => _container!.SaveRoot.NumberOfCops < 3;
    private void TestRollMisc()
    {
        _others = _container!.SaveRoot.GetMiscResults(_container.Random);
        _container.SaveRoot.RollNumber++;
        //hopefully registers with vmdata (?)
        _container.SaveRoot.NumberOfCops += _others.Count(x => x == EnumMiscType.Police);
        if (_others.Any(x => x == EnumMiscType.Free))
        {
            _container.SaveRoot.NumberOfCops--;
        }
    }
}