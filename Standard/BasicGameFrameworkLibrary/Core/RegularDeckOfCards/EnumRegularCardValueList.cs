namespace BasicGameFrameworkLibrary.Core.RegularDeckOfCards;
public readonly partial record struct EnumRegularCardValueList
{
    private enum EnumInfo
    {
        None, LowAce, Two, Three, Four, Five, Six,
        Seven, Eight, Nine, Ten,
        Jack, Queen, King, HighAce, Joker = 20, Stop, Continue
    }
}