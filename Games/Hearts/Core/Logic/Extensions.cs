namespace Hearts.Core.Logic;
public static class Extensions
{
    public static int WhoShotMoon(this PlayerCollection<HeartsPlayerItem> thisList)
    {
        var firstList = thisList.Where(xx => xx.HadPoints == true).ToBasicList();
        if (firstList.Count == 1)
        {
            return firstList.Single().Id;
        }
        return 0;
    }
}