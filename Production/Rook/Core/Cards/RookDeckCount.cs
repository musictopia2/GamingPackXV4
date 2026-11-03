namespace Rook.Core.Cards;
public class RookDeckCount : IDeckCount
{
    public int GetDeckCount()
    {
        if (GlobalClass.Container!.PlayerList!.Count == 4)
        {
            return 41;
        }
        return 44;
    }
}