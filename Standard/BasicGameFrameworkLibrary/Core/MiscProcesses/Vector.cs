namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public struct Vector
{
    public int Column { get; set; }
    public int Row { get; set; }
    public Vector(int row, int column)
    {
        Row = row;
        Column = column;
    }
}