namespace Risk.Core.Logic;
[SingletonGame]
[AutoReset]
public class RiskRollingProcesses
{
    private readonly RiskGameContainer _container;
    private RiskVMData _data;
    public RiskRollingProcesses(RiskGameContainer container)
    {
        _container = container;
        _data = aa1.Resolver!.Resolve<RiskVMData>();
        _container.SentAttackProcessesAsync = ShowRollingAsync;
    }
    public async Task RollDiceAsync()
    {
        SetDice();
        var defenseList = _data.DefenseCup!.RollDice();
        var attackList = _data.AttackCup!.RollDice();
        SendAttackResult results = new(attackList, defenseList);
        if (_container.BasicData.MultiPlayer)
        {
            await _container.Network!.SendAllAsync(nameof(IMultiplayerModel.SendAttackData), results);
        }
        await ShowRollingAsync(results);
    }
    private void SetDice()
    {
        _data.AttackCup!.HowManyDice = _container.SaveRoot.ArmiesInBattle;
        _data.DefenseCup!.HowManyDice = _container.SaveRoot.NumberDefenseArmies;
        HideDice();
    }
    public void HideDice()
    {
        _data.AttackCup!.CanShowDice = false;
        _data.DefenseCup!.CanShowDice = false;
        _data.AttackCup.RefreshDice();
        _data.DefenseCup.RefreshDice();
    }
    private async Task ShowRollingAsync(SendAttackResult results)
    {
        _data = aa1.Resolver!.Resolve<RiskVMData>();
        SetDice();
        await _data.AttackCup!.ShowRollingAsync(results.AttackList);
        await _data.DefenseCup!.ShowRollingAsync(results.DefenseList);
        _data.AttackCup.SortDice(true);
        _data.DefenseCup.SortDice(true);
        if (_container.AfterRollingAsync is null)
        {
            throw new CustomBasicException("Nobody is handling after rolling");
        }
        await _container.AfterRollingAsync.Invoke();
    }
}