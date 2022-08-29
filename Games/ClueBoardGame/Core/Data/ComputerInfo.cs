namespace ClueBoardGame.Core.Data;
public class ComputerInfo
{
    public BasicList<GivenInfo> CluesGiven = new();
    public BasicList<ReceivedInfo> CluesReceived = new();
    public string RoomHeaded = "";
    public string Weapon = ""; // this is the weapon the computer thinks it is
    public string Character = ""; // this is the character the computer thinks it is
}