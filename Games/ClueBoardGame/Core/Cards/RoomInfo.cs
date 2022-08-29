namespace ClueBoardGame.Core.Cards;
public class RoomInfo : MainInfo
{
    public GameSpace? Space { get; set; }
    public int RoomPassage { get; set; }
    public BasicList<int> DoorList { get; set; } = new(); //this will list all the doors that you can go through to get in/out of the room
    public override void Populate(int chosen)
    {
        throw new CustomBasicException("I don't think we need to implement populate for RoomInfo.  If I am wrong, rethink");
    }
    public override void Reset() { }
}