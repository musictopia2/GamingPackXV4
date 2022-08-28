namespace Risk.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses
{
    private readonly RiskGameContainer _gameContainer;
    private readonly IToast _toast;
    public GameBoardProcesses(RiskGameContainer gameContainer, IToast toast)
    {
        _gameContainer = gameContainer;
        _toast = toast;
    }
    private BasicList<int> GetConnectedTerritoryList(int id)
    {
        TerritoryModel territory = _gameContainer.GetTerritory(id);
        if (territory.Owns != _gameContainer.WhoTurn)
        {
            return new();
        }
        BasicList<int> output = new();
        GetConnectedTerritoryListR(territory, ref output);
        output.RemoveAllAndObtain(xx => xx == id);
        return output;
    }
    private void GetConnectedTerritoryListR(TerritoryModel territory, ref BasicList<int> list)
    {
        if (territory.Owns != _gameContainer.WhoTurn)
        {
            return;
        }
        bool addit = true; //has to be proven false.
        if (list.Any(xx => xx == territory.Id))
        {
            addit = false;
        }
        if (addit)
        {
            list.Add(territory.Id);
        }
        TerritoryModel newTerritory;
        foreach (var v in territory.Neighbors)
        {
            newTerritory = _gameContainer.GetTerritory(v);
            if (newTerritory.Owns == territory.Owns)
            {
                addit = true;
                if (list.Any(xx => xx == v))
                {
                    addit = false;
                }
                if (addit)
                {
                    list.Add(newTerritory.Id);
                    GetConnectedTerritoryListR(newTerritory, ref list);
                }
            }
        }
    }
    private bool CanPassThrough(int currentPosition, int previousPosition)
    {
        if (currentPosition == _gameContainer.SaveRoot.CurrentTerritory)
        {
            return true;
        }
        TerritoryModel territory = _gameContainer.GetTerritory(currentPosition);
        if (territory.Owns != _gameContainer.WhoTurn)
        {
            return false;
        }
        territory = _gameContainer.GetTerritory(previousPosition);
        if (territory.Owns != _gameContainer.WhoTurn)
        {
            return false;
        }
        if (_gameContainer.SaveRoot.CurrentTerritory != 0 && _gameContainer.SaveRoot.CurrentTerritory != _gameContainer.SaveRoot.CurrentTerritory)
        {
            return false;
        }
        BasicList<int> connections = GetConnectedTerritoryList(previousPosition);
        return connections.Any(xx => xx == currentPosition);
    }
    private bool CanSelectTerritory(TerritoryModel territory)
    {
        if (territory.Owns == _gameContainer.WhoTurn && territory.Armies == 1)
        {
            return false;
        }
        if (territory.Owns == _gameContainer.WhoTurn)
        {
            return true;
        }
        if (_gameContainer.SaveRoot.PreviousTerritory == 0)
        {
            return false;
        }
        TerritoryModel previous = _gameContainer.GetTerritory(_gameContainer.SaveRoot.PreviousTerritory);
        return previous.Neighbors.Any(xx => xx == territory.Id);
    }
    public async Task TerritorySelectedAsync(TerritoryModel territory, bool validate)
    {
        DidWin = false;
        if (_gameContainer.SaveRoot.Stage == EnumStageList.StartAttack)
        {
            await SelectUnselectTerritoryAsync(territory, validate);
            return;
        }
        if (_gameContainer.SaveRoot.Stage == EnumStageList.Move)
        {
            await MoveTerritoryAsync(territory, validate);
            return;
        }
        if (_gameContainer.SaveRoot.Stage == EnumStageList.Place)
        {
            await PlaceArmiesAsync(territory, validate);
            return;
        }
        throw new CustomBasicException("Not Supported");
    }
    private async Task PlaceArmiesAsync(TerritoryModel territory, bool validate)
    {
        if (validate)
        {
            if (territory.Owns != _gameContainer.WhoTurn)
            {
                return;
            }
            await SendTerritoryAsync(territory);
        }
        _gameContainer.SaveRoot.PreviousTerritory = territory.Id;
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    private async Task SendTerritoryAsync(TerritoryModel territory)
    {
        if (_gameContainer.BasicData.MultiPlayer)
        {
            await _gameContainer.Network!.SendAllAsync(nameof(IMultiplayerModel.SelectTerritory), territory.Id);
        }
    }
    private async Task MoveTerritoryAsync(TerritoryModel territory, bool validate)
    {
        if (validate)
        {
            if (_gameContainer.SaveRoot.PreviousTerritory == 0 || _gameContainer.SaveRoot.PreviousTerritory == territory.Id)
            {
                if (CanSelectTerritory(territory) == false)
                {
                    return;
                }
            }
            else
            {
                if (CanPassThrough(territory.Id, _gameContainer.SaveRoot.PreviousTerritory) == false)
                {
                    _toast.ShowUserErrorToast("You cannot pass through"); //for now, show toast.
                    return;
                }
            }
            await SendTerritoryAsync(territory);
        }
        if (_gameContainer.SaveRoot.PreviousTerritory == 0)
        {
            _gameContainer.SaveRoot.PreviousTerritory = territory.Id;
        }
        else if (_gameContainer.SaveRoot.PreviousTerritory == territory.Id && _gameContainer.SaveRoot.CurrentTerritory == 0)
        {
            _gameContainer.SaveRoot.PreviousTerritory = 0;
        }
        else if (_gameContainer.SaveRoot.CurrentTerritory == territory.Id)
        {
            _gameContainer.SaveRoot.CurrentTerritory = 0;
        }
        else
        {
            _gameContainer.SaveRoot.CurrentTerritory = territory.Id;
        }
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    private async Task SelectUnselectTerritoryAsync(TerritoryModel territory, bool validate)
    {
        if (validate)
        {
            if (CanSelectTerritory(territory) == false)
            {
                return;
            }
            await SendTerritoryAsync(territory);
        }
        if (_gameContainer.SaveRoot.PreviousTerritory == territory.Id)
        {
            ResetMove();
        }
        else if (_gameContainer.SaveRoot.PreviousTerritory == 0)
        {
            _gameContainer.SaveRoot.PreviousTerritory = territory.Id;
            _gameContainer.SaveRoot.CurrentTerritory = 0;
        }
        else if (_gameContainer.SaveRoot.CurrentTerritory == territory.Id)
        {
            _gameContainer.SaveRoot.CurrentTerritory = 0;
        }
        else if (territory.Owns == _gameContainer.WhoTurn)
        {
            _gameContainer.SaveRoot.PreviousTerritory = territory.Id;
            _gameContainer.SaveRoot.CurrentTerritory = 0;
        }
        else if (territory.Owns != _gameContainer.WhoTurn)
        {
            _gameContainer.SaveRoot.CurrentTerritory = territory.Id;
        }
        else
        {
            throw new CustomBasicException("Not sure how to handle selecting this territory");
        }
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task PlaceArmiesAsync(bool show)
    {
        if (_gameContainer.VMData.ArmiesChosen == 0)
        {
            throw new CustomBasicException("Must choose to place at least one army");
        }
        TerritoryModel territory = _gameContainer.GetTerritory(_gameContainer.SaveRoot.PreviousTerritory);
        if (territory.Owns != _gameContainer.WhoTurn)
        {
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return; //attempt to make it where if you are placing armies, then if the player whose turn it is you can't do it, just continue turn from there.
        }
        await ShowUIAsync(show);
        territory.Armies += _gameContainer.VMData.ArmiesChosen;
        _gameContainer.SaveRoot.ArmiesToPlace -= _gameContainer.VMData.ArmiesChosen;
        if (_gameContainer.SaveRoot.ArmiesToPlace == 0)
        {
            _gameContainer.SaveRoot.Stage = EnumStageList.StartAttack;
        }
        ResetMove();
        await _gameContainer.ContinueTurnAsync!.Invoke();
    }
    private async Task ShowUIAsync(bool show)
    {
        if (show)
        {
            _gameContainer.VMData.NumberPicker.SelectNumberValue(_gameContainer.VMData.ArmiesChosen);
            _gameContainer.Command.UpdateAll();
            await _gameContainer.Delay.DelayMilli(800);
        }
    }
    public async Task MoveArmiesAsync(bool show)
    {
        if (_gameContainer.VMData.ArmiesChosen == 0 && _gameContainer.SaveRoot.Stage == EnumStageList.Move)
        {
            throw new CustomBasicException("If not transferring armies, cannot move 0 armies");
        }
        TerritoryModel previous = _gameContainer.GetTerritory(_gameContainer.SaveRoot.PreviousTerritory);
        TerritoryModel current = _gameContainer.GetTerritory(_gameContainer.SaveRoot.CurrentTerritory);
        await ShowUIAsync(show);
        current.Armies += _gameContainer.VMData.ArmiesChosen;
        previous.Armies -= _gameContainer.VMData.ArmiesChosen;
        ResetMove();
        if (_gameContainer.SaveRoot.Stage == EnumStageList.TransferAfterBattle)
        {
            _gameContainer.SaveRoot.Stage = EnumStageList.StartAttack;
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        await _gameContainer.EndMoveAsync!.Invoke();
    }
    public bool DidWin { get; private set; }
    public void ConquerTerritory(TerritoryModel territory)
    {
        RiskPlayerItem oldPlayer = _gameContainer.PlayerList![territory.Owns];
        territory.Owns = _gameContainer.WhoTurn;
        territory.Color = _gameContainer.SingleInfo!.Color.WebColor;
        territory.Armies = _gameContainer.SaveRoot.ArmiesInBattle;
        territory = _gameContainer.GetTerritory(_gameContainer.SaveRoot.PreviousTerritory);
        territory.Armies -= _gameContainer.SaveRoot.ArmiesInBattle;
        if (oldPlayer.InGame)
        {
            if (_gameContainer.SaveRoot.TerritoryList.Any(xx => xx.Owns == oldPlayer.Id) == false)
            {
                DidWin = true;
            }
        }
        _gameContainer.SaveRoot.ConqueredOne = true;
    }
    public void ResetMove()
    {
        _gameContainer.SaveRoot.PreviousTerritory = 0;
        _gameContainer.SaveRoot.CurrentTerritory = 0;
        _gameContainer.VMData.ArmiesChosen = 0; //try this too.
    }
    public int HowManyTerritories(int turn)
    {
        return _gameContainer.SaveRoot.TerritoryList.Count(xx => xx.Owns == turn);
    }
    public bool ContinentControlled(EnumContinent continent)
    {
        var list = _gameContainer.SaveRoot.TerritoryList.Where(xx => xx.GetContinent() == continent).ToBasicList();
        return list.All(xx => xx.Owns == _gameContainer.WhoTurn); //hopefully this simple.
    }
}