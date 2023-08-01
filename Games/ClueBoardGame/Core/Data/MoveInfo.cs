namespace ClueBoardGame.Core.Data;
public class MoveInfo
{
    public int SpaceNumber { get; set; } // if filled out, then this is the space number
    public int RoomNumber { get; set; } // if filled out, then this is the room
    public EnumPositionInfo Position { get;set; }
}