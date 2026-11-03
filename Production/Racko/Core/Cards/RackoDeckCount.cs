namespace Racko.Core.Cards;
public class RackoDeckCount(RackoDelegates delegates) : IDeckCount
{
    public int GetDeckCount()
    {
        int count = delegates.PlayerCount!.Invoke();
        if (count == 2)
        {
            return 40;
        }
        if (count == 3)
        {
            return 50;
        }
        if (count == 4)
        {
            return 60;
        }
        throw new CustomBasicException("Only 2 to 4 players are supported");
    }
}