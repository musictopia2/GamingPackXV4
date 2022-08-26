namespace BattleshipLite.Core.Logic;
[SingletonGame]
[AutoReset]
public class BattleshipBoardClass : ISerializable, IMoveNM
{
    private readonly BattleshipLiteGameContainer _gameContainer;
    public BattleshipBoardClass(BattleshipLiteGameContainer gameContainer)
    {
        _gameContainer = gameContainer;
    }
    public bool CanMakeMove(ShipInfo ship)
    {
        if (_gameContainer.SaveRoot.GameStatus == EnumGameStatus.PlacingShips)
        {
            return ship.WasShip == false;
        }
        return ship.ShipStatus == EnumShipStatus.None;
    }
    public async Task MakeMoveAsync(ShipInfo payLoad)
    {
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData))
        {
            await _gameContainer.Network!.SendMoveAsync(payLoad);
        }
        ShipInfo ourShip = _gameContainer.SingleInfo!.Ships[payLoad.Vector];
        if (_gameContainer.SaveRoot.GameStatus == EnumGameStatus.PlacingShips)
        {
            ourShip.WasShip = true;
            _gameContainer.Command.UpdateAll();
            if (_gameContainer.SingleInfo.Ships.Count(x => x.WasShip) == 5)
            {
                //means has to end turn.
                await _gameContainer.Delay.DelayMilli(1000); //so you can see the last one placed.
                await _gameContainer.EndTurnAsync!.Invoke();
                return;
            }
            await _gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        //this means its in game.
        BattleshipLitePlayerItem player;
        if (_gameContainer.WhoTurn == 1)
        {
            player = _gameContainer.PlayerList![2];
        }
        else
        {
            player = _gameContainer.PlayerList![1];
        }
        ShipInfo otherShip = player.Ships[payLoad.Vector];
        if (otherShip.WasShip)
        {
            //this means this was where they placed their ship
            ourShip.ShipStatus = EnumShipStatus.Hit;
            _gameContainer.Command.UpdateAll();
        }
        else
        {
            ourShip.ShipStatus = EnumShipStatus.Miss;
        }
        _gameContainer.Command?.UpdateAll(); //you have to update the ui stuff here.
        await _gameContainer.EndTurnAsync!.Invoke();
    }
    public void PossibleChangeStatus()
    {
        if (_gameContainer.SaveRoot.GameStatus == EnumGameStatus.MainGame)
        {
            return;
        }
        foreach (var player in _gameContainer.PlayerList!)
        {
            if (player.Ships.Count(x => x.WasShip) < 5)
            {
                return; //because a player has at least one ship that has to be placed.
            }
        }
        _gameContainer.SaveRoot.GameStatus = EnumGameStatus.MainGame; //because all players has placed all ships.
    }
    public bool IsGameOver()
    {
        if (_gameContainer.SaveRoot.GameStatus == EnumGameStatus.PlacingShips)
        {
            return false;
        }
        int hits = _gameContainer.SingleInfo!.Ships.Count(x => x.ShipStatus == EnumShipStatus.Hit);
        return hits == 5;
    }
    async Task IMoveNM.MoveReceivedAsync(string data)
    {
        ShipInfo ship = await js.DeserializeObjectAsync<ShipInfo>(data);
        await MakeMoveAsync(ship);
    }
    public static BasicList<string> RowList => new() { "1", "2", "3", "4", "5" };
    public static BasicList<string> ColumnList = new() { "A", "B", "C", "D", "E" };
}