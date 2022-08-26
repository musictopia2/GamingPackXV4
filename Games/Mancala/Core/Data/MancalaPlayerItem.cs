namespace Mancala.Core.Data;
public class MancalaPlayerItem : SimplePlayer
{
    public int HowManyPiecesAtHome { get; set; }
    public BasicList<PlayerPieceData> ObjectList { get; set; } = new();
}