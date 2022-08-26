namespace TileRummy.Core.Data;
public readonly partial record struct EnumDrawType
{
    private enum EnumInfo
    {
        IsNone, FromPool, FromSet, FromHand
    }
}