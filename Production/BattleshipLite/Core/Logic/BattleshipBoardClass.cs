namespace BattleshipLite.Core.Logic;
[SingletonGame]
[AutoReset]
public class BattleshipBoardClass(BattleshipLiteGameContainer gameContainer) : ISerializable, IMoveNM
{
    public bool CanMakeMove(ShipInfo ship)
    {
        if (gameContainer.SaveRoot.GameStatus == EnumGameStatus.PlacingShips)
        {
            return ship.WasShip == false;
        }
        return ship.ShipStatus == EnumShipStatus.None;
    }
    public async Task MakeMoveAsync(ShipInfo payLoad)
    {
        if (gameContainer.SingleInfo!.CanSendMessage(gameContainer.BasicData))
        {
            await gameContainer.Network!.SendMoveAsync(payLoad);
        }
        ShipInfo ourShip = gameContainer.SingleInfo!.Ships[payLoad.Vector];
        if (gameContainer.SaveRoot.GameStatus == EnumGameStatus.PlacingShips)
        {
            ourShip.WasShip = true;
            if (gameContainer.SingleInfo.Ships.Count(x => x.WasShip) == 5)
            {
                //means has to end turn.
                await gameContainer.Delay.DelayMilli(1000); //so you can see the last one placed.
                gameContainer.SingleInfo.UpdateShips();
                await gameContainer.EndTurnAsync!.Invoke();
                return;
            }
            
            //gameContainer.SingleInfo.
            await gameContainer.ContinueTurnAsync!.Invoke();
            return;
        }
        //this means its in game.
        BattleshipLitePlayerItem player;
        if (gameContainer.WhoTurn == 1)
        {
            player = gameContainer.PlayerList![2];
        }
        else
        {
            player = gameContainer.PlayerList![1];
        }
        ShipInfo otherShip = player.Ships[payLoad.Vector];
        if (otherShip.WasShip)
        {
            //this means this was where they placed their ship
            ourShip.ShipStatus = EnumShipStatus.Hit;
        }
        else
        {
            ourShip.ShipStatus = EnumShipStatus.Miss;
        }
        await gameContainer.EndTurnAsync!.Invoke();
    }
    public void PossibleChangeStatus()
    {
        if (gameContainer.SaveRoot.GameStatus == EnumGameStatus.MainGame)
        {
            return;
        }
        foreach (var player in gameContainer.PlayerList!)
        {
            if (player.Ships.Count(x => x.WasShip) < 5)
            {
                return; //because a player has at least one ship that has to be placed.
            }
        }
        gameContainer.SaveRoot.GameStatus = EnumGameStatus.MainGame; //because all players has placed all ships.
        var self = gameContainer.PlayerList.GetSelf();
        foreach (var ship in self.Ships)
        {
            ship.UpdateShip();
        }
    }
    public bool IsGameOver()
    {
        if (gameContainer.SaveRoot.GameStatus == EnumGameStatus.PlacingShips)
        {
            return false;
        }
        int hits = gameContainer.SingleInfo!.Ships.Count(x => x.ShipStatus == EnumShipStatus.Hit);
        return hits == 5;
    }
    async Task IMoveNM.MoveReceivedAsync(string data)
    {
        ShipInfo ship = await js1.DeserializeObjectAsync<ShipInfo>(data);
        await MakeMoveAsync(ship);
    }
    public static BasicList<string> RowList => ["1", "2", "3", "4", "5"];
    public static BasicList<string> ColumnList => ["A", "B", "C", "D", "E"];
}