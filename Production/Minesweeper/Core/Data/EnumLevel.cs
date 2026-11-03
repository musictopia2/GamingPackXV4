namespace Minesweeper.Core.Data;
public readonly partial record struct EnumLevel
{
    private enum EnumInfo
    {
        Easy = 1,
        Medium,
        Hard
    }
}