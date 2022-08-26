namespace Mancala.Core.Data;
[SingletonGame]
public class MancalaSaveInfo : BasicSavedGameClass<MancalaPlayerItem>, IMappable, ISaveInfo
{
    public bool IsStart { get; set; }
}