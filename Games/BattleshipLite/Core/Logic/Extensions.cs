namespace BattleshipLite.Core.Logic;
public static class Extensions
{
    private static string GetLetter(this ShipInfo ship)
    {
        if (ship.Vector.Column == 1)
        {
            return "A";
        }
        if (ship.Vector.Column == 2)
        {
            return "B";
        }
        if (ship.Vector.Column == 3)
        {
            return "C";
        }
        if (ship.Vector.Column == 4)
        {
            return "D";
        }
        if (ship.Vector.Column == 5)
        {
            return "E";
        }
        throw new CustomBasicException("Only 5 columns are supported for battleship lite");
    }
    public static string ShipLocation(this ShipInfo ship)
    {
        string letter = ship.GetLetter();
        return $"{letter}{ship.Vector.Row}";
    }
}