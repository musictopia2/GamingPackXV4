namespace BattleshipLite.Core.Data;
public class ShipInfo : IBasicSpace
{
    public Vector Vector { get; set; }
    public bool WasShip { get; set; } //you may or not not be the ship.
    public EnumShipStatus ShipStatus { get; set; }
    private BattleshipLiteGameContainer? _gameContainer;
    public string FillColor()
    {
        _gameContainer ??= aa1.Resolver!.Resolve<BattleshipLiteGameContainer>();
        if (_gameContainer.SaveRoot.GameStatus == EnumGameStatus.PlacingShips)
        {
            if (_gameContainer.PlayerList is null)
            {
                return cs1.Blue;
            }
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            if (_gameContainer.SingleInfo!.PlayerCategory is not EnumPlayerCategory.Self)
            {
                return cs1.Blue;
            }
            if (WasShip)
            {
                return cs1.Yellow;
            }
            return cs1.Blue;
        }
        if (ShipStatus == EnumShipStatus.None)
        {
            return cs1.Blue;
        }
        if (ShipStatus == EnumShipStatus.Miss)
        {
            return cs1.Lime;
        }
        return cs1.Blue; //for hits, will show image anyways so its fine.
    }
    public void ClearSpace()
    {
        ShipStatus = EnumShipStatus.None;
        WasShip = false;
    }
    public bool IsFilled()
    {
        return false; //try this way (?)
    }
    
}