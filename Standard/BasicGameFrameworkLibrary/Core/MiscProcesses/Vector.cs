namespace BasicGameFrameworkLibrary.Core.MiscProcesses;
public struct Vector(int row, int column) //has to do structs the old fashioned way because the values can change (i think)
{
    public int Column { get; set; } = column;
    public int Row { get; set; } = row;
}