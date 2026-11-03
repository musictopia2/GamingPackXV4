namespace Poker.Core.Data;
public class DisplayCard  //for now 2 classes.  may be able to do as one (?)
{
    public PokerCardInfo? CurrentCard { get; set; }
    public bool WillHold { get; set; }
    public string Text
    {
        get
        {
            if (WillHold == true)
            {
                return "Hold";
            }
            return "";
        }
    }
}