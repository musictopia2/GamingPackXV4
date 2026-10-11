namespace Risk.Core.Logic;
[SingletonGame]
[AutoReset]
public class GameBoardProcesses(RiskGameContainer gameContainer, IToast toast)
{
    private BasicList<int> GetConnectedTerritoryList(int id)
    {
        TerritoryModel territory = gameContainer.GetTerritory(id);
        if (territory.Owns != gameContainer.WhoTurn)
        {
            return [];
        }
        BasicList<int> output = [];
        GetConnectedTerritoryListR(territory, ref output);
        output.RemoveAllAndObtain(xx => xx == id);
        return output;
    }
    private void GetConnectedTerritoryListR(TerritoryModel territory, ref BasicList<int> list)
    {
        if (territory.Owns != gameContainer.WhoTurn)
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
            newTerritory = gameContainer.GetTerritory(v);
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
        if (currentPosition == gameContainer.SaveRoot.CurrentTerritory)
        {
            return true;
        }
        TerritoryModel territory = gameContainer.GetTerritory(currentPosition);
        if (territory.Owns != gameContainer.WhoTurn)
        {
            return false;
        }
        territory = gameContainer.GetTerritory(previousPosition);
        if (territory.Owns != gameContainer.WhoTurn)
        {
            return false;
        }
        if (gameContainer.SaveRoot.CurrentTerritory != 0 && gameContainer.SaveRoot.CurrentTerritory != gameContainer.SaveRoot.CurrentTerritory)
        {
            return false;
        }
        BasicList<int> connections = GetConnectedTerritoryList(previousPosition);
        return connections.Any(xx => xx == currentPosition);
    }
    private bool CanSelectTerritory(TerritoryModel territory)
    {
        if (territory.Owns == gameContainer.WhoTurn && territory.Armies == 1)
        {
            return false;
        }
        if (territory.Owns == gameContainer.WhoTurn)
        {
            return true;
        }
        if (gameContainer.SaveRoot.PreviousTerritory == 0)
        {
            return false;
        }
        TerritoryModel previous = gameContainer.GetTerritory(gameContainer.SaveRoot.PreviousTerritory);
        return previous.Neighbors.Any(xx => xx == territory.Id);
    }
    public async Task TerritorySelectedAsync(TerritoryModel territory, bool validate)
    {
        DidWin = false;
        if (gameContainer.SaveRoot.Stage == EnumStageList.StartAttack)
        {
            await SelectUnselectTerritoryAsync(territory, validate);
            return;
        }
        if (gameContainer.SaveRoot.Stage == EnumStageList.Move)
        {
            await MoveTerritoryAsync(territory, validate);
            return;
        }
        if (gameContainer.SaveRoot.Stage == EnumStageList.Place)
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
            if (territory.Owns != gameContainer.WhoTurn)
            {
                return;
            }
            await SendTerritoryAsync(territory);
        }
        gameContainer.SaveRoot.PreviousTerritory = territory.Id;
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private async Task SendTerritoryAsync(TerritoryModel territory)
    {
        if (gameContainer.BasicData.MultiPlayer)
        {
            await gameContainer.Network!.SendAllAsync(nameof(IMultiplayerModel.SelectTerritory), territory.Id);
        }
    }
    private async Task MoveTerritoryAsync(TerritoryModel territory, bool validate)
    {
        if (validate)
        {
            if (gameContainer.SaveRoot.PreviousTerritory == 0 || gameContainer.SaveRoot.PreviousTerritory == territory.Id)
            {
                if (CanSelectTerritory(territory) == false)
                {
                    return;
                }
            }
            else
            {
                if (CanPassThrough(territory.Id, gameContainer.SaveRoot.PreviousTerritory) == false)
                {
                    toast.ShowUserErrorToast("You cannot pass through"); //for now, show toast.
                    return;
                }
            }
            await SendTerritoryAsync(territory);
        }
        if (gameContainer.SaveRoot.PreviousTerritory == 0)
        {
            gameContainer.SaveRoot.PreviousTerritory = territory.Id;
        }
        else if (gameContainer.SaveRoot.PreviousTerritory == territory.Id && gameContainer.SaveRoot.CurrentTerritory == 0)
        {
            gameContainer.SaveRoot.PreviousTerritory = 0;
        }
        else if (gameContainer.SaveRoot.CurrentTerritory == territory.Id)
        {
            gameContainer.SaveRoot.CurrentTerritory = 0;
        }
        else
        {
            gameContainer.SaveRoot.CurrentTerritory = territory.Id;
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
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
        if (gameContainer.SaveRoot.PreviousTerritory == territory.Id)
        {
            ResetMove();
        }
        else if (gameContainer.SaveRoot.PreviousTerritory == 0)
        {
            gameContainer.SaveRoot.PreviousTerritory = territory.Id;
            gameContainer.SaveRoot.CurrentTerritory = 0;
        }
        else if (gameContainer.SaveRoot.CurrentTerritory == territory.Id)
        {
            gameContainer.SaveRoot.CurrentTerritory = 0;
        }
        else if (territory.Owns == gameContainer.WhoTurn)
        {
            gameContainer.SaveRoot.PreviousTerritory = territory.Id;
            gameContainer.SaveRoot.CurrentTerritory = 0;
        }
        else if (territory.Owns != gameContainer.WhoTurn)
        {
            gameContainer.SaveRoot.CurrentTerritory = territory.Id;
        }
        else
        {
            throw new CustomBasicException("Not sure how to handle selecting this territory");
        }
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    public async Task PlaceArmiesAsync(bool show)
    {
        if (gameContainer.VMData.ArmiesChosen == 0)
        {
            throw new CustomBasicException("Must choose to place at least one army");
        }
        TerritoryModel territory = gameContainer.GetTerritory(gameContainer.SaveRoot.PreviousTerritory);
        if (territory.Owns != gameContainer.WhoTurn)
        {
            await gameContainer.ContinueTurnAsync!.Invoke();
            return; //attempt to make it where if you are placing armies, then if the player whose turn it is you can't do it, just continue turn from there.
        }
        await ShowUIAsync(show);
        territory.Armies += gameContainer.VMData.ArmiesChosen;
        gameContainer.SaveRoot.ArmiesToPlace -= gameContainer.VMData.ArmiesChosen;
        if (gameContainer.SaveRoot.ArmiesToPlace == 0)
        {
            gameContainer.SaveRoot.Stage = EnumStageList.StartAttack;
        }
        ResetMove();
        await gameContainer.ContinueTurnAsync!.Invoke();
    }
    private async Task ShowUIAsync(bool show)
    {
        if (show)
        {
            gameContainer.VMData.NumberPicker.SelectNumberValue(gameContainer.VMData.ArmiesChosen);
            await gameContainer.Delay.DelayMilli(800);
        }
    }
    public async Task MoveArmiesAsync(bool show)
    {
        if (gameContainer.VMData.ArmiesChosen == 0 && gameContainer.SaveRoot.Stage == EnumStageList.Move)
        {
            throw new CustomBasicException("If not transferring armies, cannot move 0 armies");
        }
        TerritoryModel previous = gameContainer.GetTerritory(gameContainer.SaveRoot.PreviousTerritory);
        TerritoryModel current = gameContainer.GetTerritory(gameContainer.SaveRoot.CurrentTerritory);
        await ShowUIAsync(show);
        current.Armies += gameContainer.VMData.ArmiesChosen;
        previous.Armies -= gameContainer.VMData.ArmiesChosen;
        ResetMove();
        if (gameContainer.SaveRoot.Stage == EnumStageList.TransferAfterBattle)
        {
            gameContainer.SaveRoot.Stage = EnumStageList.StartAttack;
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        await gameContainer.EndMoveAsync!.Invoke();
    }
    public bool DidWin { get; private set; }
    public void ConquerTerritory(TerritoryModel territory)
    {
        RiskPlayerItem oldPlayer = gameContainer.PlayerList![territory.Owns];
        territory.Owns = gameContainer.WhoTurn;
        territory.Color = gameContainer.SingleInfo!.Color.WebColor;
        territory.Armies = gameContainer.SaveRoot.ArmiesInBattle;
        territory = gameContainer.GetTerritory(gameContainer.SaveRoot.PreviousTerritory);
        territory.Armies -= gameContainer.SaveRoot.ArmiesInBattle;
        if (oldPlayer.InGame)
        {
            if (gameContainer.SaveRoot.TerritoryList.Any(xx => xx.Owns == oldPlayer.Id) == false)
            {
                DidWin = true;
            }
        }
        gameContainer.SaveRoot.ConqueredOne = true;
    }
    public void ResetMove()
    {
        gameContainer.SaveRoot.PreviousTerritory = 0;
        gameContainer.SaveRoot.CurrentTerritory = 0;
        gameContainer.VMData.ArmiesChosen = 0; //try this too.
    }
    public int HowManyTerritories(int turn)
    {
        return gameContainer.SaveRoot.TerritoryList.Count(xx => xx.Owns == turn);
    }
    public bool ContinentControlled(EnumContinent continent)
    {
        var list = gameContainer.SaveRoot.TerritoryList.Where(xx => xx.Continent == continent).ToBasicList();
        return list.All(xx => xx.Owns == gameContainer.WhoTurn); //hopefully this simple.
    }
}