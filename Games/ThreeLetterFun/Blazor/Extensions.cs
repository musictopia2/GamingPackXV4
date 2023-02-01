namespace ThreeLetterFun.Blazor;
public static class Extensions
{
    private static readonly BasicList<string> _vowelList = new() { "A", "E", "I", "O", "U" };
    public static string GetColorOfLetter(this string thisLetter)
    {
        if (_vowelList.Count != 5)
        {
            throw new Exception("Must have 5 vowels");
        }
        if (_vowelList.Exists(x => x == thisLetter))
        {
            return cc1.Red;
        }
        return cc1.Black;
    }
}