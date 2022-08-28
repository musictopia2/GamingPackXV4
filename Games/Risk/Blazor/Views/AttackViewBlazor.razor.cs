namespace Risk.Blazor.Views;
public partial class AttackViewBlazor
{
    private ICustomCommand RollCommand => DataContext!.RollAttackCommand!;
    private readonly RiskGameContainer _container;
    private readonly string _attackColor = "";
    private readonly string _defenseColor = "";
    public AttackViewBlazor()
    {
        _container = aa.Resolver!.Resolve<RiskGameContainer>();
        if (_container.SaveRoot.CurrentTerritory == 0 || _container.SaveRoot.PreviousTerritory == 0)
        {
            throw new CustomBasicException("Must have territory to and territory from filled out");
        }
        TerritoryModel territory = _container.GetTerritory(_container.SaveRoot.CurrentTerritory);
        RiskPlayerItem player = _container.PlayerList![territory.Owns];
        _defenseColor = player.Color.Color;
        territory = _container.GetTerritory(_container.SaveRoot.PreviousTerritory);
        player = _container.PlayerList![territory.Owns];
        _attackColor = player.Color.Color;
    }
}