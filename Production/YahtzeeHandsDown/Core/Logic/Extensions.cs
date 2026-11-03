namespace YahtzeeHandsDown.Core.Logic;
public static class Extensions
{
    extension (IList<YahtzeeHandsDownCardInformation> list)
    {
        public BasicList<ICard> GetInterfaceList()
        {
            BasicList<ICard> output = [.. list];
            return output;
        }
    }   
}