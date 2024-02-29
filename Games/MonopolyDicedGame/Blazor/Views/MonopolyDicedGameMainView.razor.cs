namespace MonopolyDicedGame.Blazor.Views;
public partial class MonopolyDicedGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private BasicList<EnumMiscType> _others = [];
    private MonopolyDicedGameGameContainer? _container;
    private HouseDice? _house;
    protected override void OnInitialized()
    {
        _container = aa1.Resolver!.Resolve<MonopolyDicedGameGameContainer>();
        _house = aa1.Resolver!.Resolve<HouseDice>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(MonopolyDicedGameVMData.NormalTurn))
            .AddLabel("Status", nameof(MonopolyDicedGameVMData.Status))
            .AddLabel("Roll", nameof(MonopolyDicedGameVMData.RollNumber))
            .AddLabel("Current Score", nameof(MonopolyDicedGameVMData.CurrentScore))
            ;
        base.OnInitialized();
    }
    private bool CanTestRoll() => _container!.SaveRoot.NumberOfCops < 3;
    private void ClearTestRoll()
    {
        _container!.SaveRoot.RollNumber = 1;
        _container.SaveRoot.NumberOfCops = 0;
        _others.Clear();
    }
    private async Task TestRollMisc()
    {

        var list = _house!.RollDice();
        await _house.ShowRollingAsync(list);


        _others = _container!.SaveRoot.GetMiscResults(_container.Random);
        _container.SaveRoot.RollNumber++;
        //hopefully registers with vmdata (?)
        _container.SaveRoot.NumberOfCops += _others.Count(x => x == EnumMiscType.Police);
        if (_others.Any(x => x == EnumMiscType.Free))
        {
            _container.SaveRoot.NumberOfCops--;
        }
        if (_others.Any(x => x == EnumMiscType.Go))
        {
            _container.SaveRoot.CurrentScore += 200;
        }
        if (_house.Value == EnumMiscType.Free && _container.SaveRoot.NumberOfCops > 0)
        {
            _container.SaveRoot.NumberOfCops--;
        }
        if (_house.Value == EnumMiscType.BrokenHouse)
        {
            if (_container.SaveRoot.NumberOfHouses > 0)
            {
                _container.SaveRoot.NumberOfHouses--;
            }
        }
        if (_house.Value == EnumMiscType.RegularHouse)
        {
            if (_container.SaveRoot.NumberOfHouses < 4 && _container.SaveRoot.HasHotel == false)
            {
                _container.SaveRoot.NumberOfHouses++;
            }
        }
        if (_house.Value == EnumMiscType.Hotel)
        {
            if (_container.SaveRoot.NumberOfHouses == 4)
            {
                _container.SaveRoot.HasHotel = true;
                _container.SaveRoot.NumberOfHouses = 0; //because you now have hotel.
            }
        }
        if (_container.SaveRoot.NumberOfCops > 2)
        {
            _container.SaveRoot.CurrentScore = 0; //this means 0 points period.
        }
        //for now, the code is here.  will eventually move to somewhere else.
        _container.Command.UpdateAll(); 
    }
}