namespace DutchBlitz.Core.Cards;
public class DutchBlitzDeckCount : IDeckCount
{
    public static bool DoubleDeck { get; set; }
    public int GetDeckCount()
    {
        if (DoubleDeck)
        {
            return 320;
        }
        return 160;
    }
}