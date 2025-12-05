namespace Candyland.Blazor;
public static class Extensions
{
    extension (Dictionary<int, CandylandSquareModel> squares)
    {
        public Dictionary<int, CandylandSquareModel> AddSquare(
            float firstWidth, float firstHeight,
            float secondWidth, float secondHeight,
            float thirdWidth, float thirdHeight,
            float fourthWidth, float fourthHeight
        )
        {
            squares.Add(squares.Count + 1, new CandylandSquareModel()
            {
                FirstWidth = firstWidth,
                FirstHeight = firstHeight,
                SecondWidth = secondWidth,
                SecondHeight = secondHeight,
                ThirdWidth = thirdWidth,
                ThirdHeight = thirdHeight,
                FourthWidth = fourthWidth,
                FourthHeight = fourthHeight
            });
            return squares;
        }
    }
    extension(CandylandPlayerItem player)
    {
        public string GetColor()
        {
            return GetColor(player.Id);
        }
    }
    
    private static string GetColor(int index)
    {
        return index switch
        {
            1 => cc1.Aqua,
            2 => cc1.LimeGreen,
            3 => cc1.Orange,
            4 => cc1.Gray,
            _ => throw new Exception("Only 1 to 4 are supported for piece color"),
        };
    }
}