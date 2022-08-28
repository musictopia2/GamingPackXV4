namespace Risk.Core.Logic;
[SingletonGame]
[AutoReset] //because needs the container which had to be changed as well.
public class AttackLogic
{
    private readonly RiskGameContainer _container;
    private readonly RiskRollingProcesses _rollingProcesses;
    private readonly GameBoardProcesses _gameBoardProcesses;
    public AttackLogic(RiskGameContainer container,
        RiskRollingProcesses rollingProcesses,
        GameBoardProcesses gameBoardProcesses
        )
    {
        _container = container;
        _rollingProcesses = rollingProcesses;
        _gameBoardProcesses = gameBoardProcesses;
        _container!.AfterRollingAsync = AfterRollingAsync; //for now, this will handle the afterolling.
        _container.StartSentAttack = () =>
        {
            PopulateDefenses();
            _container.SaveRoot.Stage = EnumStageList.Roll;
            _container.VMData.RollingProgress = true;
            _container.Command.UpdateAll();
        };
    }
    private void PopulateDefenses()
    {
        if (_container.SaveRoot.CurrentTerritory == 0)
        {
            throw new CustomBasicException("There is no territory for defense");
        }
        TerritoryModel territory = _container.GetTerritory(_container.SaveRoot.CurrentTerritory);
        if (territory.Armies >= 2)
        {
            _container.SaveRoot.NumberDefenseArmies = 2;
        }
        else
        {
            _container.SaveRoot.NumberDefenseArmies = territory.Armies;
        }
    }
    private async Task AfterRollingAsync()
    {
        if (_container.VMData.DefenseCup!.HowManyDice != _container.SaveRoot.NumberDefenseArmies)
        {
            throw new CustomBasicException($"Defense cup does not match the defense armies.  Number of defense armies is {_container.SaveRoot.NumberDefenseArmies} but the cup has {_container.VMData.DefenseCup.HowManyDice}");
        }
        if (_container.VMData.AttackCup!.HowManyDice != _container.SaveRoot.ArmiesInBattle)
        {
            throw new CustomBasicException($"Attack cup does not match the attack armies.  Number of attack armies is {_container.SaveRoot.ArmiesInBattle} but the cup has {_container.VMData.AttackCup.HowManyDice}");
        }
        if (_container.VMData.DefenseCup.DiceList.Count != _container.VMData.DefenseCup.HowManyDice)
        {
            throw new CustomBasicException($"Defense should had {_container.VMData.DefenseCup.HowManyDice} but only have {_container.VMData.DefenseCup.DiceList.Count}");
        }
        if (_container.VMData.AttackCup.DiceList.Count != _container.VMData.AttackCup.HowManyDice)
        {
            throw new CustomBasicException($"Attack should had {_container.VMData.AttackCup.HowManyDice} but only have {_container.VMData.AttackCup.DiceList.Count}");
        }
        var lossData = _container.GetAttackResults();
        _container.SaveRoot.ArmiesInBattle -= lossData.AttackLosses;
        _container.SaveRoot.NumberDefenseArmies -= lossData.DefenseLosses;
        _container.Command.UpdateAll(); //try this as well.  this should update everything (?)
        await _container.Delay.DelayMilli(1200); //to see the results
        TerritoryModel territory = _container.GetTerritory(_container.SaveRoot.PreviousTerritory);
        territory.Armies -= lossData.AttackLosses;
        bool needsToEnd = false;
        if (territory.Armies == 1)
        {
            needsToEnd = true;
        }
        territory = _container.GetTerritory(_container.SaveRoot.CurrentTerritory);
        territory.Armies -= lossData.DefenseLosses;
        _container.SaveRoot.Stage = EnumStageList.StartAttack; //go ahead and start attack again unless something else is set.
        if (territory.Armies == 0)
        {
            _gameBoardProcesses.ConquerTerritory(territory);
            territory = _container.GetTerritory(_container.SaveRoot.PreviousTerritory);
            if (territory.Armies == 1)
            {
                needsToEnd = true; //here too.
            }

            if (needsToEnd == false)
            {
                _container.SaveRoot.Stage = EnumStageList.TransferAfterBattle; //has to now transfer after battle.
            }
        }
        if (needsToEnd)
        {
            _gameBoardProcesses.ResetMove();
        }
        _container.SaveRoot.ArmiesInBattle = 0; //should be fine for this version because we are testing something.
        _container.SaveRoot.NumberDefenseArmies = 0;
        _rollingProcesses.HideDice();
        _container.VMData.RollingProgress = false;
        if (_gameBoardProcesses.DidWin)
        {
            _container.SaveRoot.Stage = EnumStageList.EndGame; //i think now.
            _gameBoardProcesses.ResetMove();
            _container.SaveRoot.Instructions = $"Game Is Over {_container.SingleInfo!.NickName} has won"; //since there is no status, then show game is over
            _container.Command.UpdateAll(); //to show that part.
            await _container.ShowWinAsync!.Invoke(); ;
            return;
        }
        await _container.ContinueTurnAsync!.Invoke(); //has to continue turn because you can do other moves now.
    }
    public async Task RollAttackAsync()
    {
        PopulateDefenses();
        _container.VMData.RollingProgress = true;
        if (_container.BasicData.MultiPlayer)
        {
            await _container.Network!.SendAllAsync(nameof(IMultiplayerModel.AttackArmies), _container.SaveRoot.ArmiesInBattle);
        }
        _container.Command.UpdateAll();
        await _rollingProcesses.RollDiceAsync();
    }
}