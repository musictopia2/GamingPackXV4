namespace SnakesAndLadders.Core.Data;
[SingletonGame]
[UseStateChanged]
public partial class CurrentPlayerPieceModel
{
    [StateProperty]
    public partial int PlayerId { get; set; }

    [StateProperty]
    public partial int SpaceNumber { get; set; }
}