namespace BattleshipLite.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class BattleshipLiteVMData : IViewModelData
{
    private BattleshipLiteGameContainer? _gameContainer;
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions
    {
        get
        {
            if (_gameContainer == null)
            {
                _gameContainer = aa.Resolver!.Resolve<BattleshipLiteGameContainer>();
            }
            if (_gameContainer.SaveRoot.GameStatus == EnumGameStatus.PlacingShips)
            {
                if (_gameContainer.SingleInfo is null)
                {
                    return "Waiting For Players";
                }
                if (_gameContainer.SingleInfo.PlayerCategory == EnumPlayerCategory.Self)
                {
                    return "Place your ships";
                }
                return "Waiting For other player to place their ships";

            }
            return "Choose ship to attack";
        }
    }
}