namespace YahtzeeHandsDown.Core.Logic;
public static class Extensions
{
    public static BasicList<ICard> GetInterfaceList(this IDeckDict<YahtzeeHandsDownCardInformation> thisList)
    {
        BasicList<ICard> output = new();
        output.AddRange(thisList);
        return output;
    }
}