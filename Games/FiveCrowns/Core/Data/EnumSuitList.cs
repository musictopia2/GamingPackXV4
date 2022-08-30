namespace FiveCrowns.Core.Data;
public readonly partial record struct EnumSuitList
{
    private enum EnumInfo
    {
        None,
        Clubs,
        Diamonds,
        Spades,
        Hearts,
        Stars
    }
}