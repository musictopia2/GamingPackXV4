namespace PlainBoardGamesMultiplayer.Core.Data;
public readonly partial record struct EnumColorChoice
{
    private enum EnumInfo
    {
        None,
        Blue //most common color.  if no blue, change to what it is.  has to include in template so the source generator can show the proper color stuff.
    }
}