namespace Hearts.Core.Logic;
public static class Extensions
{
    extension (PlayerCollection<HeartsPlayerItem> list)
    {
        public int WhoShotMoon
        {
            get
            {
                var firstList = list.Where(xx => xx.HadPoints == true).ToBasicList();
                if (firstList.Count == 1)
                {
                    return firstList.Single().Id;
                }
                return 0;
            }
            
        }
    }
    
}