namespace BattleshipLite.Core.Data;
public class ShipInfo : IBasicSpace
{
    public Vector Vector { get; set; }
    public bool WasShip { get; set; } //you may or not not be the ship.
    public EnumShipStatus ShipStatus { get; set; }
    private BattleshipLiteGameContainer? _gameContainer;
    public string FillColor()
    {
        if (_gameContainer is null)
        {
            _gameContainer = aa.Resolver!.Resolve<BattleshipLiteGameContainer>();
        }
        if (_gameContainer.SaveRoot.GameStatus == EnumGameStatus.PlacingShips)
        {
            if (_gameContainer.PlayerList is null)
            {
                return cs.Blue;
            }
            _gameContainer.SingleInfo = _gameContainer.PlayerList.GetWhoPlayer();
            if (_gameContainer.SingleInfo!.PlayerCategory is not EnumPlayerCategory.Self)
            {
                return cs.Blue;
            }
            if (WasShip)
            {
                return cs.Yellow;
            }
            return cs.Blue;
        }
        if (ShipStatus == EnumShipStatus.None)
        {
            return cs.Blue;
        }
        if (ShipStatus == EnumShipStatus.Miss)
        {
            return cs.Lime;
        }
        return cs.Blue; //for hits, will show image anyways so its fine.
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